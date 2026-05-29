import { showDiscretePopup, showInvasivePopup } from "./generalScript.js";
import { ajaxCall, ajaxCallPromise } from "./ajaxCalls.js";
import { BASE_URL } from "./config.js";

$(document).on("click", "#UserPersonalizeBttn", function () {
  window.location.href = "../main/userEditPage.html";
});

$(document).ready(function () {
  const viewUserID = sessionStorage.getItem("viewUserID");
  // console.log(viewUserID);
  const currentUser =
    localStorage.getItem("user") || sessionStorage.getItem("user");
  const currentUserID = JSON.parse(currentUser).userID;

  if (viewUserID == null || viewUserID === currentUserID) {
    // User is viewing their own page
    renderUserData(currentUser);
    renderOwnedBooks(currentUserID);
    renderReadBooks(currentUserID);
    renderTradedBooks(currentUserID);
    renderWishlistedBooks(currentUserID);
    fetchUserReviews(currentUserID, currentUser, currentUserID);
  } else {
    // User is viewing another user's page
    let api = `${BASE_URL}/BookStoreUser/getUserDataByID`;
    ajaxCallPromise("GET", api, { UserID: viewUserID })
      .then(function (user) {
        $("#UserPersonalizeBttn").css("display", "none");
        renderUserData(user);
        renderReadBooks(viewUserID);
        //renderUserReviews(viewUserID);
        fetchUserReviews(viewUserID, user, currentUserID);
        $("#OwnedBooksContainer").css("display", "none");
        $("#OwnedBooksChooser").css("display", "none");
        $("#TradedBooksContainer").css("display", "none");
        $("#TradedBooksChooser").css("display", "none");
        $("#WishlistBooksContainer").css("display", "none");
        $("#WishlistBooksChooser").css("display", "none");
      })
      .catch(function (error) {
        showDiscretePopup(
          "Error",
          "Unable to get user data. Please check your internet connection and try again, or submit a bug report. " +
            error,
        );
      });
  }
});

// Render user data //
function renderUserData(user) {
  // If the user is an array, extract the first element
  if (Array.isArray(user)) {
    user = user[0];
  }

  // Ensure we're working with an object
  if (typeof user === "string") {
    user = JSON.parse(user);
  }

  // Date formatting
  let tempDate = new Date(user.dateCreated);
  let options = { year: "numeric", month: "long", day: "numeric" };
  let formattedDate = tempDate.toDateString(undefined, options);

  // Set user data in the DOM
  $("#UserProfileNameHolder").text("Username: " + user.userName);
  $("#UserProfileJoinDateHolder").text("Date Joined: " + formattedDate);
  if (user.userSignature == "") {
    $("#UserSignature").text("Signature not available.");
  } else {
    $("#UserSignature").html(`Signature: <br>${user.userSignature}`);
  }

  // Set the user's profile picture
  var imgElement = $("<img>")
    .attr("alt", user.userName)
    .addClass("profilepic")
    .on("error", function () {
      // Fallback image URL
      $(this).attr("src", "../media/user.png");
    });

  if (user.image === "Image link unavailable." || user.image === "") {
    imgElement.attr("src", "../media/user.png");
  } else {
    imgElement.attr("src", user.image);
  }

  // Append the image to the profile picture frame
  $("#UserProfilePicFrame").append(imgElement);
}

// Render owned books //
function renderOwnedBooks(userID) {
  var container = document.querySelector(".OwnedBooksContainer"); // Use the existing container
  let api = `${BASE_URL}/BookStoreUser/GetUserOwnedBooksAhHoc`;
  return ajaxCallPromise("GET", api, { UserID: userID })
    .then(function (books) {
      container.innerHTML = books.map(createBookHTMLWithRead).join("");

      document.querySelectorAll(".view-book-btn").forEach((item) => {
        item.addEventListener("click", function () {
          const bookID = this.getAttribute("data-book-id");
          localStorage.setItem("bookID", bookID); // Stores the bookID in local storage
          window.location.href = "bookPage.html"; // Navigate to the next page
        });
      });

      document.querySelectorAll(".read-book-checkbox").forEach((item) => {
        item.addEventListener("change", function () {
          const bookID = this.getAttribute("data-book-id");
          showDiscretePopup("Read", "Book toggled read");
          markBookAsRead(userID, bookID, this.checked);
          renderReadBooks(userID);
        });
      });
    })
    .catch(function (error) {
      showInvasivePopup(
        "x",
        "Error",
        "Unable to pull user's books. Check your internet connection or subit a bug report. " +
          error,
      );
    });
}

