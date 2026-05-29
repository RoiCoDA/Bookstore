import { userLoggedIn } from "./userUtilities.js";
import { showDiscretePopup, showInvasivePopup } from "./generalScript.js";
import { ajaxCall, ajaxCallPromise } from "./ajaxCalls.js";
import { BASE_URL } from "./config.js";

var userActive = userLoggedIn();

var saleBooks;
var books = [];
var booksDisplayed = 0; // Tracks how many books have been displayed
const booksPerPage = 12; // Number of books to display per "page"

// Function to read books from the database
$(document).ready(function () {
  fetchTopFiveReviews().then((reviews) => {
    updateReviewElements(reviews);
  });

  var splide = new Splide(".splide", {
    type: "loop",
    perPage: 1,
    autoplay: true,
  });
  splide.mount();

  // localStorage.removeItem('bookID');
  function ReadBooksAdHoc() {
    let api = `${BASE_URL}/Book/GetAllSitewideBooks`;
    ajaxCall("GET", api, "", ReadBooksAdHocSCB, ReadBooksAdHocECB);
  }

  function ReadBooksAdHocSCB(ListOfBooks) {
    books = ListOfBooks;
    renderBooks(books);
  }

  function ReadBooksAdHocECB() {
    showInvasivePopup("x", "Error", "Error connecting to server");
  }

  ReadBooksAdHoc();

  //getFlashSaleBooks();

  GetTop10UserOwnedBooks();
});

// Event listener for "Show More" button
$("#AllBooksLibraryIndexBttn").on("click", function () {
  renderBooks(); // Load more books when "Show More" is clicked
  scrollToContainer(); // Scroll to the AllBooksLibraryContainer after loading more books
});

// Event listener for "Back to Start" button
$("#ResetButton").on("click", function () {
  booksDisplayed = 0; // Reset the counter
  $(".BookContainer").empty(); // Clear the container
  renderBooks(); // Show the first set of books
  $("#AllBooksLibraryIndexBttn").show(); // Ensure "Show More" button is visible
  const container = document.getElementById("AllBooksLibraryContainer");
  if (container) {
    container.scrollIntoView({ behavior: "smooth", block: "start" });
  }
});

// Function to render books
function renderBooks() {
  if (
    !(
      window.location.pathname.endsWith("/main/index.html") ||
      window.location.pathname.endsWith("/main/")
    )
  ) {
    return;
  }
  var container = document.querySelector(".BookContainer");
  if (container) {
    // Determine the books to display
    const booksToDisplay = books.slice(
      booksDisplayed,
      booksDisplayed + booksPerPage,
    );

    // Append the books to the container
    booksToDisplay.forEach((book) => {
      container.innerHTML += createBookHTML(book);
    });

    booksDisplayed += booksPerPage; // Update the counter

    // Attach event listeners after rendering
    document.querySelectorAll(".view-book-btn").forEach((item) => {
      item.addEventListener("click", function () {
        const bookID = this.getAttribute("data-book-id");
        localStorage.setItem("bookID", bookID); // Stores the bookID in local storage
        window.location.href = "bookPage.html"; // Navigate to the next page
      });
    });

    // Hide the "Show More" button if all books are displayed
    if (booksDisplayed >= books.length) {
      $("#AllBooksLibraryIndexBttn").hide();
      $("#ResetButtonContainer").show(); // Show "Back to Start" button
    } else {
      $("#ResetButtonContainer").hide(); // Hide "Back to Start" button if "Show More" is still visible
    }
  }
}

// Function to create HTML for each book
function createBookHTML(book) {
  return `
    <li class="Book3DContainer">
            <div class="Book" onclick="return true">
                <div class="BookPic" style="background-image: url('${book.thumbnail}');"></div>
                <div class="BookInfo">
                    <header>
                        <h1>${book.title}</h1>
                        <span class="year">${book.authors}</span>
                    </header>
                    <p>${book.description}</p>
                    <button class="wishlist-book-btn" data-book-id="${book.bookID}">Wishlist Book</button>
                    <button class="view-book-btn" data-book-id="${book.bookID}">View Book</button>
                </div>
            </div>
        </li>
        `;
}

// Function to scroll to the AllBooksLibraryContainer
function scrollToContainer() {
  const container = document.getElementById("AllBooksLibraryContainer");
  if (container) {
    container.scrollIntoView({ behavior: "smooth", block: "end" });
  } else {
  }
}

$(document).on("click", ".wishlist-book-btn", function () {
  if (userActive.userID == 0) {
    showInvasivePopup(
      "x",
      "Wishlist",
      "You must log-in first to wishlist a book",
    );
    return;
  }
  ToggleBookWishlist(this);
});

