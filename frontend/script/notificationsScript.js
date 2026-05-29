import { showDiscretePopup, showInvasivePopup } from "./generalScript.js";
import { userLoggedIn } from "./userUtilities.js";
import { ajaxCallPromise } from "./ajaxCalls.js";
import { BASE_URL } from "./config.js";

var userActive = userLoggedIn();

if (userActive.userID == 0)
  window.location.href = "../main/loginSignupPage.html";

$(document).ready(function () {
  // Handles opening/closing and shape of notifications
  $(".notification-checkbox").on("click", function (e) {
    e.stopPropagation();
  });

  // Functionality for checking/unchecking all notifications
  $("#select-all-notifications").on("change", function () {
    let isChecked = $(this).prop("checked");
    $(".notification-toggle").prop("checked", isChecked);
  });

  $(".notification-title, .expand-toggle").on("click", function (e) {
    const notification = $(this).closest(".notification");

    notification.toggleClass("active");

    if (notification.hasClass("active")) {
      notification
        .find(".notification-content")
        .slideDown(300)
        .css("opacity", "1");
      notification.find(".expand-toggle").text("-");
    } else {
      notification
        .find(".notification-content")
        .slideUp(300)
        .css("opacity", "0");
      notification.find(".expand-toggle").text("+");
    }

    $("#NotificationsSetMarkedAsRead").on("click", function () {
      markSelectedNotificationsAsRead();
    });
  });
});

LoadAllUserNotifications();

function markSelectedNotificationsAsRead() {
  // Gather all selected notification IDs
  let selectedNotifications = [];
  $(".notification-toggle:checked").each(function () {
    let notificationDiv = $(this).closest(".notification");
    let notificationID = notificationDiv.data("notificationid"); // Accessing the NotificationID
    if (notificationID) {
      selectedNotifications.push(notificationID);
    }
  });

  if (selectedNotifications.length === 0) return;

  selectedNotifications.forEach((notificationID) => {
    let api = `${BASE_URL}/Notification/MarkNotificationAsRead?notificationID=${notificationID}`;
    ajaxCallPromise("PUT", api, "")
      .then(() => {
        $(`.notification[data-notificationid="${notificationID}"]`).removeClass(
          "unread",
        );
      })
      .catch((error) => {
        showDiscretePopup(
          "Error",
          `Error marking notification ${notificationID} as read, Please check your internet connection or submit a bug report` +
            error,
        );
      });
  });

  LoadAllUserNotifications();
}

function deleteSelectedNotifications() {
  let selectedNotifications = [];
  $(".notification-toggle:checked").each(function () {
    let notificationDiv = $(this).closest(".notification");
    let notificationID = notificationDiv.data("notificationid"); // Accessing the NotificationID
    let tradeID = $(this)
      .closest(".notification")
      .find(".notification-btn.accept-btn")
      .data("tradeid"); // Accessing the TradeID if present

    selectedNotifications.push({ notificationID, tradeID });
  });

  if (selectedNotifications.length === 0) return;

  selectedNotifications.forEach((notification) => {
    let { notificationID, tradeID } = notification;

    // If it's a trade notification with an unresolved trade
    if (tradeID) {
      // Reject the trade and then delete the notification
      handleRejectTrade(tradeID)
        .then(() => {
          HT;
          return deleteNotification(notificationID); // Delete the notification after rejecting the trade
        })
        .then(() => {
          $(`.notification[data-notificationid="${notificationID}"]`).remove(); // Remove from the UI
        })
        .catch((error) => {
          showDiscretePopup(
            "Error",
            `Error handling trade or deleting notification ${notificationID}, Please check your internet connection or submit a bug report. ` +
              error,
          );
        });
    } else {
      // If it's not a trade notification or the trade doesn't need rejection, just delete the notification
      deleteNotification(notificationID)
        .then(() => {
          showDiscretePopup(
            "Notification",
            `Notification ${notificationID} deleted.`,
          );
          $(`.notification[data-notificationid="${notificationID}"]`).remove(); // Remove from the UI
        })
        .catch((error) => {
          showDiscretePopup(
            "Error",
            `Error deleting notification ${notificationID}:` + error,
          );
        });
    }
  });
}

