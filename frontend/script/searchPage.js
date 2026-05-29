import { showDiscretePopup, showInvasivePopup } from "./generalScript.js";
import { userLoggedIn } from "./userUtilities.js";
import { ajaxCallPromise } from "./ajaxCalls.js";
import { BASE_URL } from "./config.js";

var userActive = userLoggedIn();
var allAuthors = [];
var displayedAuthors = 0;
const authorsPerPage = 12;

var books = [];
const booksPerPage = 12; // Number of books to display per "page"

// Update searchPage.html script to handle the search type parameter
$(document).ready(function () {
  var query = getQueryParam("q");
  var searchType = getQueryParam("type");
  if (query) {
    // Perform search with the query and search type
    searchInCurrentPage(query, searchType);
  }
});

// Used when the search is from another page
function getQueryParam(param) {
  var queryString = window.location.search;
  var urlParams = new URLSearchParams(queryString);
  return urlParams.get(param);
}

// If already in the search page, call searchInCurrentPage function from the generalScript.js
function searchInCurrentPage(query, searchType) {
  // Implement search logic based on searchType
  //// console.log('Searching for:', query, 'with type:', searchType);
  performSearch(query, searchType); // Call performSearch to handle the search
}

function performSearch(query, searchType) {
  if (searchType === "books") {
    if (query.length > 200) {
      showInvasivePopup(
        "x",
        "Error",
        "Search query for books is too long ( 200 limit )",
      );
      return;
    }
    var api = `${BASE_URL}/Book/GetBooksByTitle`;
    ajaxCallPromise("GET", api, { query: query })
      .then(function (response) {
        books = response;
        $(".BookContainer").empty();

        $("#SearchBooksContainer").css("display", "flex");
        $("#SearchAuthorsContainer").css("display", "none");
        $("#SearchContentHeader").text(
          'Results for book titles that include "' + query + '"',
        );
        displayBooks(books);
      })
      .catch(function (error) {
        showInvasivePopup(
          "x",
          "Error",
          "Error performing search, please check your internet connection or submit a bug report. " +
            error,
        );
      });
  }
  if (searchType === "authors") {
    if (query.length > 100) {
      showInvasivePopup(
        "x",
        "Error",
        "Search query for books is too long ( 100 limit )",
      );
      return;
    }
    var api = `${BASE_URL}/Author/GetAuthorsByName`;
    ajaxCallPromise("GET", api, { query: query })
      .then(function (response) {
        // Handle the response, e.g., display search results
        allAuthors = response; // Update allAuthors with the API response
        displayedAuthors = 0; // Reset counter
        $("#AllWritersBoxContainer").empty();

        $("#SearchBooksContainer").css("display", "none");
        $("#SearchAuthorsContainer").css("display", "flex");
        // $("#SearchWordsContainer").css("display", "none");
        //displayAuthors(response);
        $("#SearchContentHeader").text(
          'Results for author names that include "' + query + '"',
        );
        displayAuthors();
      })
      .catch(function (error) {
        showInvasivePopup(
          "x",
          "Error",
          "Error performing search, please check your internet connection or submit a bug report. " +
            error,
        );
      });
  }
  if (searchType === "words") {
    if (query.length > 2000) {
      showInvasivePopup(
        "x",
        "Error",
        "Search query for words is too long ( 2000 limit )",
      );
      return;
    }
    var api = `${BASE_URL}/Book/GetBooksByWords`;
    ajaxCallPromise("GET", api, { query: query })
      .then(function (response) {
        books = response;
        $(".BookContainer").empty();

        $("#SearchBooksContainer").css("display", "flex");
        $("#SearchAuthorsContainer").css("display", "none");
        // $("#SearchWordsContainer").css("display", "none");
        $("#SearchContentHeader").text(
          'Results for books that include "' + query + '"',
        );
        displayBooks(books);
      })
      .catch(function (error) {
        showInvasivePopup(
          "x",
          "Error",
          "Error performing search, please check your internet connection or submit a bug report. " +
            error,
        );
      });
  }
}

function displayBooks(books) {
  var container = document.querySelector(".BookContainer");

  if (container) {
    // Append all books to the container
    books.forEach((book) => {
      container.innerHTML += createBookHTML(book);
    });

    // Attach event listeners after rendering
    document.querySelectorAll(".view-book-btn").forEach((item) => {
      item.addEventListener("click", function () {
        const bookID = this.getAttribute("data-book-id");
        localStorage.setItem("bookID", bookID); // Stores the bookID in local storage
        window.location.href = "bookPage.html"; // Navigate to the next page
      });
    });

    // Hide "Show More" button and show "Back to Start" button since all books are displayed
    $("#AllBooksLibraryBttn").hide();
    $("#ResetButtonContainer").show();
  } else {
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
  if (window.location.href.includes("searchPage.html")) {
    const container = document.getElementById("SearchBooksContainer");
    if (container) {
      container.scrollIntoView({ behavior: "smooth", block: "end" });
    } else {
    }
  }
}

$(document).on("click", ".wishlist-book-btn", function () {
  if (!window.location.href.includes("searchPage.html")) return;
  if (userActive.userID == 0) {
    showInvasivePopup(
      "x",
      "Error",
      "You need to login in order to wishlist books!",
    );
    return;
  }
  ToggleBookWishlist(this);
});

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
            showDiscretePopup("v", "Wishlist", "Book added to wishlist!"); // !! Need to add feedback for
            return;
          });
        });
      } else {
        // Exists
        bookID = parseInt(bookID); // Convert bookID to integer

        let api = `${BASE_URL}/BookStoreUser/ToggleBookUserToWishlist`;

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
    .catch((error) => {
      showInvasivePopup(
        "x",
        "Error",
        "Error toggling wishlist, check your interent connection or submit a bug report. " +
          error,
      );
    });
}

// Event listener for "Show More" button
$("#AllBooksLibraryBttn").on("click", function () {
  displayBooks(books); // Load more books when "Show More" is clicked
  scrollToContainer(); // Scroll to the SearchBooksContainer after loading more books
});

// Event listener for "Back to Start" button
$("#ResetButton").on("click", function () {
  $(".BookContainer").empty(); // Clear the container
  displayBooks(books); // Show the first set of books
  $("#AllBooksLibraryBttn").show(); // Ensure "Show More" button is visible
  const container = document.getElementById("SearchBooksContainer");
  if (container) {
    container.scrollIntoView({ behavior: "smooth", block: "start" });
  } else {
  }
});

function displayAuthors() {
  const container = $("#AllWritersBoxContainer");
  for (let i = 0; i < allAuthors.length; i++) {
    const author = allAuthors[i];
    const authorTile = `
      <div class="author-tile" data-author="${author.authorName}">
        <div class="author-image" style="background-image: url('${author.image}');"></div>
        <div class="author-name">${author.authorName}</div>
        <button class="view-author" data-authorid="${author.authorID}">View</button>
      </div>
    `;
    container.append(authorTile);
  }

  $("html, body").animate({ scrollTop: $(document).height() }, "slow");
}
$(document).on("click", ".view-author", function () {
  const authorID = $(this).data("authorid");
  sessionStorage.setItem("authorID", authorID);
  window.location.href = "writerPage.html";
});
$("#AllWritersShowMoreBttn").on("click", function () {
  displayAuthors();
});

export { searchInCurrentPage };
