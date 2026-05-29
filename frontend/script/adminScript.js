import { showDiscretePopup, showInvasivePopup } from "./generalScript.js";
import { userLoggedIn } from "./userUtilities.js";
import { ajaxCall, ajaxCallPromise } from "./ajaxCalls.js";
import { BASE_URL } from "./config.js";

var userActive = userLoggedIn();

var selectedBookForAddition;

if (userActive.userID == 1) {
  document.getElementById("HolderOfAll").style.display = "flex";
} else {
  window.location.href = "index.html";
}

$(document).ready(function () {
  GetDataForUserTable();
  GetDataForBookTable();
  GetDataForWriterTable();
});

document.addEventListener("DOMContentLoaded", function () {
  const modal = document.getElementById("bookSelectionModal");
  const span = document.getElementsByClassName("close")[0];

  // Show modal with book options (no pagination)
  function showBookSelection(books) {
    const bookList = document.getElementById("bookList");
    bookList.innerHTML = ""; // Clear previous content

    // Loop through all books and add them to the modal
    books.forEach((book) => {
      const bookItem = document.createElement("div");
      bookItem.className = "book-item";
      const bookInfo = `
        <strong>${book.volumeInfo.title}</strong><br>
        Author: ${
          book.volumeInfo.authors
            ? book.volumeInfo.authors.join(", ")
            : "Unknown"
        }<br>
        <a href="${
          book.selfLink
        }" target="_blank">View more details (JSON)</a><br>
        <a href="#" class="select-book" data-selflink="${
          book.selfLink
        }">Select this book</a>
      `;
      bookItem.innerHTML = bookInfo;
      bookItem
        .querySelector(".select-book")
        .addEventListener("click", function (e) {
          e.preventDefault();
          selectBook(book.selfLink);
          modal.style.display = "none";
        });
      bookList.appendChild(bookItem);
    });

    modal.style.display = "block";
  }

  // Handle modal close
  span.onclick = function () {
    modal.style.display = "none";
  };

  window.onclick = function (event) {
    if (event.target == modal) {
      modal.style.display = "none";
    }
  };

  // Select a book to display its details in the gray box

  async function selectBook(selfLink) {
    try {
      const response = await fetch(selfLink);
      const book = await response.json();

      // Display the full JSON contents in the gray box
      document.getElementById("importedBookInfoDisplayer").innerHTML =
        `<pre>${JSON.stringify(book, null, 2)}</pre>`;

      // Store the selected book data for further processing
      document.getElementById("acceptNewBook").onclick = async function () {
        try {
          const {
            formattedBook,
            uniqueAuthorEntities,
            uniqueCategoryEntities,
          } = await processBookData(book);
          AddBookToDB(
            formattedBook,
            uniqueAuthorEntities,
            uniqueCategoryEntities,
          );
        } catch (error) {
          showInvasivePopup(
            "x",
            "Error",
            "Error processing book data: " + error,
          );
        }
      };
    } catch (error) {
      showInvasivePopup("x", "Error", "Error fetching book details: " + error);
    }
  }

  // Process and log the book, author, and category entities
  async function processBookData(book) {
    // Format book data
    const formattedBook = formatBookData(book);

    // Extract and format author entities, ensuring no duplicates
    const authorEntities = new Set();
    if (book.volumeInfo.authors && book.volumeInfo.authors.length > 0) {
      for (const author of book.volumeInfo.authors) {
        const authorData = await searchWikipedia(author);
        authorEntities.add(authorData);
      }
    }
    const uniqueAuthorEntities = Array.from(authorEntities);

    // Extract and format category entities, ensuring no duplicates
    const categoryEntities = new Set();
    if (book.volumeInfo.categories && book.volumeInfo.categories.length > 0) {
      book.volumeInfo.categories.forEach((category) => {
        const splitCategories = category.split(/[/,]/).map(formatCategoryData);
        splitCategories.forEach((formattedCategory) => {
          categoryEntities.add(formattedCategory);
        });
      });
    }
    const uniqueCategoryEntities = Array.from(categoryEntities);

    // Return the formatted data
    return { formattedBook, uniqueAuthorEntities, uniqueCategoryEntities };
  }

  // Helper function to format author data
  function formatAuthorData(author) {
    return author.trim(); // Trim any leading or trailing whitespace
  }

  // Helper function to format category data
  function formatCategoryData(category) {
    return category.trim().split(" ").map(capitalizeFirstLetter).join(" ");
  }

  // Helper function to capitalize the first letter of each word
  function capitalizeFirstLetter(word) {
    return word.charAt(0).toUpperCase() + word.slice(1).toLowerCase();
  }

  // Helper function to format category data
  function formatCategoryData(category) {
    return category.trim().split(" ").map(capitalizeFirstLetter).join(" ");
  }

  // Helper function to capitalize the first letter of each word
  function capitalizeFirstLetter(word) {
    return word.charAt(0).toUpperCase() + word.slice(1).toLowerCase();
  }

  // Clear the gray box when "Reject" is clicked

  document
    .getElementById("rejectNewBook")
    .addEventListener("click", function () {
      document.getElementById("importedBookInfoDisplayer").innerHTML =
        "No book selected";
    });

  // Search buttons

  document
    .getElementById("searchByISBN10")
    .addEventListener("click", function () {
      searchBooks("isbn10");
    });

  document
    .getElementById("searchByISBN13")
    .addEventListener("click", function () {
      searchBooks("isbn13");
    });

  document
    .getElementById("searchByName")
    .addEventListener("click", function () {
      searchBooks("name");
    });

  // Function to call Google Books API

  async function searchBooks(type) {
    const query = document.getElementById("bookImportInput").value.trim();
    if (!query) {
      showDiscretePopup("Error", "Please enter a search term.");
      return;
    }
    let apiUrl = "https://www.googleapis.com/books/v1/volumes?q=";

    if (type === "isbn10") {
      apiUrl += `isbn:${query}`;
    } else if (type === "isbn13") {
      apiUrl += `isbn:${query}`;
    } else if (type === "name") {
      apiUrl += query;
    }

    apiUrl += "&maxResults=10";

    try {
      const response = await fetch(apiUrl);
      const data = await response.json();
      if (data.totalItems > 0) {
        showBookSelection(data.items);
      } else {
        showDiscretePopup("Error", "No books found");
      }
    } catch (error) {
      showInvasivePopup(
        "x",
        "Error",
        "Error fetching book search data: " + error,
      );
    }
  }

  // Helper function to format book data according to your specified format

  function formatBookData(item) {
    const { isbn_10, isbn_13 } = parseIndustryIdentifiers(
      item.volumeInfo.industryIdentifiers,
    );
    const isEbook = item.saleInfo.isEbook || false;

    // Function to clean up description
    function cleanDescription(description) {
      if (!description) return "";
      // Remove HTML tags
      let cleanedDescription = description.replace(/<\/?[^>]+(>|$)/g, "");
      // Replace multiple quotes with a single quote
      cleanedDescription = cleanedDescription.replace(/"+/g, '"');
      return cleanedDescription;
    }

    return {
      title: item.volumeInfo.title || "",
      subtitle: item.volumeInfo.subtitle || "",
      description: cleanDescription(item.volumeInfo.description), // Use the cleanDescription function
      publisher: item.volumeInfo.publisher || "",
      publishedDate: item.volumeInfo.publishedDate
        ? new Date(item.volumeInfo.publishedDate).toISOString()
        : "",
      printType: item.volumeInfo.printType || "",
      pageCount: item.volumeInfo.pageCount || 0,
      previewLink: item.volumeInfo.previewLink || "",
      maturityRating: item.volumeInfo.maturityRating || "",
      language: item.volumeInfo.language || "",
      infoLink: item.volumeInfo.infoLink || "",
      ISBN_10: isbn_10,
      ISBN_13: isbn_13,
      smallThumbnail: item.volumeInfo.imageLinks
        ? item.volumeInfo.imageLinks.smallThumbnail
        : "",
      thumbnail: item.volumeInfo.imageLinks
        ? item.volumeInfo.imageLinks.thumbnail
        : "https://static.vecteezy.com/system/resources/thumbnails/022/014/063/small_2x/missing-picture-page-for-website-design-or-mobile-app-design-no-image-available-icon-vector.jpg",
      canonicalVolumeLink: item.volumeInfo.canonicalVolumeLink || "",
      selfLink: item.selfLink || "",
      isEbook: isEbook,
      downloadLink: extractDownloadLink(item.accessInfo) || "",
      price: prices[Math.floor(Math.random() * prices.length)],
    };
  }

  // Helper function to parse industry identifiers

  function parseIndustryIdentifiers(industryIdentifiers) {
    let isbn_10 = null;
    let isbn_13 = null;

    if (industryIdentifiers) {
      industryIdentifiers.forEach((identifier) => {
        if (identifier.type === "ISBN_10") {
          isbn_10 = identifier.identifier;
        } else if (identifier.type === "ISBN_13") {
          isbn_13 = identifier.identifier;
        }
      });
    }

    return { isbn_10, isbn_13 };
  }

  // Function to search Wikipedia for author details

  async function searchWikipedia(author) {
    const url = `https://en.wikipedia.org/api/rest_v1/page/summary/${encodeURIComponent(
      author,
    )}`;

    try {
      const response = await fetch(url);
      const data = await response.json();
      return {
        author,
        summary: data.extract || "",
        image: data.originalimage ? data.originalimage.source : "",
        link: data.content_urls ? data.content_urls.desktop.page : "",
      };
    } catch (error) {
      showInvasivePopup(
        "x",
        "Error",
        `Error fetching Wikipedia data for author: ${author}` + error,
      );
      return {
        author,
        summary: "",
        image: "",
        link: "",
      };
    }
  }

  // Helper function to extract download link for eBooks
  function extractDownloadLink(accessInfo) {
    if (accessInfo.epub && accessInfo.epub.isAvailable)
      return accessInfo.epub.downloadLink;
    if (accessInfo.pdf && accessInfo.pdf.isAvailable)
      return accessInfo.pdf.downloadLink;
    return "";
  }

  const prices = [
    9.99, 10.99, 15.5, 19.99, 24.99, 11.99, 20.5, 14.5, 13.5, 14.9, 25.99,
  ];
});

function GetDataForUserTable() {
  let api = `${BASE_URL}/BookStoreUser/GetUserDataForAdminTable?userID=${userActive.userID}`;
  ajaxCallPromise("GET", api, "")
    .then((data) => {
      // Initialize the DataTable with the data returned from the server
      $("#userTable").DataTable({
        data: data, // Use the data returned from the AJAX call
        columns: [
          { title: "User ID", data: "userID" },
          { title: "User Name", data: "userName" },
          { title: "User Email", data: "userEmail" },
          { title: "Books Owned By User", data: "uniqueBooks" },
        ],
      });
    })
    .catch(function (error) {
      showInvasivePopup("x", "Error", "Unable to pull User data. " + error);
    });
}

// Function to get and populate data for the book table
function GetDataForBookTable() {
  let api = `${BASE_URL}/Book/GetBookDataForAdminTable?userID=${userActive.userID}`;
  ajaxCallPromise("GET", api, "")
    .then((data) => {
      // Initialize the DataTable with the data returned from the server
      $("#bookTable").DataTable({
        data: data, // Use the data returned from the AJAX call
        columns: [
          { title: "Book ID", data: "bookID" },
          { title: "Book Title", data: "bookTitle" },
          { title: "Book Author", data: "bookAuthor" },
          {
            title: "How Many People Own This Book",
            data: "uniqueOwners",
          },
        ],
      });
    })
    .catch(function (error) {
      showInvasivePopup("x", "Error", "Unable to pull Book data. " + error);
    });
}

// Function to get and populate data for the writer table
function GetDataForWriterTable() {
  let api = `${BASE_URL}/Author/GetAuthorDataForAdminTable?userID=${userActive.userID}`;
  ajaxCallPromise("GET", api, "")
    .then((data) => {
      // Initialize the DataTable with the data returned from the server
      $("#writerTable").DataTable({
        data: data, // Use the data returned from the AJAX call
        columns: [
          { title: "Author ID", data: "authorID" },
          { title: "Author Name", data: "authorName" },
          {
            title: "Unique Appearances in User Libraries",
            data: "timesInLibraries",
          },
        ],
      });
    })
    .catch(function (error) {
      showInvasivePopup("x", "Error", "Unable to pull Author data. " + error);
    });
}

function AddBookToDB(
  formattedBook,
  uniqueAuthorEntities,
  uniqueCategoryEntities,
) {
  const isbn10 = formattedBook.ISBN_10 || "irrelevant";
  const isbn13 = formattedBook.ISBN_13 || "irrelevant";

  // API call to check for duplicates in the database
  let api = `${BASE_URL}/Book/CheckIfBookIsInDB`;

  ajaxCallPromise(
    "POST",
    api,
    JSON.stringify({
      userID: userActive.userID,
      title: formattedBook.title,
      isbn10: isbn10,
      isbn13: isbn13,
    }),
  )
    .then((isPresent) => {
      if (isPresent) {
        showInvasivePopup("x", "Alert", "Book already in DB!");
        return;
      }
      let api = `${BASE_URL}/Book/AddNewBookToDB`;
      return ajaxCallPromise(
        "POST",
        api,
        JSON.stringify({ ...formattedBook, userID: userActive.userID }),
      );
    })
    .then((newBookID) => {
      if (!newBookID) {
        showInvasivePopup("x", "Alert", "Could not upload book");
        return;
      }

      // Now handle authors and categories using a controlled loop approach
      handleAuthorsAndCategories(
        newBookID,
        uniqueAuthorEntities,
        uniqueCategoryEntities,
      );
    })
    .catch((error) => {
      showInvasivePopup("x", "Error", "Error during book addition - " + error);
    });
}

// Function to handle authors and categories in sequence
function handleAuthorsAndCategories(newBookID, authors, categories) {
  handleAuthors(newBookID, authors, () => {
    handleCategories(newBookID, categories, () => {
      showInvasivePopup(
        "v",
        "Success",
        "Book, authors, and categories successfully added to the database!",
      );
    });
  });
}

// Function to handle adding authors one by one
function handleAuthors(bookID, authors, callback) {
  let index = 0;

  function addNextAuthor() {
    if (index < authors.length) {
      let author = authors[index];
      let api = `${BASE_URL}/Author/InsertNewAuthorAndConnectToBook`;

      ajaxCall(
        "POST",
        api,
        JSON.stringify({
          userID: userActive.userID,
          authorName: author.author,
          authorSummary: author.summary,
          authorImage: author.image,
          authorLink: author.link,
          bookID: bookID,
        }),
        (result) => {
          if (!result) {
            showInvasivePopup(
              "x",
              "Error",
              `Failed to add author: ${author.author}`,
            );
          }
          index++;
          addNextAuthor(); // Proceed to the next author
        },
      );
    } else {
      callback(); // All authors processed
    }
  }

  addNextAuthor(); // Start processing authors
}

// Function to handle adding categories one by one
function handleCategories(bookID, categories, callback) {
  let index = 0;

  function addNextCategory() {
    if (index < categories.length) {
      let category = categories[index];
      let api = `${BASE_URL}/Book/InsertNewCategoryAndConnectToBook`;

      ajaxCall(
        "POST",
        api,
        JSON.stringify({
          userID: userActive.userID,
          categoryName: category,
          bookID: bookID,
        }),
        (result) => {
          if (!result) {
            showInvasivePopup(
              `x`,
              `error`,
              `Failed to add category: ${category}`,
            );
          }
          index++;
          addNextCategory(); // Proceed to the next category
        },
      );
    } else {
      callback(); // All categories processed
    }
  }

  addNextCategory(); // Start processing categories
}