// Helper function to delete a notification by ID
function deleteNotification(notificationID) {
  let api = `${BASE_URL}/Notification/DeleteNotification?notificationID=${notificationID}`;
  return ajaxCallPromise("DELETE", api, "");
}

// Bind the "Mark as Read"/"Delete" buttons to the functions
$("#NotificationsSetMarkedAsRead").on("click", function () {
  markSelectedNotificationsAsRead();
});

$("#NotificationsDeleteMarked").on("click", function () {
  deleteSelectedNotifications();
});

function LoadAllUserNotifications() {
  let api = `${BASE_URL}/Notification/GetAllUserNotifications?userID=${userActive.userID}`;

  return ajaxCallPromise("GET", api, "")
    .then((Notifications) => {
      const notificationContainer = document.getElementById(
        "NotificationDisplayBox",
      );

      // Clear any existing content
      notificationContainer.innerHTML = "";

      if (Notifications.length === 0) {
        // No notifications case
        notificationContainer.innerHTML = `
          <div class="notification">
            <div class="notification-header">
              <span class="notification-title">No Notifications available.</span>
            </div>
          </div>`;
      } else {
        // Render each notification
        Notifications.forEach((notification) => {
          notificationContainer.innerHTML +=
            createNotificationHTML(notification);
        });
      }

      // Attach event listeners after rendering
      attachNotificationHeaderEventListeners();
      attachNotificationActionButtonsEventListeners();
    })
    .catch((error) => {
      showInvasivePopup(
        "x",
        "Error",
        "Error loading notifications, please check your internet connection or submit a bug report. " +
          error,
      );
    });
}

// Function to create HTML for each notification
function createNotificationHTML(notification) {
  let buttonsHTML = "";

  // Generate buttons based on notification type
  if (notification.notificationType === "trade offer incoming") {
    buttonsHTML = `
      <div class="notification-actions">
        <button class="notification-btn accept-btn" data-tradeid="${notification.tradeID}">Accept</button>
        <button class="notification-btn reject-btn" data-tradeid="${notification.tradeID}">Reject</button>
      </div>`;
  } else if (
    notification.notificationType === "trade accepted" ||
    notification.notificationType === "trade rejected"
  ) {
    buttonsHTML = `
      <div class="notification-actions">
        <button class="notification-btn go-library-btn">Go to Library</button>
      </div>`;
  } else if (notification.notificationType === "book bought") {
    buttonsHTML = `
      <div class="notification-actions">
        <button class="notification-btn go-library-btn">Go to Library</button>
      </div>`;
  }

  // Generate the HTML structure, with the NotificationID stored as a data attribute
  return `
    <div class="notification ${
      !notification.notificationRead ? "unread" : ""
    }" data-notificationid="${notification.notificationID}">
      <div class="notification-header">
        <input type="checkbox" class="notification-toggle" />
        <span class="notification-title">${mapNotificationTypeToHeader(
          notification.notificationType,
        )}</span>
        <span class="expand-toggle">+</span>
      </div>
      <div class="notification-content" style="display: none;">
        <p>${notification.description}</p>
        ${buttonsHTML}
      </div>
    </div>`;
}

// Function to attach event listeners for notification headers
function attachNotificationHeaderEventListeners() {
  document.querySelectorAll(".notification-header").forEach((header) => {
    header.addEventListener("click", function (e) {
      const notificationDiv = header.parentElement;
      const notificationContent = notificationDiv.querySelector(
        ".notification-content",
      );
      const expandToggle = header.querySelector(".expand-toggle");

      const isActive = notificationDiv.classList.toggle("active");

      if (isActive) {
        notificationContent.style.display = "block";
        expandToggle.textContent = "-";

        // If unread, mark as read
        if (notificationDiv.classList.contains("unread")) {
          let notificationID = notificationDiv.dataset.notificationid;
          markNotificationAsRead(notificationID);
          notificationDiv.classList.remove("unread");
        }
      } else {
        notificationContent.style.display = "none";
        expandToggle.textContent = "+";
      }
    });
  });
}

