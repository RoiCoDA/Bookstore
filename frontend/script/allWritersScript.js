import { showInvasivePopup } from "./generalScript.js";
import { ajaxCallPromise } from "./ajaxCalls.js";
import { BASE_URL } from "./config.js";

var allAuthors = [];
//var displayedAuthors = 0;
//const authorsPerPage = 12;

$(document).ready(function () {
  getAndDisplayAuthors();

  $("#SearchForAWriterInput").on("keyup", function () {
    const query = $(this).val();
    if (query) {
      filterAuthors(query);
    } else {
      // If the input is empty, reset the display
      $("#AllWritersBoxContainer").empty();
      displayAuthors();
    }
  });

  $(document).on("click", ".view-author", function () {
    const authorID = $(this).data("authorid");
    sessionStorage.setItem("authorID", authorID);
    window.location.href = "writerPage.html";
  });
});

function getAndDisplayAuthors() {
  let api = `${BASE_URL}/Author/GetAllWritersDisplayAdHoc`;
  ajaxCallPromise("GET", api, "")
    .then((data) => {
      allAuthors = data;
      $("#AllWritersBoxContainer").empty();
      displayAuthors();
    })
    .catch((error) => {
      showInvasivePopup(
        "x",
        "Error",
        "Error fetching author data. Please check your internet connection or submit a bug report. " +
          error,
      );
    });
}

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

function filterAuthors(query) {
  const lowerCaseQuery = query.toLowerCase();
  $(".author-tile").each(function () {
    const authorName = $(this).data("author").toLowerCase();
    if (authorName.includes(lowerCaseQuery)) {
      $(this).show();
    } else {
      $(this).hide();
    }
  });
}
