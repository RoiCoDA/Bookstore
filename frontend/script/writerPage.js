import { showDiscretePopup, showInvasivePopup } from "./generalScript.js";
import { userLoggedIn } from "./userUtilities.js";
import { ajaxCall, ajaxCallPromise } from "./ajaxCalls.js";
import { BASE_URL } from "./config.js";

let authorID = sessionStorage.getItem("authorID");

if (authorID == null) {
  window.location.href = "index.html";
}

sessionStorage.setItem("replyToPostID", 0);
var chatOffset = 0;
var authorData;
var userActive = userLoggedIn();

$(document).ready(function () {
  authorID = sessionStorage.getItem("authorID");

  GetAuthorAndBooks();

  Pull100ChatMessages().then((chatLog) => {
    DisplayChatMessages(chatLog);
  });

  GetAllForumPosts()
    .then((forumPosts) => {
      forumPosts.forEach((post) => {
        createForumPost(post);
      });
    })
    .catch((error) => {
      showInvasivePopup(
        "x",
        "Error",
        "Error fetching forum posts, please check your internet connection or submit a bug report. " +
          error,
      );
    });

  // Check if there's a stored reply post ID and show the reply notice
  const storedReplyPostID = sessionStorage.getItem("replyToPostID");
  if (storedReplyPostID) {
    showReplyNotice(storedReplyPostID);
  }
});

/* Replying to a users form post */

$(document).on("click", ".reply-button", function () {
  const postID = $(this).data("post-id");

  // Store the post ID in sessionStorage
  sessionStorage.setItem("replyToPostID", postID);

  // Scroll to the new post form
  $("html, body").animate(
    {
      scrollTop: $("#WriterForumNewPostBox").offset().top,
    },
    600,
  );

  // Remove any existing reply notice before showing the new one
  $("#ReplyNotice").remove();

  // Show the reply notice only if postID is not 0
  if (postID !== 0) {
    showReplyNotice(postID);
  }

  // Expand the new post form
  $("#WriterForumNewPostBox").removeClass("collapsed").addClass("expanded");
});

/* Lets user see he's replying to someone */

function showReplyNotice(postID) {
  if (postID == 0) {
    return;
  }

  const replyNotice = `
      <div id="ReplyNotice">
        Replying to post #${postID}
        <button id="CancelReplyButton">Cancel</button>
      </div>
  `;
  $("#NewPostForm").prepend(replyNotice);

  // Handle cancel reply
  $("#CancelReplyButton").on("click", function () {
    sessionStorage.removeItem("replyToPostID");
    $("#ReplyNotice").remove(); // Hide the reply notice on cancel
  });
}

// Chat functions //

function scrollToBottom() {
  const testContainer = $("#BottomBarChatExpanded");

  // Ensure the chat container is not empty before scrolling
  if (testContainer.length) {
    testContainer.scrollTop(testContainer[0].scrollHeight);
  }
}

$("#BottomBarChatSummonBttn").on("click", function () {
  $("#BottomBarBeforeChat").hide();
  $("#BottomBarChatExpanded").css("display", "flex");
  $("#ChatInputContainerContainer").css("display", "flex");
  $("#ChatRoomTitle").text(`${$("#WriterProfileNameHolder").text()} Chat Room`);
  scrollToBottom();
});

// Close chat
$("#CloseChatButton").on("click", function () {
  $("#BottomBarChatExpanded").hide();
  $("#ChatInputContainerContainer").hide();
  $("#BottomBarBeforeChat").show();
});

// Ensure the chat messages container remains scrollable
$("#ChatMessagesContainer").on("mouseenter", function () {
  $("body").css("overflow", "hidden"); // Disable body scrolling
});

$("#ChatMessagesContainer").on("mouseleave", function () {
  $("body").css("overflow", "auto"); // Re-enable body scrolling
});

// Send message
$("#ChatSendBttn").on("click", function () {
  sendMessage();
});

$("#ChatInput").on("keypress", function (e) {
  if (e.which === 13) {
    sendMessage();
  }
});

