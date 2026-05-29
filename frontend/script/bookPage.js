import { showDiscretePopup, showInvasivePopup } from "./generalScript.js";
import { checkAndSetUserStorage, userLoggedIn } from "./userUtilities.js";
import { ajaxCall, ajaxCallPromise } from "./ajaxCalls.js";
import { BASE_URL } from "./config.js";

var bookID = localStorage.getItem("bookID");

var userActive = userLoggedIn();

// Quill starter \/

const ReviewToolbarOptions = [
  [{ header: [1, 2, 3, false] }],
  ["bold", "italic", "underline"],
  ["link"],
  [{ list: "ordered" }, { list: "bullet" }],
  [{ align: [] }],
  ["clean"],
];

$(document).ready(function () {
  // $(".modal-overlay").hide();

  GetUserSentimentFromRatings();

  GetAverageRatingForBook();

  if (bookID) {
    GetBookData();
  } else {
    window.location.href = "../main/index.html";
  }

  var quill = new Quill("#reviewEditor", {
    theme: "snow",
    placeholder: "Write your review...",
    modules: {
      toolbar: ReviewToolbarOptions,
    },
  });

  var maxChars = 300;

  // Event listener for text change in Quill editor

  quill.on("text-change", function () {
    var text = quill.getText().trim(); // Get plain text
    var charCount = text.length;

    $("#charCount").text(charCount + "/" + maxChars + " characters");

    if (charCount > maxChars) {
      $("#charCount").addClass("exceeded");
    } else {
      $("#charCount").removeClass("exceeded");
    }
  });

  // Review form sendoff

  $("#reviewForm").on("submit", function (e) {
    e.preventDefault(); // Prevent default form submission

    var reviewContent = quill.getText().trim(); // Plain text content
    var htmlContent = quill.root.innerHTML; // HTML content
    var selectedStarRating = $("input[name='star']:checked").val();

    // Check character length
    if (reviewContent.length > maxChars) {
      showInvasivePopup("x", "Alert", "Review cannot exceed 300 characters.");
      return false;
    }

    if (!selectedStarRating) {
      showInvasivePopup("x", "Alert", "Please select a rating.");
      return false;
    }

    // Check for line-drop constraints
    var lineDropCount = (htmlContent.match(/<p><br><\/p>/g) || []).length;
    var consecutiveLineDrops = (
      htmlContent.match(/(<p><br><\/p>\s*){2,}/g) || []
    ).length;

    if (consecutiveLineDrops > 0 || lineDropCount > 4) {
      showInvasivePopup(
        "x",
        "Alert",
        "Review cannot have more than 2 consecutive line-drops and 4 line-drops in total.",
      );
      return false;
    }

    var header = $("#header").val();

    // Collect data from form inputs
    var formData = {
      userID: userActive.userID,
      bookID: parseInt(bookID),
      ratingStars: parseInt(selectedStarRating),
      description: htmlContent,
      dateCreated: new Date(),
      header: header,
    };

    let api = `${BASE_URL}/Rating/InsertUserReview`;

    ajaxCall(
      "POST",
      api,
      JSON.stringify({
        userID: userActive.userID,
        bookID: parseInt(bookID),
        ratingStars: parseInt(selectedStarRating),
        description: htmlContent,
        dateCreated: new Date(),
        header: header,
      }),
      submitReviewSCB,
      submitReviewECB,
    );

    function submitReviewSCB(outcome) {
      switch (outcome) {
        case 0:
          showInvasivePopup(
            "x",
            "Alert",
            "You have not had interaction with this book yet",
          );
          break;
        case 1:
          showInvasivePopup(
            "x",
            "Alert",
            "You have not marked the book read yet!",
          );
          break;
        case 2:
          showInvasivePopup(
            "v",
            "Alert",
            "You have successfully submitted a review!",
          );
          break;
        case 3:
          showInvasivePopup(
            "v",
            "Alert",
            "You have successfully submitted a review and replace your old one!",
          );
          break;
      }
    }

    function submitReviewECB(error) {
      showInvasivePopup(
        "x",
        "Error",
        "Unable to submit review. Check your interent connection or submit a bug report."`${error}`,
      );
    }

    // Send data via AJAX if all conditions are met
  });

  DoesUserOwnThisBook();
  DisplayUsersBookReview();

  fetchBookDetailsAndReviews(); // Initial fetch when the page loads
});