function markBookAsRead(currentUserID, BookID, isRead) {
  let api = `${BASE_URL}/BookStoreUser/MarkBookAsRead`;
  // Toggle book as read or not
  ajaxCall(
    "PUT",
    api,
    JSON.stringify({
      UserID: currentUserID,
      BookID: parseInt(BookID),
      IsRead: isRead,
    }),
    markBookAsReadSCB,
    markBookAsReadECB,
  );
  function markBookAsReadSCB() {
    showDiscretePopup("Read", "Book marked as read");
  }
  function markBookAsReadECB() {
    showDiscretePopup(
      "Error",
      "Cannot mark book as read or unread. Check your interent connection and try again.",
    );
  }
}

// Render books read //
function renderReadBooks(userID) {
  var container = document.querySelector(".ReadBooksContainer"); // Use the existing container
  let api = `${BASE_URL}/BookStoreUser/GetUserReadBooksAhHoc`; // API to get all read books by user

  return ajaxCallPromise("GET", api, { UserID: userID })
    .then(function (books) {
      container.innerHTML = books.map(createBookHTML).join("");

      // Attach event listeners after rendering
      document.querySelectorAll(".view-book-btn").forEach((item) => {
        item.addEventListener("click", function () {
          const bookID = this.getAttribute("data-book-id");
          localStorage.setItem("bookID", bookID); // Stores the bookID in local storage
          window.location.href = "bookPage.html"; // Navigate to the next page
        });
      });
    })
    .catch(function (error) {
      {
        showInvasivePopup(
          "x",
          "Error",
          "Unable to pull user's read books. Check your internet connection or subit a bug report. " +
            error,
        );
      }
    });
}

// Render books traded //
function renderTradedBooks(userID) {
  var container = document.querySelector(".TradedBooksContainer"); // Use the existing container
  let api = `${BASE_URL}/BookStoreUser/GetUserTradedBooksAdHoc`; // API to get traded books
  return ajaxCallPromise("GET", api, { UserID: userID })
    .then(function (books) {
      container.innerHTML = books.map(createBookHTML).join("");

      // Attach event listeners after rendering
      document.querySelectorAll(".view-book-btn").forEach((item) => {
        item.addEventListener("click", function () {
          const bookID = this.getAttribute("data-book-id");
          localStorage.setItem("bookID", bookID); // Stores the bookID in local storage
          window.location.href = "bookPage.html"; // Navigate to the next page
        });
      });
    })
    .catch(function (error) {
      {
        showInvasivePopup(
          "x",
          "Error",
          "Unable to pull user's traded books. Check your internet connection or subit a bug report. " +
            error,
        );
      }
    });
}