function sendMessage() {
  const message = $("#ChatInput").val().trim();
  if (userActive.userID == 0) {
    showInvasivePopup("x", "Login", "Log in before chatting");
    return;
  }
  if (message.length > 0) {
    let api = `${BASE_URL}/BookStoreUser/SendNewChatMessage`;
    return ajaxCallPromise(
      "POST",
      api,
      JSON.stringify({
        userID: userActive.userID,
        authorID: parseInt(authorID),
        MessageContent: message,
      }),
    )
      .then((messageMeta) => {
        if (!messageMeta) {
          showInvasivePopup(
            "x",
            "Error",
            "Message could not be sent, please check your internet connection and try again later",
          );
          return;
        }

        ////////////////////////////////////////////// HERE WE ARE GETTING BACK THE MESSAGE AND AUTHOR ID FOR SOCKET USE //////////////////////////////////////////////

        // Form the message object to be displayed
        const newMessage = {
          userID: userActive.userID,
          userName: userActive.userName, // Assuming you have the user's name in `userActive`
          messageContent: message,
          datePosted: new Date().toISOString(),
        };
      })
      .then(() => {
        // Update chat
        scrollToBottom();

        // Clear input
        $("#ChatInput").val("");
      });
  }
}

function Pull100ChatMessages() {
  let api = `${BASE_URL}/Author/Load100ChatMessages?AuthorID=${authorID}&Offset=${chatOffset}`;
  return ajaxCallPromise("GET", api, "").then((chats) => {
    // console.log(chats); // REMOVE BEFORE FLIGHT
    chatOffset += 100;
    return chats;
  });
}

// CALL FROM SOCKET // \/

function PullChatMessage(authorID, messageID) {
  let api = `${BASE_URL}/Author/GetChatMessageByChatMessagIDAndAuthorID?AuthorID=${authorID}&MessageID=${messageID}`;
  return ajaxCallPromise("GET", api, "").then((chatMessage) => {
    // console.log(chatMessage); // REMOVE BEFORE FLIGHT
    if (chatMessage == null)
      showInvasivePopup(
        "x",
        "Error",
        "Error receivng message, please check your internet connection and try again.",
      );
    else DisplayChatMessages(chatMessage);
  });
}

function DisplayChatMessages(chatLog) {
  const container = $("#ChatMessagesContainer");

  chatLog.forEach((message) => {
    // Create the message bubble
    const messageBubble = document.createElement("div");
    messageBubble.classList.add("chat-message");

    // Determine if the message is incoming or outgoing
    const messageClass =
      message.userID === userActive.userID
        ? "outgoing-message"
        : "incoming-message";
    messageBubble.classList.add(messageClass);

    // Create the user info and timestamp
    const date = new Date(message.datePosted);
    const formattedDate = date.toLocaleDateString(); // e.g., 8/14/2024
    const formattedTime = date.toLocaleTimeString(); // e.g., 11:34 AM

    const userInfo = document.createElement("div");
    userInfo.classList.add("user-info");
    userInfo.innerHTML = `<strong><a href="#" data-user-id="${message.userID}" class="chat-username">${message.userName}</a></strong> <span class="timestamp">(${formattedDate} - ${formattedTime})</span>`;

    // Create the message content
    const messageContent = document.createElement("div");
    messageContent.classList.add("message-content");
    messageContent.textContent = message.messageContent;

    // Append user info and message content to the message bubble
    messageBubble.appendChild(userInfo);
    messageBubble.appendChild(messageContent);

    // Append the message bubble to the chat container
    container.append(messageBubble);

    // Add event listener to handle user link click
    userInfo
      .querySelector(".chat-username")
      .addEventListener("click", function (e) {
        e.preventDefault();
        const userID = parseInt(this.dataset.userId);

        if (userID !== userActive.userID) {
          sessionStorage.setItem("viewUserID", userID);
        } else {
          sessionStorage.removeItem("viewUserID");
        }
        window.location.href = "userPage.html";
      });
  });

  // Scroll to the bottom after appending all messages
  scrollToBottom();
}