$("#LeaveRatingBttn").on("click", function () {
  $(".modal-overlay").fadeIn(); // Show the modal
});

// Close modal when clicking the close button or outside the modal container
$(document).on("click", function (e) {
  // Close when clicking on the close button
  if ($(e.target).closest(".close-modal").length) {
    $(".modal-overlay").fadeOut();
  }

  // Close when clicking outside the modal container
  if ($(e.target).is(".modal-overlay")) {
    $(".modal-overlay").fadeOut();
  }
});

function GetAverageRatingForBook() {
  // Gets average (float) rating of the book to display as stars + text
  let api = `${BASE_URL}/Book/GetAverageRatingForBook?BookID=${bookID}`;
  return ajaxCallPromise("GET", api, "").then((avgRating) => {
    const roundedRating = Math.round(avgRating);
    const ratingText = avgRating.toFixed(1);

    let verbalRating = "";

    if (avgRating == 0) {
      verbalRating = "No user ratings submitted";
    } else if (avgRating >= 1 && avgRating < 2) {
      verbalRating = "Disappointing";
    } else if (avgRating >= 2 && avgRating < 3) {
      verbalRating = "Okay";
    } else if (avgRating >= 3 && avgRating < 4) {
      verbalRating = "Good";
    } else if (avgRating >= 4 && avgRating <= 5) {
      verbalRating = "Great!";
    }

    // Update the stars
    let starsHTML = "";
    for (let i = 0; i < 5; i++) {
      if (i < roundedRating) {
        starsHTML += '<i class="fas fa-star star-filled"></i>';
      } else {
        starsHTML += '<i class="fas fa-star star-empty"></i>';
      }
    }

    // Update the DOM elements
    $("#StarsAndVerbalRating .stars-display").html(starsHTML);
    if (ratingText == 0.0) {
      $("#StarsAndVerbalRating .rating-text").text(`${verbalRating}`);
    } else {
      $("#StarsAndVerbalRating .rating-text").text(
        `${ratingText} - ${verbalRating}`,
      );
    }
  });
}

function updateStars(rating) {
  const starsContainer = $("#StarsAndVerbalRating").empty();

  for (let i = 1; i <= 5; i++) {
    const starClass = i <= rating ? "filled-star" : "unfilled-star";
    starsContainer.append(`<span class="fa fa-star ${starClass}"></span>`);
  }
}

/* Gets book data from server */

function GetBookData() {
  let api = `${BASE_URL}/Book/bookPage`;
  ajaxCall("GET", api, { id: bookID }, GetBookDataSCB, GetBookDataECB);
}
function GetBookDataSCB(Book) {
  // console.log(Book);
}
function GetBookDataECB() {
  //  console.log("ERROR!");
}