/////////////////////// Sale stuff ///////////////////////////

// Initialize the flash sale section
//renderFlashSaleBooks(saleBooks);

//////////////////////////////////////////////////////////////

// Function to toggle book wishlist status
function ToggleBookWishlist(book) {
  let api = `${BASE_URL}/BookStoreUser/DoesBookUserConnectionExist`;
  let bookID = $(book).attr("data-book-id");

  return ajaxCallPromise("GET", api, {
    UserID: userActive.userID,
    BookID: bookID,
  })
    .then(function (DoesUserBookConnectionExist) {
      if (!DoesUserBookConnectionExist) {
        api = `${BASE_URL}/BookStoreUser/AddNewUserBookRelation`;
        bookID = parseInt(bookID); // Convert bookID to integer

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
        });
      } else {
        // Exists
        bookID = parseInt(bookID); // Convert bookID to integer
        api = `${BASE_URL}/BookStoreUser/ToggleBookUserToWishlist`;

        return ajaxCallPromise(
          "POST",
          api,
          JSON.stringify({ UserID: userActive.userID, BookID: bookID }),
        ).then(function (answer) {
          switch (answer) {
            case -1: {
              showInvasivePopup("x", "Wishlist", "Book already owned!");
              break;
            }
            case 0: {
              showDiscretePopup("Wishlist", "Removed from wishlist!");
              break;
            }
            case 1: {
              showDiscretePopup("Wishlist", "Added to wishlist successfully!");
              break;
            }
          }
        });
      }
    })
    .catch(() => {
      showInvasivePopup(
        "x",
        "Error",
        "Could not add book to wishlist! please submit a bug report on this issue.",
      );
    });
}

// Function to fetch top five reviews
function fetchTopFiveReviews() {
  let userID =
    JSON.parse(localStorage.getItem("user")) ||
    JSON.parse(sessionStorage.getItem("user"));
  userID = userID.userID;
  let api = `${BASE_URL}/Rating/GetTopFiveRatingsReviews`;
  return ajaxCallPromise("GET", api, { userID: userID });
}

// Function to update the review elements on the page
function updateReviewElements(reviews) {
  //console.log("Updating review elements with the following reviews:", reviews);

  // Get all slides
  const slides = document.querySelectorAll(".splide__slide");
  //console.log("Total slides found:", slides.length);

  // Filter to get only the actual slides (excluding clones)
  const actualSlides = Array.from(slides).filter(
    (slide) => !slide.classList.contains("splide__slide--clone"),
  );

  //console.log("Actual slides found:", actualSlides.length);

  reviews.forEach((review, index) => {
    // Debugging output
    //console.log(`Processing review ${index + 1}`, review);
    if (index >= actualSlides.length) {
      return; // Avoid exceeding available actual slides
    }

    const slide = actualSlides[index];
    if (slide) {
      //console.log("Updating slide:", slide);

      // Format date
      let tempDate = new Date(review.dateCreated);
      let options = { year: "numeric", month: "long", day: "numeric" };
      let formattedDate = tempDate.toLocaleDateString(undefined, options);

      // Update elements
      slide.querySelector(".header").textContent = `"${review.header}"`;
      slide.querySelector(".book-name").textContent = review.bookTitle;
      slide.querySelector(".date").textContent = formattedDate;
      slide.querySelector(".meta .name").innerHTML =
        "By: <br>" + review.userName;

      const maxStars = 5; // Maximum number of stars
      const starContainer = slide.querySelector(".meta .star-ranking");

      starContainer.innerHTML = "Star Ranking";
      let lineBreak = document.createElement("br");
      starContainer.appendChild(lineBreak);

      // Loop to add stars
      for (let i = 0; i < maxStars; i++) {
        let starLabel = document.createElement("label");
        starLabel.className = "star";
        if (i < review.ratingStars) {
          starLabel.classList.add("filled-star");
        } else {
          starLabel.classList.add("unfilled-star");
        }
        starContainer.appendChild(starLabel);
      }

      slide.querySelector(".meta .votes").textContent = review.totalScore;
      slide.querySelector(".description p").innerHTML = review.description;

      // Update vote button styles based on user rating
      const upvoteButton = slide.querySelector(".upvote-button");
      const downvoteButton = slide.querySelector(".downvote-button");

      if (review.userRating === 1) {
        upvoteButton.classList.add("active");
        downvoteButton.classList.remove("inactive");
      } else if (review.userRating === -1) {
        downvoteButton.classList.add("inactive");
        upvoteButton.classList.remove("active");
      } else {
        upvoteButton.classList.remove("active");
        downvoteButton.classList.remove("inactive");
      }

      // Add event listeners for vote buttons
      upvoteButton.addEventListener("click", function () {
        let userID =
          JSON.parse(localStorage.getItem("user")) ||
          JSON.parse(sessionStorage.getItem("user"));
        userID = userID.userID;
        if (userID == null || userID == 0) {
          showInvasivePopup(
            "x",
            "Log in",
            "Please log in first to submit ratings",
          );
          return;
        }
        if (userID == review.userID) {
          showInvasivePopup("x", "Log in", "You cannnot rate your own review.");
          return;
        }
        handleVoteButtonClick(review.ratingID, 1, upvoteButton, downvoteButton);
      });

      downvoteButton.addEventListener("click", function () {
        let userID =
          JSON.parse(localStorage.getItem("user")) ||
          JSON.parse(sessionStorage.getItem("user"));
        userID = userID.userID;
        if (userID == null || userID == 0) {
          showInvasivePopup(
            "x",
            "Log in",
            "Please log in first to submit ratings",
          );
          return;
        }
        if (userID == review.userID) {
          showInvasivePopup("x", "Log in", "You cannnot rate your own review.");
          return;
        }
        handleVoteButtonClick(
          review.ratingID,
          -1,
          upvoteButton,
          downvoteButton,
        );
      });
    } else {
    }
  });
}