$("#LoadMoreMessages").on("click", function () {
  Pull100ChatMessages().then((chatLog) => {
    if (chatLog.length === 0) {
      // No new messages, reset offset to current message count
      chatOffset = $("#ChatMessagesContainer .chat-message").length;
      $(this).prop("disabled", true); // Disable the button if no more messages are available
    } else {
      // Load more messages and increase the offset
      DisplayChatMessages(chatLog);
      chatOffset += chatLog.length;
    }
  });
});

// End of chat functions

/* Gets author and book data from server */
function GetAuthorAndBooks() {
  let api = `${BASE_URL}/Author/authorPage`;
  ajaxCall(
    "GET",
    api,
    { id: authorID },
    GetAuthorAndBooksSCB,
    GetAuthorAndBooksECB,
  );
}

function GetAuthorAndBooksSCB(Author) {
  authorData = Author;
  DisplayAuthorData(Author);
}

function GetAuthorAndBooksECB(error) {
  showInvasivePopup(
    "x",
    "Error",
    "Unable to get author data. Check your interent connection and try again, or submit a bug report. " +
      error,
  );
}

function DisplayAuthorData(author) {
  $("#WriterForumPostsHeader").text(author[0].author + "'s Forum");

  $("#WriterProfileNameHolder").text("Author: " + author[0].author);

  if (author[0].image == "") {
    var imgElement = $("<img>")
      .attr(
        "src",
        "https://static.vecteezy.com/system/resources/thumbnails/022/014/063/small_2x/missing-picture-page-for-website-design-or-mobile-app-design-no-image-available-icon-vector.jpg",
      )
      .attr("alt", author.Author)
      .addClass("profilepic");
  } else {
    var imgElement = $("<img>")
      .attr("src", author[0].image)
      .attr("alt", author[0].author)
      .addClass("profilepic");
  }
  $("#WriterProfilePicFrame").append(imgElement);

  if (author[0].summary == "") {
    $("#WriterSummary").text("Summary not available.");
  } else {
    $("#WriterSummary").html(`Summary: <br>${author[0].summary}`);
  }

  const bookListContainer = $(".BookContainer");
  if (author[0].books && author[0].books.length > 0) {
    author[0].books.forEach((book) => {
      bookListContainer.append(createBookHTML(book));
    });
  } else {
    bookListContainer.append("<p>No books available for this author.</p>");
  }

  // Attach event listeners after rendering all books
  document.querySelectorAll(".view-book-btn").forEach((item) => {
    item.addEventListener("click", function () {
      const bookID = this.getAttribute("data-book-id");
      localStorage.setItem("bookID", bookID); // Stores the bookID in local storage
      window.location.href = "bookPage.html"; // Navigate to the next page
    });
  });
}

function createBookHTML(book) {
  const thumbnail =
    book.thumbnail ||
    "https://static.vecteezy.com/system/resources/thumbnails/022/014/063/small_2x/missing-picture-page-for-website-design-or-mobile-app-design-no-image-available-icon-vector.jpg";

  return `
    <li class="Book3DContainer">
            <div class="Book" onclick="return true">
                <div class="BookPic" style="background-image: url('${thumbnail}');"></div>
                <div class="BookInfo">
                    <header>
                        <h1>${book.title}</h1>
                        <span class="year">${
                          authorData[0].author || "Unknown Author"
                        }</span>
                    </header>
                    <p>${book.description || "No description available."}</p>
                    <button class="wishlist-book-btn" data-book-id="${
                      book.bookID
                    }">Wishlist Book</button>
                    <button class="view-book-btn" data-book-id="${
                      book.bookID
                    }">View Book</button>
                </div>
            </div>
        </li>
        `;
}

// Forum logic

const ForumCommentToolbarOptions = [
  ["bold", "italic", "underline"],
  ["link", "blockquote", "code-block"],
  [{ list: "ordered" }, { list: "bullet" }],
  [{ align: [] }],
  ["clean"],
];