function DisplayBookData(book) {
  // Clean up the Title, Subtitle, and Authors to remove all instances of "

  $("#BookPictureHolder").empty();
  book.title = book.title.replace(/"/g, "");
  book.subtitle = book.subtitle.replace(/"/g, "");
  book.authors = book.authors.replace(/"/g, "");

  // Clean up the description to ensure no more than one consecutive "
  book.description = book.description.replace(/"{2,}/g, '"');

  // Ensure the description does not end with a "
  if (book.description.endsWith('"')) {
    book.description = book.description.slice(0, -1);
  }

  // Update the DOM with the cleaned data
  $("#book-name").html(book.title);

  if (book.thumbnail == "Image link unavailable.") {
    var imgElement = $("<img>")
      .attr(
        "src",
        "https://static.vecteezy.com/system/resources/thumbnails/022/014/063/small_2x/missing-picture-page-for-website-design-or-mobile-app-design-no-image-available-icon-vector.jpg",
      )
      .attr("alt", book.title);
  } else {
    var imgElement = $("<img>")
      .attr("src", book.thumbnail)
      .attr("alt", book.title);
  }
  $("#BookPictureHolder").append(imgElement);

  $("#BookTitle").text("Title: " + book.title);

  if (book.subtitle == "") {
    $("#BookSubtitle").hide();
  } else {
    $("#BookSubtitle").text("Subtitle: " + book.subtitle);
  }

  // Modified authors section
  if (book.authors == "" || book.authors == " No authors specified.") {
    $("#BookAuthors").text(" Authors: Not available");
  } else {
    if (book.authors && book.authors.length > 0) {
      let authorsArray = book.authors.split(", ");
      let authorsHTML = authorsArray
        .map((authorEntry) => {
          let [id, name] = authorEntry.split(":");
          return `<a href="writerPage.html" onclick="sessionStorage.setItem('authorID', ${id})">${name}</a>`;
        })
        .join(", ");
      $("#BookAuthors").html("Authors:&nbsp  " + authorsHTML);
    } else {
      $("#BookAuthors").text("Authors: Not available");
    }
  }
  // Replacing &amp; with & in categories
  let cleanedCategories = book.categories.replace(/&amp;/g, "&");
  $("#BookCategories").text("Categories: " + cleanedCategories);

  if (book.description == "") {
    $("#BookDescriptionContents").text("No description available.");
  } else {
    $("#BookDescriptionContents").text(book.description);
  }

  if (book.publisher == "") {
    $("#BookPublisher").text(`No publisher available.`);
  } else {
    $("#BookPublisher").text(`Publisher: ${book.publisher}`);
  }

  $("#BookPublishedDate").text(
    `Published Date: ${formatDate(book.publishedDate)}`,
  );

  if (book.printType == "BOOK") {
    $("#BookPrintType").text(`Print Type: Book`);
  } else {
    $("#BookPrintType").text(`Print Type: ${book.printType}`);
  }

  if (book.pageCount == 0) {
    $("#BookPageCount").text("No page count available.");
  } else {
    $("#BookPageCount").text(`Page Count: ${book.pageCount}`);
  }

  $("#BookPreviewLink").html(
    `<a href="${book.previewLink}" target="_blank">Book Preview Link</a>`,
  );

  if (book.maturityRating == "NOT_MATURE") {
    $("#BookMaturityRating").text(`Maturity Rating: Not Mature`);
  } else {
    $("#BookMaturityRating").text(`Maturity Rating: ${book.maturityRating}`);
  }

  $("#BookLanguage").text(`Language: ${book.language.toUpperCase()}`);
  $("#BookInfoLink").html(
    `<a href="${book.infoLink}" target="_blank">Book Info Link</a>`,
  );

  if (book.isbn10 == "") {
    $("#BookISBN10").text(`No ISBN10 available.`);
  } else {
    $("#BookISBN10").text(`ISBN10: ${book.isbn10}`);
  }

  if (book.isbn13 == "") {
    $("#BookISBN13").text(`No ISBN13 available.`);
  } else {
    $("#BookISBN13").text(`ISBN13: ${book.isbn13}`);
  }

  $("#BookCanonicalVolumeLink").html(
    `<a href="${book.canonicalVolumeLink}" target="_blank">Book Canonical Volume Link</a>`,
  );

  var copyType = book.isEbook ? "Digital" : "Physical";
  $("#BookIsEbook").text(`Copy Type: ${copyType}`);

  $("#BookPrice").text(`Price: ${book.price} $`);

  if (book.isEbook && book.downloadLink.trim() !== "") {
    $("#DownloadBttn").show();

    // Attach the click event listener to the div
    $("#DownloadBttn").on("click", function () {
      window.location.href = book.downloadLink;
    });
  }
}

function userBuyBook(BookID) {
  if (userActive.userID == 0) {
    showInvasivePopup("x", "Alert", "You must be logged in to purchase a book");
    return;
  }

  let api = `${BASE_URL}/BookStoreUser/CheckIfUserOwnsBook?UserID=${
    userActive.userID
  }&BookID=${parseInt(BookID)}`;

  // Make the AJAX call with the callback functions defined below
  ajaxCall("GET", api, "", CheckIfUserOwnsBookSCB, CheckIfUserOwnsBookECB);

  // Success callback function
  function CheckIfUserOwnsBookSCB(result) {
    if (result) {
      showInvasivePopup("x", "Error", "You already own this book");
      return; // Stop further execution if the user already owns the book
    } else {
      // Continue with the next steps if the user doesn't own the book
      api = `${BASE_URL}/BookStoreUser/DoesBookUserConnectionExist`;
      ajaxCallPromise("GET", api, {
        UserID: userActive.userID,
        BookID: BookID,
      })
        .then(function (DoesUserBookConnectionExist) {
          if (!DoesUserBookConnectionExist) {
            const api = `${BASE_URL}/BookStoreUser/AddNewUserBookRelation`;
            BookID = parseInt(BookID); // Convert BookID to integer

            return ajaxCallPromise(
              "POST",
              api,
              JSON.stringify({ UserID: userActive.userID, BookID: BookID }),
            );
          }
        })
        .then(function () {
          let api = `${BASE_URL}/BookStoreUser/UserBuyBook`;
          return ajaxCallPromise(
            "PUT",
            api,
            JSON.stringify({
              UserID: userActive.userID,
              BookID: parseInt(BookID),
            }),
          );
        })
        .then((status) => {
          if (status) {
            let api = `${BASE_URL}/BookStoreUser/RecordBookPurchaseInHistory`;
            return ajaxCallPromise(
              "POST",
              api,
              JSON.stringify({
                UserID: userActive.userID,
                BookID: parseInt(BookID),
              }),
            );
          } else {
            throw new Error("Unable to record purchase in history");
          }
        })
        .then(() => {
          showInvasivePopup("v", "Success", "Purchase successful!");
          document
            .querySelectorAll('img[src="../media/bell.svg"]')
            .forEach((img) => {
              img.src = "../media/bell-dot.svg";
              img.style.filter =
                "invert(78%) sepia(75%) saturate(724%) hue-rotate(349deg) brightness(97%) contrast(103%)";
              img.classList.add("animate__animated", "animate__heartBeat");

              function animateBell() {
                img.classList.remove("animate__heartBeat");
                void img.offsetWidth;
                img.classList.add("animate__heartBeat");
                setTimeout(animateBell, 2500);
              }
              animateBell();
            });

          let api = `${BASE_URL}/BookStoreUser/SendBookBoughtNotification`;
          return ajaxCallPromise(
            "POST",
            api,
            JSON.stringify({
              BookID: parseInt(BookID),
              UserID: userActive.userID,
            }),
          );
        })
        .then((notificationStatus) => {
          if (notificationStatus) {
          } else {
            showDiscretePopup(
              "Error",
              "Failed to send you a notification, but the book is yours!",
            );
          }
        })
        .catch((error) => {
          showInvasivePopup(
            "x",
            "Alert",
            "An error occurred during the purchase process." + error,
          );
        });
    }
  }

  // Error callback function
  function CheckIfUserOwnsBookECB(error) {
    showInvasivePopup(
      "x",
      "Error",
      "Failed to check book ownership. Please check your internet connection or submit a bug report - " +
        error,
    );
  }
}

$(document).on("click", "#BookBuyBttn", function () {
  userBuyBook(bookID);
});

/////////////////////////////////////////////

$(document).on("click", "#BookWishlistBttn", function () {
  let api = `${BASE_URL}/BookStoreUser/DoesBookUserConnectionExist`;

  return ajaxCallPromise("GET", api, {
    UserID: userActive.userID,
    BookID: bookID,
  })
    .then(function (DoesUserBookConnectionExist) {
      if (!DoesUserBookConnectionExist) {
        api = `${BASE_URL}/BookStoreUser/AddNewUserBookRelation`;
        bookID = parseInt(bookID);

        return ajaxCallPromise(
          "POST",
          api,
          JSON.stringify({ UserID: userActive.userID, BookID: bookID }),
        ).then(function () {
          api = `${BASE_URL}/BookStoreUser/ToggleBookUserToWishlist`;

          return ajaxCallPromise(
            "POST",
            api,
            JSON.stringify({ UserID: userActive.userID, BookID: bookID }),
          ).then(function () {
            showInvasivePopup("v", "Wishlist", "Book added to wishlist!");
            return;
          });
          // .catch(() => {
          //   console.log("ERROR!");
          // });
        });
      } else {
        // Exists
        bookID = parseInt(bookID);

        let api = `${BASE_URL}/BookStoreUser/ToggleBookUserToWishlist`;

        return ajaxCallPromise(
          "POST",
          api,
          JSON.stringify({ UserID: userActive.userID, BookID: bookID }),
        ).then(function (answer) {
          switch (answer) {
            case -1: {
              showInvasivePopup("x", "Error", "you already own this book");
              break;
            }
            case 0: {
              showDiscretePopup("Wishlist", "Removed from wishlist");
              break;
            }
            case 1: {
              showDiscretePopup(
                "Wishlist",
                "Book added to wishlist successfully",
              );
              break;
            }
          }
        });
        // .catch(() => {
        //   console.log("ERROR!");
        // });
      }
    })
    .catch((error) => {
      showInvasivePopup(
        "x",
        "Error",
        "Error in the wishlist process. Please check your interent connection or submit a bug report." +
          error,
      );
    });
});

function formatDate(dateString) {
  const date = new Date(dateString);
  const day = String(date.getDate()).padStart(2, "0");
  const month = String(date.getMonth() + 1).padStart(2, "0"); // Months are 0-based
  const year = date.getFullYear();
  return `${day}/${month}/${year}`;
}

function DoesUserOwnThisBook() {
  let userActive = userLoggedIn();

  let api = `${BASE_URL}/BookStoreUser/CheckUserConnectionToBookForTrade?userID=${userActive.userID}&BookID=${bookID}`;

  return ajaxCallPromise("GET", api, "").then(
    function (DoesUserBookConnectionExist) {
      if (DoesUserBookConnectionExist > 0) {
        $("#BooksOwner").css("display", "block");
      }
    },
  );
}

// Function to display the user's book review
function DisplayUsersBookReview() {
  let userActive = userLoggedIn();

  let api = `${BASE_URL}/Rating/GetUsersBookReview`;

  ajaxCallPromise("GET", api, {
    userID: userActive.userID,
    bookID: bookID,
  }).then(function (review) {
    if (review.length > 0) {
      let tempDate = new Date(review[0].dateRated);
      let options = { year: "numeric", month: "long", day: "numeric" };
      let formattedDate = tempDate.toLocaleDateString(undefined, options);

      // Display the user's review
      $("#UserRatingDisplayer .header").text('"' + review[0].header + '"');
      $("#UserRatingDisplayer .date").text(formattedDate);
      $("#UserRatingDisplayer .name").html("By: <br>" + userActive.userName);
      // $("#UserRatingDisplayer .star-ranking").html('★'.repeat(review[0].rating) + '☆'.repeat(5 - review[0].rating));
      fillStars(review[0].rating);
      $("#UserRatingDisplayer .votes").text(review[0].totalScore);
      $("#UserRatingDisplayer .description").html(review[0].description);
      $("#LeaveRatingBttn").text("Change Review");
    } else {
      $("#UserRatingDisplayer").html("No user review found.");
    }
  });
}

// Function to fill stars based on rating
function fillStars(rating) {
  // Get all star elements
  const stars = document.querySelectorAll(
    "#UserRatingDisplayer .stars-container .star",
  );
  // Loop through each star
  stars.forEach((star, index) => {
    // Add 'filled' class if index is less than rating
    if (index < rating) {
      star.classList.add("filled-star");
    } else {
      star.classList.remove("filled-star");
    }
  });
}

// Function to fetch book details and reviews
async function fetchBookDetailsAndReviews() {
  try {
    // Fetch book details and reviews
    let bookDetails = await fetchBookDetails(bookID);
    let userID = getUserIDFromStorage();
    const reviews = await fetchTopFiveReviews(bookID, userID);

    // Populate the book details into the DOM
    //console.log(bookDetails);
    DisplayBookData(bookDetails);

    const ulElement = document.querySelector(".splide__list"); // Get the <ul> element to append slides

    // Clear existing slides
    if (ulElement) {
      ulElement.innerHTML = ""; // Clear old slides
    } else {
      return;
    }

    // Add new slides
    reviews.slice(0, 5).forEach((review) => {
      const liElement = document.createElement("li");
      liElement.className = "splide__slide";

      // Create review card
      const reviewCard = document.createElement("div");
      reviewCard.className = "review-card";
      reviewCard.innerHTML = `
        <div class="header"></div>
        <div class="book-name"></div>
        <div class="date"></div>
        <div class="meta">
          <div class="name"></div>
          <div class="star-ranking"></div>
          <div class="votes"></div>
        </div>
        <div class="description">
          <p></p>
        </div>
        <div class="footer">
          <div class="useful">Was the review useful?</div>
          <div class="buttons">
            <button class="vote-button upvote-button" data-rating-id="${review.ratingID}">+1</button>
            <button class="vote-button downvote-button" data-rating-id="${review.ratingID}">-1</button>
          </div>
        </div>
      `;

      liElement.appendChild(reviewCard); // Append review card to <li> element

      ulElement.appendChild(liElement); // Append <li> element to <ul> element

      updateSlideContent(liElement, review); // Update slide content and functionality
    });

    // Initialize and mount Splide after reviews are updated
    if (ulElement.querySelector(".splide__slide")) {
      const splide = new Splide(".splide", {
        type: "loop",
        perPage: 1,
        autoplay: true,
      });
      splide.mount();
    } else {
      $("#BookReviewsContents")
        .addClass("no-reviews")
        .text("No reviews found for this book.");
    }
  } catch (error) {
    showInvasivePopup(
      "x",
      "Error",
      "Unable to get vital page details. Please check your interent connection or submit a bug report. " +
        error,
    );
  }
}

function getUserIDFromStorage() {
  const user =
    JSON.parse(localStorage.getItem("user")) ||
    JSON.parse(sessionStorage.getItem("user"));
  return user ? user.userID : 0;
}

async function fetchBookDetails(bookID) {
  const api = `${BASE_URL}/Book/bookPage`;

  const data = { id: bookID }; // Data to send with the request
  try {
    // Call ajaxCallPromise with GET method, API endpoint, and data
    const bookDetails = await ajaxCallPromise("GET", api, data);
    return bookDetails;
  } catch (error) {
    throw new Error("Failed to fetch book details");
  }
}

async function fetchTopFiveReviews(bookID, userID) {
  const api = `${BASE_URL}/Rating/GetTopFiveRatingsReviewsForBook`;

  const data = { userID, bookID }; // Data to send with the request

  try {
    const reviews = await ajaxCallPromise("GET", api, data);
    return reviews;
  } catch (error) {
    throw new Error("Failed to fetch top reviews");
  }
}

function updateSlideContent(slide, review) {
  const formattedDate = new Date(review.dateCreated).toLocaleDateString(
    undefined,
    { year: "numeric", month: "long", day: "numeric" },
  );

  slide.querySelector(".header").textContent = `"${review.header}"`;
  slide.querySelector(".book-name").textContent = review.bookName;
  slide.querySelector(".date").textContent = formattedDate;
  slide.querySelector(".name").innerHTML = "By: <br>" + review.userName;

  const maxStars = 5;
  const starContainer = slide.querySelector(".meta .star-ranking");

  // Clear previous content
  starContainer.innerHTML = "";

  // Add "Star Ranking" text
  const rankingText = document.createElement("div");
  rankingText.className = "star-ranking-text";
  rankingText.textContent = "Star Ranking";
  starContainer.appendChild(rankingText);

  // Add star labels
  const starsContainer = document.createElement("div");
  starsContainer.className = "stars-container";
  for (let i = 0; i < maxStars; i++) {
    const starLabel = document.createElement("label");
    starLabel.className = "star";
    starLabel.classList.toggle("filled-star", i < review.ratingStars);
    starLabel.classList.toggle("unfilled-star", i >= review.ratingStars);
    starsContainer.appendChild(starLabel);
  }
  starContainer.appendChild(starsContainer);

  slide.querySelector(".votes").textContent = `${review.totalScore}`;
  slide.querySelector(".description p").innerHTML = review.description;

  updateVoteButtonStyles(slide, review);
  addVoteButtonEventListeners(slide, review);
}

function updateVoteButtonStyles(slide, review) {
  const upvoteButton = slide.querySelector(".upvote-button");
  const downvoteButton = slide.querySelector(".downvote-button");

  upvoteButton.classList.toggle("active", review.userRating === 1);
  downvoteButton.classList.toggle("inactive", review.userRating === -1);
}

function addVoteButtonEventListeners(slide, review) {
  const upvoteButton = slide.querySelector(".upvote-button");
  const downvoteButton = slide.querySelector(".downvote-button");
  //console.log(review);

  // Add event listeners to toggle vote
  upvoteButton.addEventListener("click", () =>
    handleVoteButtonClick(
      review.ratingID,
      1,
      upvoteButton,
      downvoteButton,
      review.userID,
    ),
  );
  downvoteButton.addEventListener("click", () =>
    handleVoteButtonClick(
      review.ratingID,
      -1,
      upvoteButton,
      downvoteButton,
      review.userID,
    ),
  );
}

async function handleVoteButtonClick(
  ratingID,
  rating,
  upvoteButton,
  downvoteButton,
  reviewUserID,
) {
  const userID = getUserIDFromStorage();

  if (!userID || userID === 0) {
    showInvasivePopup("x", "Alert", "Please log in to rate reviews.");
    return;
  }

  // Check if the current user is trying to rate their own review
  if (userID === reviewUserID) {
    showInvasivePopup("x", "Alert", "You cannot rate your own review.");
    return;
  }

  // Check current rating state to determine if the user is removing the rating
  const currentRating = getCurrentRating(upvoteButton, downvoteButton);
  let newRating = rating;

  // If the current rating matches the new rating, set it to 0 (remove rating)
  if (currentRating === rating) {
    newRating = 0;
  }

  const api = `${BASE_URL}/Rating/RateReview`;

  const response = await fetch(api, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({
      userID: userID,
      ratingID: ratingID,
      score: newRating,
    }),
  });

  if (!response.ok) {
    showInvasivePopup("x", "Alert", "Failed to submit rating.");
    return;
  }

  const result = await response.json();
  handleRatingOutcome(result, upvoteButton, downvoteButton, newRating);
}

function handleRatingOutcome(result, upvoteButton, downvoteButton, newRating) {
  if (newRating === 0) {
    upvoteButton.classList.remove("active");
    downvoteButton.classList.remove("inactive");
  } else if (result === 1) {
    upvoteButton.classList.add("active");
    downvoteButton.classList.remove("inactive");
  } else if (result === -1) {
    upvoteButton.classList.remove("active");
    downvoteButton.classList.add("inactive");
  } else {
    showInvasivePopup(
      "x",
      "Alert",
      "An error occurred while processing your rating.",
    );
  }
}

function getCurrentRating(upvoteButton, downvoteButton) {
  if (upvoteButton.classList.contains("active")) {
    return 1;
  } else if (downvoteButton.classList.contains("inactive")) {
    return -1;
  }
  return 0;
}

function displaySentimentAnalysis(response) {
  const sentimentBlock = document.getElementById("AiSentimentBlock");

  // Clear previous content
  sentimentBlock.innerHTML = "";

  if (!response || response.length === 0) {
    // Handle case where there's no data or an error occurred
    sentimentBlock.innerHTML = `
      <div class="error-message">
        No data, or could not fetch sentiment analysis. Please try again later.
      </div>
    `;
    return;
  }

  // Sort response by score in descending order
  response.sort((a, b) => b.score - a.score);

  // Take the top 2 sentiments
  const topSentiments = response.slice(0, 2);

  // Create content for top sentiments
  let sentimentContent = topSentiments
    .map((sentiment) => {
      const label =
        sentiment.label.charAt(0).toUpperCase() + sentiment.label.slice(1); // Capitalize the first letter
      const percentage = Math.round(sentiment.score * 100); // Convert score to percentage
      return `<div class="sentiment-item">
              <strong>${label}</strong>: ${percentage}%
            </div>`;
    })
    .join("");

  // Add the sentiment content to the sentiment block with a header
  sentimentBlock.innerHTML = `
    <div class="sentiment-header">According to user reviews, the readers view this book:</div>
    <div class="sentiment-content">${sentimentContent}</div>
  `;

  // Optionally add styling
  sentimentBlock.style.padding = "20px";
  sentimentBlock.style.backgroundColor = "#f9f9f9";
  sentimentBlock.style.borderRadius = "10px";
  sentimentBlock.style.textAlign = "center";
  sentimentBlock.style.fontSize = "18px";
  sentimentBlock.style.lineHeight = "1.6em";
}

// Example usage with response
function GetUserSentimentFromRatings() {
  let api = `${BASE_URL}/Rating/GetRatingWrittenDataForSentimentAnalysis?bookID=${parseInt(
    bookID,
  )}`;

  return ajaxCallPromise("GET", api, "").then((data) => {
    let formattedData = data
      .map((item) => {
        let header = item.header || "";
        let description = item.description || "";

        description = description.replace(/<[^>]+>/g, ""); // Remove HTML tags

        return `${header}: ${description}`;
      })
      .join(", ");

    // Check if formattedData is empty or null
    if (!formattedData || formattedData.trim() === "") {
      // No user interaction, show "no data available"
      displaySentimentAnalysis(null);
      return;
    }

    let sentimentApi = "${BASE_URL}/Rating/GetSentimentAnalysis";

    let payload = {
      text: formattedData,
    };

    ajaxCall("POST", sentimentApi, JSON.stringify(payload), successCB, errorCB);

    function successCB(response) {
      displaySentimentAnalysis(response); // Call the function to display the sentiment analysis
    }

    function errorCB(error) {
      displaySentimentAnalysis(null); // Pass null to handle the error case
    }
  });
}
