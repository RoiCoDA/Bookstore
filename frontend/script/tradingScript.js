import { showDiscretePopup, showInvasivePopup } from "./generalScript.js";
import { userLoggedIn } from "./userUtilities.js";
import { ajaxCallPromise } from "./ajaxCalls.js";
import { BASE_URL } from "./config.js";

var userActive = userLoggedIn();

document.addEventListener("DOMContentLoaded", function () {
  DisplayAllTradableBooks();
});

// Searching for a book //

document
  .getElementById("SearchForATradableBookInput")
  .addEventListener("keyup", function () {
    const searchQuery = this.value.trim().toLowerCase();
    filterBooksByName(searchQuery);
  });

function filterBooksByName(query) {
  const bookOffers = document.querySelectorAll(".book-offer");

  bookOffers.forEach((offer) => {
    const title = offer
      .querySelector(".book-name h2")
      .textContent.toLowerCase();
    if (title.includes(query)) {
      offer.style.display = "flex"; // Show the book offer if it matches the query
    } else {
      offer.style.display = "none"; // Hide the book offer if it doesn't match the query
    }
  });
}

///////////////////////////////////////////////////////////////////

// Displaying all tradable books //

function DisplayAllTradableBooks() {
  let api = `${BASE_URL}/BookStoreUser/GetAllTradableBooks`;
  return ajaxCallPromise("GET", api, { UserID: userActive.userID })
    .then(function (books) {
      var container = document.querySelector("#OfferedBooksDisplay");
      if (container) {
        container.innerHTML = books.map(createTradableBookHTML).join("");

        // Attach event listener using event delegation
        container.addEventListener("click", function (event) {
          if (event.target.classList.contains("trade-button")) {
            const bookID = event.target.getAttribute("data-book-id");
            const userID = event.target.getAttribute("data-user-id");
            MakeTradeOffer(bookID, parseInt(userID));
          }
        });
      } else {
      }
    })
    .catch(function (error) {
      // Handle any errors that occur during the API call
      showDiscretePopup(
        "Error",
        "Error fetching tradable books. Please check your internet connection.",
      );
    });
}

function createTradableBookHTML(book) {
  return `
      <div class="book-offer">
          <div class="book-name">
              <h2>${book.title}</h2>
          </div>
          <div class="user-name">
              <p>Offered by: ${book.userName}</p>
          </div>
          <button class="trade-button" data-book-id="${book.bookID}" data-user-id="${book.userID}">Trade offer</button>
      </div>
    `;
}

//////////////////////////////////////////////////////////////////

function MakeTradeOffer(bookID, OtherUserID) {
  let thisUserID = userActive.userID;
  bookID = parseInt(bookID);

  // Step 1: Check if the user already owns the book
  let api = `${BASE_URL}/BookStoreUser/CheckUserConnectionToBookForTrade?userID=${thisUserID}&BookID=${bookID}`;
  return ajaxCallPromise("GET", api, "")
    .then(function (DoesUserOwnBook) {
      if (DoesUserOwnBook == 1) {
        showInvasivePopup(
          "x",
          "Error",
          "You cannot ask to trade for a book you already own!",
        );
        return Promise.reject("User already owns the book"); // Stop further execution by rejecting the promise
      }

      // Step 2: Check for duplicate trade requests
      let api = `${BASE_URL}/BookStoreUser/DenyDuplicateTradeRequests?BookID=${bookID}&UserIDInitiator=${thisUserID}&UserIDRecipient=${OtherUserID}`;
      return ajaxCallPromise("GET", api, "");
    })
    .then(function (duplicateCheckResult) {
      if (duplicateCheckResult > 0) {
        showInvasivePopup(
          "x",
          "Error",
          "You have already submitted an offer for this book!",
        );
        return Promise.reject("Duplicate trade request"); // Stop further execution by rejecting the promise
      }

      let api = `${BASE_URL}/BookStoreUser/DoesBookUserConnectionExist`;
      return ajaxCallPromise("GET", api, {
        UserID: thisUserID,
        BookID: bookID,
      });
    })
    .then(function (DoesUserBookConnectionExist) {
      if (!DoesUserBookConnectionExist) {
        let api = `${BASE_URL}/BookStoreUser/AddNewUserBookRelation`;
        return ajaxCallPromise(
          "POST",
          api,
          JSON.stringify({ UserID: thisUserID, BookID: bookID }),
        );
      } else {
        return Promise.resolve(); // If the connection exists, move on to the next step
      }
    })
    .then(() => {
      let api = `${BASE_URL}/BookStoreUser/CreateBookTradesHistoryEntry`;
      return ajaxCallPromise(
        "POST",
        api,
        JSON.stringify({
          BookID: bookID,
          UserIDInitiator: thisUserID,
          UserIDRecipient: OtherUserID,
        }),
      );
    })
    .then((TradeEntry) => {
      let api = `${BASE_URL}/BookStoreUser/SendTradeNotificationToOfferingUser`;
      return ajaxCallPromise(
        "POST",
        api,
        JSON.stringify({ TradeID: TradeEntry, NotifRecipient: OtherUserID }),
      ).then(() => {
        showInvasivePopup("v", "Success", "Successfully asked to trade book!");
      });
    })
    .catch((error) => {
      showInvasivePopup(
        "x",
        "Error",
        "Error in making of a trade offer, check your internet connection or submit a bug report. " +
          error,
      );
    });
}
document
  .getElementById("OfferedBooksHeaderShowAllBttn")
  .addEventListener("click", function () {
    document.getElementById("SearchForATradableBookInput").value = "";
    filterBooksByName(""); // Optionally, call this to reset the book display
  });