// Check if the Quill editor element exists
const quillEditorElement = document.querySelector("#QuillForumEditor");

if (quillEditorElement) {
  // Initialize Quill only if the element is found
  const quill = new Quill("#QuillForumEditor", {
    theme: "snow",
    modules: {
      toolbar: ForumCommentToolbarOptions,
    },
  });

  const maxCharacters = 500;

  // Update character count as the user types
  quill.on("text-change", function () {
    const text = quill.getText().trim();
    const charCount = text.length;
    $("#CharacterCounter").text(`${charCount}/${maxCharacters} characters`);

    if (charCount > maxCharacters) {
      $("#CharacterCounter").addClass("limit-exceeded");
    } else {
      $("#CharacterCounter").removeClass("limit-exceeded");
    }
  });

  // Close the form and clear it
  $("#CloseFormButton").on("click", function () {
    $("#WriterForumNewPostBox").removeClass("expanded").addClass("collapsed");
    $("#PostHeaderInput").val("");
    quill.setText("");
    $("#CharacterCounter").text(`0/${maxCharacters} characters`);
    $("#CharacterCounter").removeClass("limit-exceeded");
  });

  // Handle post submission with character limit and line break validation
  $("#SubmitPostButton").on("click", function (e) {
    e.preventDefault();
    const header = $("#PostHeaderInput").val().trim();
    let content = quill.root.innerHTML.trim();
    const text = quill.getText().trim();
    const charCount = text.length;

    // Validate character limit
    if (charCount > maxCharacters) {
      showInvasivePopup(
        "x",
        "Error",
        "Your post exceeds the 500-character limit.",
      );
      return;
    }

    // Validate and limit consecutive line breaks to 3
    content = content.replace(/(<br\s*\/?>\s*){4,}/g, "<br><br><br>");

    // Get the responseToPostID from sessionStorage, default to 0
    const responseToPostID =
      parseInt(sessionStorage.getItem("replyToPostID")) || 0;

    if (header && content) {
      if (userActive.userID == 0) {
        showInvasivePopup(
          "x",
          "Error",
          "Please sign in to make a new forum post",
        );
        return;
      }

      let api = `${BASE_URL}/Author/SubmitForumPost`; // API for forum message submission
      return ajaxCallPromise(
        "POST",
        api,
        JSON.stringify({
          authorID: parseInt(authorID),
          userID: userActive.userID,
          header,
          content,
          responseToPostID: responseToPostID,
        }),
      )
        .then((status) => {
          if (status) {
            // If the post was submitted successfully
            if (responseToPostID !== 0) {
              // Send a notification if it's a reply to a post
              let api = `${BASE_URL}/BookStoreUser/SendForumMentionNotification`; // Correct endpoint
              return ajaxCallPromise(
                "POST",
                api,
                JSON.stringify({
                  authorID: parseInt(authorID),
                  userID: userActive.userID,
                  responseToPostID: responseToPostID,
                }),
              )
                .then((notificationStatus) => {
                  if (notificationStatus) {
                    showInvasivePopup(
                      "v",
                      "Success",
                      "Post submitted successfully!",
                    );
                    $("#CloseFormButton").click();
                    sessionStorage.removeItem("replyToPostID"); // Clear the stored post ID after submission
                    $("#ReplyNotice").remove(); // Remove the reply notice after submission
                    return;
                  } else {
                  }
                })
                .catch((error) => {});
            } else {
              showDiscretePopup("Success", "Post submitted successfully!");
              $("#CloseFormButton").click();
              sessionStorage.removeItem("replyToPostID"); // Clear the stored post ID after submission
              $("#ReplyNotice").remove(); // Remove the reply notice after submission
              $("#WriterForumPostsContainer .post-container").remove();
              GetAllForumPosts().then((forumPosts) => {
                forumPosts.forEach((post) => {
                  createForumPost(post);
                });
              });
              return;
            }
          } else {
            showInvasivePopup(
              "x",
              "Error",
              "Post submission failed. Please check your internet connection and try again.",
            );
          }
        })
        .catch((error) => {
          showInvasivePopup(
            "x",
            "Error",
            "Post submission failed. Please check your internet connection and try again, or submit a bug report. " +
              error,
          );
        });
    } else {
      showInvasivePopup(
        "x",
        "Error",
        "Please fill out both the header and the content.",
      );
    }
  });
}

