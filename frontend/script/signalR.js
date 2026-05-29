import {
  renderFlashSaleBooks,
  CheckForNotifications,
  showDiscretePopup,
} from "./generalScript.js";
import { userLoggedIn } from "./userUtilities.js";
import { ajaxCallPromise } from "./ajaxCalls.js";
import { BASE_URL } from "./config.js";

var userActive = userLoggedIn();

let connection;
let saleStarted = false; // Flag to track if SaleStart has already been received

function initializeSignalRConnection() {
  if (connection && connection.state === "Connected") {
    return;
  }

  connection = new signalR.HubConnectionBuilder()
    .withUrl("https://proj.ruppin.ac.il/cgroup79/test2/tar1/notifications") // Server URL
    .withAutomaticReconnect() // Enable automatic reconnection
    .configureLogging(signalR.LogLevel.Debug) // Enable detailed logging
    .build();

  // Setup event handlers before starting the connection
  setupSignalREventHandlers();

  connection
    .start()
    .then(() => {
      // console.log("SignalR connection established.");
    })
    .catch((err) => console.error("SignalR connection error:", err));
}

function setupSignalREventHandlers() {
  // Handle incoming chat messages
  connection.on("ReceiveMessage", (receivedData) => {
    if (receivedData.event === "ChatMessage") {
      if (window.location.href.includes("writerPage.html")) {
        let authorID = sessionStorage.getItem("authorID");
        if (parseInt(receivedData.data.authorID) == parseInt(authorID)) {
          PullChatMessage(authorID, receivedData.data.message);
        }
      }
      return;
    }

    // Handle notifications
    if (receivedData.event === "Notification") {
      var userToNotify = receivedData.data.userID;
      if (userToNotify == userActive.userID) {
        CheckForNotifications();
      }
      return;
    }

    // Handle flash sale start
    if (receivedData.event === "SaleStart") {
      if (!saleStarted) {
        showDiscretePopup("Flash Sale", "Flash Sale has begun!");

        if (window.location.href.includes("index.html")) {
          $("#BookFlashSaleContainer").css("display", "block");
          renderFlashSaleBooks(receivedData.data.books);
        }
        saleStarted = true;
      }
      return;
    }

    // Handle flash sale end
    if (receivedData.event === "SaleEndMessage") {
      saleStarted = false; // Reset the flag when the sale ends

      if (window.location.href.includes("index.html")) {
        $("#BookFlashSaleContainer").css("display", "none");
      }
      alert("Flash book sale has ended!");
      return;
    }
  });

  // Reconnect on close
  connection.onclose((error) => {
    // console.error("SignalR connection closed:", error);
    // console.log("Attempting to reconnect in 1 second...");
    setTimeout(initializeSignalRConnection, 1000);
  });
}

// Initialize SignalR connection when the page loads
window.onload = function () {
  initializeSignalRConnection();
};

// Function to pull chat messages
function PullChatMessage(authorID, messageID) {
  let api = `${BASE_URL}/Author/GetChatMessageByChatMessagIDAndAuthorID?AuthorID=${authorID}&MessageID=${messageID}`;
  return ajaxCallPromise("GET", api, "").then((chatMessage) => {
    if (chatMessage == null) alert("error receiving message");
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
    const formattedDate = date.toLocaleDateString();
    const formattedTime = date.toLocaleTimeString();

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

function scrollToBottom() {
  const testContainer = $("#BottomBarChatExpanded");

  if (testContainer.length) {
    testContainer.scrollTop(testContainer[0].scrollHeight);
  }
}