// Render wishlisted books
function renderWishlistedBooks(userID) {
  var container = document.querySelector(".WishlistedBooksContainer"); // Use the existing container
  let api = `${BASE_URL}/BookStoreUser/GetUserWishlistedBooksAdHoc`; // API to get wishlisted books
  return ajaxCallPromise("GET", api, { UserID: userID })
    .then(function (books) {
      container.innerHTML = books.map(createBookHTML).join("");

      // Attach event listeners after rendering
      document.querySelectorAll(".view-book-btn").forEach((item) => {
        item.addEventListener("click", function () {
          const bookID = this.getAttribute("data-book-id");
          localStorage.setItem("bookID", bookID); // Stores the bookID in local storage
          window.location.href = "bookPage.html"; // Navigate to the next page
        });
      });
    })
    .catch(function (error) {
      {
        showInvasivePopup(
          "x",
          "Error",
          "Unable to pull user's wishlisted books. Check your internet connection or subit a bug report. " +
            error,
        );
      }
    });
}

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
                          <button class="view-book-btn" data-book-id="${book.bookID}">View Book</button>
                      </div>
                  </div>
              </li>
              `;
}

function createBookHTMLWithRead(book) {
  return `
      <li class="Book3DContainer">
              <div class="Book" onclick="return true">
                  <div class="BookPic" style="background-image: url('${
                    book.thumbnail
                  }');"></div>
                  <div class="BookInfo">
                      <header>
                          <h1>${book.title}</h1>
                          <span class="year">${book.authors}</span>
                      </header>
                      <p>${book.description}</p>
                      <div class="read-book-div">
                          <input type="checkbox" id="read-${
                            book.bookID
                          }" class="read-book-checkbox" data-book-id="${
                            book.bookID
                          }" ${book.readStatus ? "checked" : ""}>
                          <label for="read-${
                            book.bookID
                          }">Have you read this book?</label>
                      </div>
                      <button class="view-book-btn" data-book-id="${
                        book.bookID
                      }">View Book</button>
                  </div>
              </div>
          </li>
          `;
}

function handleChooserClick(chooserId, containerId) {
  let $chooser = $(`#${chooserId}`);

  if ($chooser.hasClass("selectedBtn")) {
    // If the chooser is already selected, remove 'selectedBtn' and show all containers
    $chooser.removeClass("selectedBtn");
    $(".container").removeClass("hiddenDiv");
  } else {
    $(".chooser").removeClass("selectedBtn");
    $chooser.addClass("selectedBtn");

    $(".container").addClass("hiddenDiv");
    $(`#${containerId}`).removeClass("hiddenDiv");
  }

  if ($(".chooser.selectedBtn").length === 0) {
    // If no chooser has the 'selectedBtn' class, show all containers
    $(".container").removeClass("hiddenDiv");
  }
}

// Add click event to all chooser divs
$(".chooser").click(function () {
  const chooserId = $(this).attr("id");
  const containerId = chooserId.replace("Chooser", "Container");
  handleChooserClick(chooserId, containerId);
});

// Function to fetch all reviews by the user
async function fetchAllUserReviews(userID) {
  const api = `${BASE_URL}/Rating/GetUsersAllReviews`;
  const data = { userID: userID }; // Data to send with the request
  try {
    const reviews = await ajaxCallPromise("GET", api, data);
    return reviews;
  } catch (error) {
    throw new Error("Failed to fetch user reviews");
  }
}