// Toggle the form open/close
$("#WriterForumPostNewPostBttn").on("click", function () {
  const postBox = $("#WriterForumNewPostBox");
  if (postBox.hasClass("collapsed")) {
    postBox.removeClass("collapsed").addClass("expanded");
  } else {
    postBox.removeClass("expanded").addClass("collapsed");
  }
});

function GetAllForumPosts() {
  let api = `${BASE_URL}/Author/GetAllForumPosts?AuthorID=${parseInt(
    authorID,
  )}`; // API for forum message submission

  return ajaxCallPromise("GET", api, "");
}

// Printing of forum posts

function createForumPost(post) {
  // Create the main post container
  const postContainer = document.createElement("div");
  postContainer.classList.add("post-container");

  // Create the collapsed view
  const collapsedView = document.createElement("div");
  collapsedView.classList.add("collapsed-view");
  collapsedView.style.height = "100px";
  collapsedView.innerHTML = `
      <div class="post-header">
          <span class="post-number">#${post.postID}</span> ${post.header}
      </div>
      <div class="post-meta">
          <span class="post-date">${new Date(
            post.datePosted,
          ).toLocaleDateString()}</span> 
          by <a href="#" data-user-id="${post.userID}" class="post-username">${
            post.userName
          }</a>
          ${
            post.responseToPostID !== 0
              ? '<span class="reply-info">Replying to Post #' +
                post.responseToPostID +
                "</span>"
              : ""
          }
      </div>
  `;

  // Create the expanded view
  const expandedView = document.createElement("div");
  expandedView.classList.add("expanded-view");
  expandedView.style.display = "none"; // Initially hidden
  expandedView.innerHTML = `
    <div class="post-expanded-content">
        <img src="${post.userImage}" alt="${post.userName}" class="user-image">
        <div class="post-details">
            <a href="#" data-user-id="${post.userID}" class="post-username">${
              post.userName
            }</a>
            <div class="post-content">${post.postContent}</div>
            ${
              post.signature
                ? `<div class="signature-container">
                        <hr class="signature-divider">
                        <div class="post-signature">${post.signature}</div>
                  </div>`
                : ""
            }
            <button class="reply-button" data-post-id="${
              post.postID
            }">Reply to this post</button>
        </div>
    </div>
`;

  // Check if post-content is empty and only contains <p><br></p>
  const postContentElement = expandedView.querySelector(".post-content");
  if (postContentElement.innerHTML.trim() === "<p><br></p>") {
    const postHeaderElement = collapsedView.querySelector(".post-header");
    postHeaderElement.innerHTML +=
      " <span class='soft-writing'>(No message contents)</span>";
  }

  // Toggle expanded view on click of collapsed view
  collapsedView.addEventListener("click", function () {
    const isExpanded = expandedView.style.display === "block";
    expandedView.style.display = isExpanded ? "none" : "block";
  });

  // Append the views to the post container
  postContainer.appendChild(collapsedView);
  postContainer.appendChild(expandedView);

  // Append the post container to the forum posts container
  document
    .getElementById("WriterForumPostsContainer")
    .appendChild(postContainer);

  // Add event listener to handle user link click
  postContainer.querySelectorAll(".post-username").forEach((userLink) => {
    userLink.addEventListener("click", function (e) {
      e.preventDefault();
      const userID = parseInt(this.dataset.userId);

      if (userID !== userActive.userID) {
        sessionStorage.setItem("viewUserID", userID);
      } else {
        sessionStorage.removeItem("viewUserID");
      }
      window.location.href = "userPage.html";
    });
  });
}