// Function to attach event listeners for notification action buttons
function attachNotificationActionButtonsEventListeners() {
  document.querySelectorAll(".accept-btn").forEach((button) => {
    button.addEventListener("click", function () {
      const tradeID = this.getAttribute("data-tradeid");
      handleAcceptTrade(tradeID);
    });
  });

  document.querySelectorAll(".reject-btn").forEach((button) => {
    button.addEventListener("click", function () {
      const tradeID = this.getAttribute("data-tradeid");
      handleRejectTrade(tradeID);
    });
  });

  document.querySelectorAll(".go-library-btn").forEach((button) => {
    button.addEventListener("click", function () {
      // Handle going to library
      window.location.href = "../main/userPage.html";
    });
  });
}

// Function to accept trade

function handleAcceptTrade(tradeID) {
  // First API call to get the BookID and UserID_TradeInitiator from the TradeID
  let api = `${BASE_URL}/BookStoreUser/GetBookIDAndInitiatorIDFromBookTradesHistoryEntry?TradeID=${tradeID}`;

  return ajaxCallPromise("GET", api, "")
    .then(function (response) {
      // Assuming the API returns an object with BookID and UserID_TradeInitiator
      let bookID = response.bookID;
      let initiatorID = response.initiatorID;

      // API call to check if the user owns the book involved in the trade
      let api = `${BASE_URL}/BookStoreUser/CheckUserConnectionToBookForTrade?userID=${initiatorID}&BookID=${bookID}`;

      return ajaxCallPromise("GET", api, "");
    })
    .then(function (DoesUserOwnBook) {
      if (DoesUserOwnBook == 1) {
        showInvasivePopup(
          "x",
          "Error",
          "You cannot accept a trade for a book you already own!",
        );

        // Reject the trade automatically since the user already owns the book
        handleRejectTrade(tradeID);

        return Promise.reject(
          "Trade automatically rejected because user already owns the book.",
        );
      }

      // Proceed with accepting the trade
      let api = `${BASE_URL}/BookStoreUser/TradeOfferAccepted?TradeID=${tradeID}`;
      return ajaxCallPromise("PUT", api, "");
    })
    .then((status) => {
      if (status) {
        showDiscretePopup("v", "Success", "Trade accepted successfully");
        let api = `${BASE_URL}/BookStoreUser/TradeConclusionNotification?TradeID=${tradeID}`;
        return ajaxCallPromise("POST", api, "");
      } else
        showInvasivePopup(
          "x",
          "Error",
          "Trade acceptance failed! Please check your internet connection.",
        );
    })
    .catch((error) => {
      showInvasivePopup(
        "x",
        "Error",
        "Error in accepting trade request. Please check your internet connection or submit a bug report. " +
          error,
      );
    });
}

// Function to reject trade

function handleRejectTrade(tradeID) {
  let api = `${BASE_URL}/BookStoreUser/TradeOfferRejected?TradeID=${tradeID}`;
  return ajaxCallPromise("PUT", api, "").then((status) => {
    let api = `${BASE_URL}/BookStoreUser/TradeConclusionNotification?TradeID=${tradeID}`;
    if (status) return ajaxCallPromise("POST", api, "");
  });
}

// Helper function to mark a notification as read
function markNotificationAsRead(notificationID) {
  let api = `${BASE_URL}/Notification/MarkNotificationAsRead?notificationID=${notificationID}`;
  ajaxCallPromise("PUT", api, "")
    .then(() => {})
    .catch((error) => {
      showDiscretePopup("Error", "Error marking notification as read:", error);
    });
}

// Helper function to map notification type to header
function mapNotificationTypeToHeader(notificationType) {
  switch (notificationType) {
    case "trade offer incoming":
      return "Trade Offer Incoming";
    case "trade accepted":
      return "Trade Accepted";
    case "trade rejected":
      return "Trade Rejected";
    case "forum post mentioned":
      return "Forum Post Mentioned";
    case "book bought":
      return "Book Bought";
    default:
      return "Notification";
  }
}