// Update fetchBookDetailsAndReviews to use fetchAllUserReviews
async function fetchUserReviews(userID, user, viewerUserID) {
  if (!userID) {
    return;
  }

  try {
    const reviews = await fetchAllUserReviews(userID);

    // Clear previous reviews and add new ones
    const ulElement = document.querySelector(".splide__list");
    if (ulElement) {
      ulElement.innerHTML = "";
    } else {
      return;
    }
    if (reviews.length === 0) {
      //$("#NoReviewsMessage").css("display", "flex");
      $("#NoReviewsMessage")
        .addClass("no-reviews")
        .text("No reviews found for this user.");
      $("#NoReviewsMessage").css("display", "flex");
      $("#UserReviewsBoxContainer").addClass("no-reviews-box");
      $(".splide").css("display", "none");
      return;
    }

    reviews.forEach((review) => {
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
        <div class="footer"></div>
      `;

      liElement.appendChild(reviewCard); // Append review card to <li> element
      ulElement.appendChild(liElement); // Append <li> element to <ul> element

      updateSlideContent(liElement, review, user, viewerUserID); // Update slide content and functionality
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
    }
  } catch (error) {
    showInvasivePopup(
      "x",
      "Error",
      "Error fetching user reviews, please check your internet connection or submit a bug report. " +
        error,
    );
  }
}

function updateSlideContent(slide, review, user, viewerUserID) {
  let tempUser = typeof user === "string" ? JSON.parse(user) : user;
  if (Array.isArray(tempUser)) tempUser = tempUser[0];

  const formattedDate = new Date(review.dateRated).toLocaleDateString(
    undefined,
    { year: "numeric", month: "long", day: "numeric" },
  );
  slide.querySelector(".header").textContent = `"${review.header}"`;
  slide.querySelector(".book-name").textContent = review.bookName;
  slide.querySelector(".date").textContent = formattedDate;
  slide.querySelector(".name").innerHTML = "By: <br>" + tempUser.userName;

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
    starLabel.classList.toggle("filled-star", i < review.rating);
    starLabel.classList.toggle("unfilled-star", i >= review.rating);
    starsContainer.appendChild(starLabel);
  }
  starContainer.appendChild(starsContainer);

  const votesEl = slide.querySelector(".votes");
  votesEl.textContent = `${review.totalScore}`;
  slide.querySelector(".description p").innerHTML = review.description;

  // Vote buttons
  const footer = slide.querySelector(".footer");
  if (footer && viewerUserID && viewerUserID !== review.userID) {
    footer.innerHTML = `
      <div class="useful">Was this review helpful?</div>
      <div class="buttons">
        <button class="vote-button upvote-button" data-rating-id="${review.ratingID}">+1</button>
        <button class="vote-button downvote-button" data-rating-id="${review.ratingID}">-1</button>
      </div>`;

    const upvoteBtn = footer.querySelector(".upvote-button");
    const downvoteBtn = footer.querySelector(".downvote-button");

    upvoteBtn.addEventListener("click", function () {
      voteOnReview(review.ratingID, 1, review.userID, viewerUserID, votesEl, upvoteBtn, downvoteBtn);
    });
    downvoteBtn.addEventListener("click", function () {
      voteOnReview(review.ratingID, -1, review.userID, viewerUserID, votesEl, upvoteBtn, downvoteBtn);
    });
  } else if (footer) {
    footer.innerHTML = "";
  }
}

function voteOnReview(ratingID, score, reviewOwnerID, viewerUserID, votesEl, upvoteBtn, downvoteBtn) {
  if (!viewerUserID) {
    showInvasivePopup("x", "Alert", "Please log in to rate reviews.");
    return;
  }
  if (viewerUserID === reviewOwnerID) {
    showInvasivePopup("x", "Alert", "You cannot rate your own review.");
    return;
  }

  // If clicking the already-active button, treat it as a removal (score 0)
  const currentScore = upvoteBtn.classList.contains("active") ? 1
    : downvoteBtn.classList.contains("inactive") ? -1 : 0;
  const newScore = currentScore === score ? 0 : score;

  const api = `${BASE_URL}/Rating/RateReview`;
  ajaxCallPromise(
    "POST",
    api,
    JSON.stringify({ userID: viewerUserID, ratingID, score: newScore }),
  )
    .then((result) => {
      if (result !== null && result !== undefined) {
        votesEl.textContent = `${result}`;
        if (newScore === 0) {
          upvoteBtn.classList.remove("active");
          downvoteBtn.classList.remove("inactive");
        } else if (newScore === 1) {
          upvoteBtn.classList.add("active");
          downvoteBtn.classList.remove("inactive");
        } else {
          upvoteBtn.classList.remove("active");
          downvoteBtn.classList.add("inactive");
        }
      } else {
        showInvasivePopup("x", "Alert", "Failed to submit rating.");
      }
    })
    .catch(() => {
      showInvasivePopup("x", "Alert", "An error occurred while processing your rating.");
    });
}

let userData = sessionStorage.getItem("viewUserData");

try {
  userData = JSON.parse(userData); // Only use this if `viewUserData` was stored as JSON
} catch (e) {}
const tradedBooksToTradeBttn = document.querySelector(
  "#TradedBooksToTradeBttn",
);
tradedBooksToTradeBttn.addEventListener("click", function () {
  window.location.href = "TradingPage.html";
});
const WishlistBooksToTradeBttn = document.querySelector(
  "#WishlistBooksToTradeBttn",
);
WishlistBooksToTradeBttn.addEventListener("click", function () {
  window.location.href = "TradingPage.html";
});