function handleVoteButtonClick(reviewID, score, upvoteButton, downvoteButton) {
  // Determine current rating state
  let currentRating = 0; // Default no rating

  if (upvoteButton.classList.contains("active")) {
    currentRating = 1;
  } else if (downvoteButton.classList.contains("inactive")) {
    currentRating = -1;
  }

  // Toggle the rating
  if (score === currentRating) {
    // User clicks the same button again (unclick action)
    score = 0;
  }

  // Update button styles based on new rating
  if (score === 1) {
    upvoteButton.classList.add("active");
    downvoteButton.classList.remove("inactive");
  } else if (score === -1) {
    downvoteButton.classList.add("inactive");
    upvoteButton.classList.remove("active");
  } else {
    upvoteButton.classList.remove("active");
    downvoteButton.classList.remove("inactive");
  }

  submitUserRating(reviewID, score)
    .then((response) => {})
    .catch(() => {});
}

function submitUserRating(reviewID, score) {
  let api = `${BASE_URL}/Rating/RateReview`;
  let userID =
    JSON.parse(localStorage.getItem("user")) ||
    JSON.parse(sessionStorage.getItem("user"));
  userID = userID.userID;

  return ajaxCallPromise(
    "POST",
    api,
    JSON.stringify({
      userID: userID,
      ratingID: reviewID,
      score: score,
    }),
  )
    .then(function (response) {
      showDiscretePopup("Rating", "Rating noted");
      // Handle response if needed
      return response;
    })
    .catch(() => {
      showInvasivePopup(
        "x",
        "Rating Error",
        "Error submitting review rating. Please refresh the page or submit a bug report.",
      );
    });
}

function GetTop10UserOwnedBooks() {
  let api = `${BASE_URL}/Book/GetTop10UserOwnedBooks`;
  return ajaxCallPromise("GET", api, "")
    .then((Top10Books) => {
      updateTop10Books(Top10Books);
    })
    .catch((error) => {
      showInvasivePopup(
        "x",
        "Loading Error",
        "Error loading favorite books. Please refresh the page or submit a bug report.",
      );
    });
}

// Function to update the slider with the top 10 books
function updateTop10Books(books) {
  const logoContainers = document.querySelectorAll(
    "#BookTopPicksBookBox .logo_items",
  );

  // Clear any existing content
  logoContainers.forEach((container) => (container.innerHTML = ""));

  // Create a fragment to hold the images
  const fragment = document.createDocumentFragment();

  books.forEach((book) => {
    const imgElement = document.createElement("img");
    imgElement.src = book.thumbnail;
    imgElement.alt = book.title;
    imgElement.setAttribute("data-book-id", book.bookID); // Add data-book-id attribute
    imgElement.classList.add("book-image"); // Add a class for easier targeting

    // Append image to fragment
    fragment.appendChild(imgElement);
  });

  // Add images to the first logo_items container
  logoContainers[0].appendChild(fragment.cloneNode(true));

  // Duplicate the content in the second logo_items container
  logoContainers[1].appendChild(fragment);

  // Add event listeners to all book images
  document
    .querySelectorAll("#BookTopPicksBookBox .book-image")
    .forEach((item) => {
      item.addEventListener("click", function () {
        const bookID = this.getAttribute("data-book-id");
        // console.log("Book ID:", bookID); // Debug: Check if bookID is fetched correctly
        localStorage.setItem("bookID", bookID); // Stores the bookID in local storage
        window.location.href = "bookPage.html"; // Navigate to the book page
      });
    });
}
