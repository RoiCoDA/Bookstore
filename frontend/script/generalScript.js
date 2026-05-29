// Managing users ( remembered and not )

import { userLoggedIn } from "./userUtilities.js";
import { ajaxCallPromise } from "./ajaxCalls.js";
import { BASE_URL } from "./config.js";

var userActive = userLoggedIn();

if (
  !window.location.pathname.endsWith("/main/userPage.html") &&
  sessionStorage.getItem("viewUserID")
) {
  // If we're not on 'userPage.html' and 'viewUserID' exists, remove 'viewUserID' from sessionStorage
  sessionStorage.removeItem("viewUserID");
}

///// Check for Notifications /////
function CheckForNotifications() {
  if (userActive.userID !== 0) {
    let api = `${BASE_URL}/BookStoreUser/DoesUserHaveUnreadNotifications?userID=${userActive.userID}`;

    ajaxCallPromise("GET", api, "")
      .then((status) => {
        if (status) {
          // Find all bell icons
          document
            .querySelectorAll('img[src="../media/bell.svg"]')
            .forEach((img) => {
              // Replace src with the red bell-dot icon
              img.src = "../media/bell-dot.svg";
              // Apply red color to the SVG icon
              img.style.filter =
                "invert(78%) sepia(75%) saturate(724%) hue-rotate(349deg) brightness(97%) contrast(103%)";

              // Add Animate.css class and start the animation loop
              img.classList.add("animate__animated", "animate__heartBeat");

              function animateBell() {
                // Remove the animation class to reset the animation
                img.classList.remove("animate__heartBeat");

                // Force reflow to allow re-triggering the animation
                void img.offsetWidth;

                // Re-add the animation class
                img.classList.add("animate__heartBeat");

                // Schedule the animation to loop every 5 seconds
                setTimeout(animateBell, 2500);
              }

              // Start the animation loop
              animateBell();
            });
        }
      })
      .catch((error) => {
        showInvasivePopup(
          "x",
          "Error",
          "Unable to show popups. Please check your internet connection or submit a bug report. " +
            error,
        );
      });
  }
}

//////////////////////////

arrangeGeneralSettingsForUsers();

function arrangeGeneralSettingsForUsers() {
  const greetContainer = document.querySelector(".UserGreetContainer");
  if (greetContainer) {
    greetContainer.innerHTML = "Hello, " + userActive.userName;
  }
  if (userActive.userID == 0) {
    $(".UserLogOutContainer").css("display", "none");
  } else $(".UserLogOutContainer").css("display", "flex");
}

// FIREBASE AND LOGIN //

import {
  createUserWithEmailAndPassword,
  signInWithEmailAndPassword,
  signInWithPopup,
  updateProfile,
} from "https://www.gstatic.com/firebasejs/10.12.5/firebase-auth.js";
import { auth, googleProvider } from "./firebaseConfig.js";

import { searchInCurrentPage } from "./searchPage.js";

//  /////////////////////////////////////////  //

// LOGIN AND SIGNUP //

document.addEventListener("DOMContentLoaded", function () {
  const signupForm = document.getElementById("signupForm");
  const loginForm = document.getElementById("loginForm");
  const googleButtons = document.getElementsByClassName("googleUserInteract");

  if (signupForm) {
    signupForm.addEventListener("submit", function (e) {
      e.preventDefault();
      registerUser();
    });
  }

  if (loginForm) {
    loginForm.addEventListener("submit", function (e) {
      e.preventDefault();
      loginUser();
    });
  }

  Array.from(googleButtons).forEach((button) => {
    button.addEventListener("click", function (e) {
      e.preventDefault();
      enterWGoogle();
    });
  });
});

const EMAIL_REGEX = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

function registerUser() {
  let userRegisterName = document
    .getElementById("userNameSignupInput")
    .value.trim();
  let userRegisterEmail = document
    .getElementById("userEmailSignupInput")
    .value.trim();
  let userRegisterPassword = document.getElementById(
    "userPasswordSignupInput",
  ).value;
  let rememberMeRegisterCheckbox =
    document.getElementById("remember-signup").checked;

  if (!userRegisterName) {
    showInvasivePopup("x", "Error", "Username cannot be empty.");
    return;
  }
  if (!EMAIL_REGEX.test(userRegisterEmail)) {
    showInvasivePopup("x", "Error", "Please enter a valid email address.");
    return;
  }
  if (userRegisterPassword.length < 6) {
    showInvasivePopup(
      "x",
      "Error",
      "Password must be at least 6 characters long.",
    );
    return;
  }
  createUserWithEmailAndPassword(auth, userRegisterEmail, userRegisterPassword)
    .then((userCredential) => {
      const user = userCredential.user;

      return updateProfile(user, { displayName: userRegisterName }) // updating user's name in the DB
        .then(() => {
          let userToDB = {
            userName: userRegisterName,
            userPassword: userRegisterPassword,
            userEmail: userRegisterEmail,
            dateCreated: new Date(),
            //image: "https://localhost:7013/images/DefaultUser.png",
            image:
              "https://proj.ruppin.ac.il/cgroup79/test2/tar1/images/DefaultUser.png",
            isActive: true,
            isAdmin: false,
            signature: "",
          };

          const api = `${BASE_URL}/BookStoreUser/registerUser`; // Adjust the API name as needed

          // Register the user in your backend
          return ajaxCallPromise("POST", api, JSON.stringify(userToDB));
        });
    })
    .then(function (registerResponse) {
      if (!registerResponse) {
        return Promise.reject(new Error("User registration failed"));
      }

      const api = `${BASE_URL}/BookStoreUser/getUserData`; // Adjust the API to get user data
      return ajaxCallPromise("GET", api, { userEmail: userRegisterEmail });
    })
    .then(function (registeredUserData) {
      let registeredUser = registeredUserData[0];
      if (rememberMeRegisterCheckbox) {
        localStorage.setItem("user", JSON.stringify(registeredUser));
        sessionStorage.removeItem("user");
        window.location.href = "../main/index.html";
      } else {
        sessionStorage.setItem("user", JSON.stringify(registeredUser));
        window.location.href = "../main/index.html";
      }
    })
    .catch((error) => {
      showInvasivePopup(
        "x",
        "Error",
        "Error in registry process. Please check your internet connection or submit a bug report. " +
          error,
      );
      // Possibilities to handle error (popup, etc.)
    });
}

function loginUser() {
  let userLoginEmail = document
    .getElementById("userEmailLoginInput")
    .value.trim();
  let userLoginPassword = document.getElementById(
    "userPasswordLoginInput",
  ).value;
  let rememberMeLoginCheckbox =
    document.getElementById("remember-login").checked;

  if (!EMAIL_REGEX.test(userLoginEmail)) {
    showInvasivePopup("x", "Error", "Please enter a valid email address.");
    return;
  }
  if (!userLoginPassword) {
    showInvasivePopup("x", "Error", "Password cannot be empty.");
    return;
  }
  signInWithEmailAndPassword(auth, userLoginEmail, userLoginPassword)
    .then(function (googleUser) {
      let api = `${BASE_URL}/BookStoreUser/getUserData`;
      let ImportedUserEmail = googleUser.user.email;
      return ajaxCallPromise("GET", api, { userEmail: ImportedUserEmail });
    })
    .then(function (loggedInUserData) {
      let LoggedInUser = loggedInUserData[0];

      if (rememberMeLoginCheckbox) {
        localStorage.setItem("user", JSON.stringify(LoggedInUser));
        sessionStorage.removeItem("user");
        window.location.href = "../main/index.html";
      } else {
        sessionStorage.setItem("user", JSON.stringify(LoggedInUser));
        window.location.href = "../main/index.html";
      }
    })
    .catch((error) => {
      // console.log(error.code, error.message);
      showInvasivePopup(
        "x",
        "Error",
        "Invalid email or password. Try changing your password or email.",
      );
    });
}

function enterWGoogle() {
  var userEmailToDB;
  let rememberMeRegisterCheckbox =
    document.getElementById("remember-signup").checked;
  let rememberMeLoginCheckbox =
    document.getElementById("remember-login").checked;
  signInWithPopup(auth, googleProvider)
    .then((result) => {
      // This gives you a Google Access Token. You can use it to access the Google API.
      const credential = GoogleAuthProvider.credentialFromResult(result);
      const token = credential.accessToken;
      const user = result.user; // The signed-in user info.

      // Extract the user's name and email
      var userNameToDB = user.displayName;
      userEmailToDB = user.email;

      var userToDB = {
        userName: userNameToDB,
        userPassword: "TempPasswordTillConfigured111",
        userEmail: userEmailToDB,
        dateCreated: new Date(),
        //image: "https://localhost:7013/images/DefaultUser.png",
        image:
          "https://proj.ruppin.ac.il/cgroup79/test2/tar1/images/DefaultUser.png",
        isActive: true,
        isAdmin: false,
        signature: "",
      };

      const api = `${BASE_URL}/BookStoreUser/getUserData`;
      return ajaxCallPromise("GET", api, { userEmail: userEmailToDB }).then(
        function (checkIfUserExists) {
          if (checkIfUserExists.length > 0) {
            // Google user exists
            const api = `${BASE_URL}/BookStoreUser/getUserData`; // Adjust the API to get user data
            return ajaxCallPromise("GET", api, {
              userEmail: userEmailToDB,
            }).then(function (registeredUserData) {
              let registeredUser = registeredUserData[0];
              if (rememberMeRegisterCheckbox || rememberMeLoginCheckbox) {
                sessionStorage.removeItem("user");
                localStorage.setItem("user", JSON.stringify(registeredUser));
              } else {
                sessionStorage.setItem("user", JSON.stringify(registeredUser));
              }
              window.location.href = "../main/index.html";
            });
          } else {
            // Google user does not exist
            const api = `${BASE_URL}/BookStoreUser/registerUser`; // Adjust the API name as needed
            return ajaxCallPromise("POST", api, JSON.stringify(userToDB))
              .then(function (registerResponse) {
                if (!registerResponse) {
                  return Promise.reject(new Error("User registration failed"));
                }
                const api = `${BASE_URL}/BookStoreUser/getUserData`; // Adjust the API to get user data
                return ajaxCallPromise("GET", api, {
                  userEmail: userEmailToDB,
                });
              })
              .then(function (registeredUserData) {
                let registeredUser = registeredUserData[0];
                if (rememberMeRegisterCheckbox || rememberMeLoginCheckbox) {
                  sessionStorage.removeItem("user");
                  localStorage.setItem("user", JSON.stringify(registeredUser));
                  window.location.href = "../main/index.html";
                } else {
                  sessionStorage.setItem(
                    "user",
                    JSON.stringify(registeredUser),
                  );
                  window.location.href = "../main/index.html";
                }
              });
          }
        },
      );
    })
    .catch((error) => {
      // Handle Errors here.
      const errorCode = error.code;
      const errorMessage = error.message;
      const email = error.customData.email; // The email of the user's account used.
      const credential = GoogleAuthProvider.credentialFromError(error); // The AuthCredential type that was used.
    });
}

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

var width = window.innerWidth;

// ////////// //

$(document).ready(function () {
  let userActive = userLoggedIn();
  if (userActive.userID == 0) {
    if (
      !(
        window.location.href.endsWith("index.html") ||
        window.location.href.endsWith("/main/") ||
        window.location.href.endsWith("loginSignupPage.html") ||
        window.location.href.endsWith("allWritersPage.html") ||
        window.location.href.endsWith("bookPage.html") ||
        window.location.href.includes("searchPage.html") ||
        window.location.href.includes("writerPage.html")
      )
    ) {
      window.location.href = "../main/loginSignupPage.html";
    }
  }
  CheckForNotifications();

  function checkWidth() {
    width = window.innerWidth;
    if (width > 800) {
      $(".TopNavBar__Mobile").hide();
      $(".TopNavBar__Desktop").css("display", "flex");
      $("#SideNavBar__Desktop").css("display", "flex");
    } else {
      $(".TopNavBar__Mobile").css("display", "flex");
      $(".TopNavBar__Desktop").hide();
      $("#SideNavBar__Mobile").css("display", "flex");
    }
  }

  // Initial adjustment
  checkWidth();

  // Adjust on window resize
  $(window).resize(function () {
    checkWidth();
  });

  // Toggle SideNav on mobile
  $("#menuButton").click(function () {
    $("#SideNavBar").toggleClass("active");
  });

  // Event listeners for buttons
  $(document).on("click", ".HomeBound", function () {
    window.location.href = "../main/index.html";
  });

  $(document).on("click", ".LibraryBound", function () {
    if (userActive.userID == 0) {
      window.location.href = "../main/loginSignupPage.html";
    } else {
      sessionStorage.removeItem("viewUserID");
      window.location.href = "../main/userPage.html";
    }
  });

  $(document).on("click", ".NotificationBound", function () {
    if (userActive.userID == 0) {
      window.location.href = "../main/loginSignupPage.html";
    } else window.location.href = "../main/NotificationsPage.html";
  });

  $(document).on("click", ".UserBound", function () {
    if (userActive.userID == 0) {
      window.location.href = "../main/loginSignupPage.html";
    } else {
      sessionStorage.removeItem("viewUserID");
      window.location.href = "../main/userPage.html";
    }
  });

  $(document).on("click", ".TradingBound", function () {
    if (userActive.userID == 0) {
      window.location.href = "../main/loginSignupPage.html";
    } else window.location.href = "../main/TradingPage.html";
  });
  $(document).on("click", ".SearchBound", function () {
    window.location.href = "../main/SearchPage.html";
  });

  $(document).on("click", ".BonusBound", function () {
    window.location.href = "../main/userPage.html";
  });

  $(document).on("click", ".AllWritersBound", function () {
    window.location.href = "../main/allWritersPage.html";
  });

  $(document).on("click", ".UserEditBound", function () {
    if (userActive.userID == 0) {
      window.location.href = "../main/loginSignupPage.html";
    } else window.location.href = "../main/userEditPage.html";
  });

  $(document).on("click", ".UserLogOutContainer", function () {
    localStorage.removeItem("user");
    sessionStorage.removeItem("user");
    location.reload();
  });

  $(document).on("click", ".BonusBound", function () {
    window.location.href = "../main/additions.html";
  });

  $(document).on("click", ".MenuBttnContainer", function () {
    $("#SideNavBar__Mobile")
      .css("visibility", "visible")
      .toggleClass("activeMobile");
  });

  $(document).on("click", ".QuizBound", function () {
    if (userActive.userID == 0) {
      window.location.href = "../main/loginSignupPage.html";
    } else window.location.href = "../main/quiz.html";
  });
  getFlashSaleBooks();
});

function formatDate(dateString) {
  // Assuming dateString is in ISO format, e.g., "2024-08-15T00:00:00Z"
  const options = { year: "numeric", month: "long", day: "numeric" };
  const date = new Date(dateString);
  return date.toLocaleDateString(undefined, options);
}

/*Dan's Impossible Mission  */
/*Event listener to hide/display search navigation bar with filters on pages (not mobile) */

document.addEventListener("DOMContentLoaded", function () {
  if (window.location.href.includes("loginSignupPage.html")) {
    return;
  }
  const searchBar = document.querySelector(".TopNavBarSearchBar");
  const dropdown = document.querySelector(".dropdown");

  searchBar.addEventListener("click", function (event) {
    // Prevent event from bubbling up to the document
    event.stopPropagation();
    // Make the dropdown visible
    dropdown.style.visibility = "visible";
  });

  // Hide dropdown when clicking outside of searchBar and dropdown
  document.addEventListener("click", function (event) {
    if (!searchBar.contains(event.target) && !dropdown.contains(event.target)) {
      dropdown.style.visibility = "hidden";
    }
    document.querySelectorAll(".tab.search-type").forEach(function (t) {
      t.classList.remove("active");
    });
  });

  // Prevent the dropdown from closing when clicking inside of it
  dropdown.addEventListener("click", function (event) {
    // Prevent event from bubbling up to the document
    event.stopPropagation();
  });
});

/*Event listener to hide/display search navigation bar with filters on pages (mobile) */

document.addEventListener("DOMContentLoaded", function () {
  if (window.location.href.includes("loginSignupPage.html")) {
    return;
  }
  const searchButton = document.querySelector(".SearchBttnContainer");
  const mobileSearchWindow = document.querySelector(".MobileSearchWindow");

  searchButton.addEventListener("click", function (event) {
    // Prevent event from bubbling up to the document
    event.stopPropagation();
    // Make the mobile search window visible
    mobileSearchWindow.style.visibility = "visible";
  });

  // Hide mobile search window when clicking outside of searchButton and mobileSearchWindow
  document.addEventListener("click", function (event) {
    if (
      !searchButton.contains(event.target) &&
      !mobileSearchWindow.contains(event.target)
    ) {
      mobileSearchWindow.style.visibility = "hidden";
    }
  });

  // Prevent the mobile search window from closing when clicking inside of it
  mobileSearchWindow.addEventListener("click", function (event) {
    // Prevent event from bubbling up to the document
    event.stopPropagation();
  });
});

// When navigating to a user's page
function navigateToUserPage(userID) {
  sessionStorage.setItem("viewUserID", userID);
  window.location.href = "userPage.html"; // Redirect to the user's page
}

/* Search Functionality */
document.addEventListener("DOMContentLoaded", function () {
  if (window.location.href.includes("loginSignupPage.html")) {
    return;
  }

  function handleSearch(searchBarId, searchButtonId) {
    var selectedSearchType = "books"; // Default search type

    // Set up click event listeners for search options
    document.querySelectorAll(".tab.search-type").forEach(function (tab) {
      tab.addEventListener("click", function () {
        // Remove active class from all tabs
        document.querySelectorAll(".tab.search-type").forEach(function (t) {
          t.classList.remove("active");
        });

        // Add active class to the clicked tab
        tab.classList.add("active");

        // Update the selected search type
        selectedSearchType = tab.getAttribute("data-search-type");
      });
    });

    document
      .getElementById(searchButtonId)
      .addEventListener("click", function () {
        var query = document.getElementById(searchBarId).value.trim();
        var currentPage = window.location.pathname.split("/").pop(); // Get the current page name

        if (query) {
          if (currentPage === "searchPage.html") {
            // Perform search within the current page
            searchInCurrentPage(query, selectedSearchType);
          } else {
            // Redirect to searchPage.html with query and search type parameters
            window.location.href =
              "searchPage.html?q=" +
              encodeURIComponent(query) +
              "&type=" +
              encodeURIComponent(selectedSearchType);
          }
        }
      });
  }

  // Set up search functionality for both desktop and mobile
  handleSearch("TopNavBarSearchBarDesktop", "searchButtonDesktop");
  handleSearch("TopNavBarSearchBarMobile", "searchButtonMobile");
});

/////////////////// Add admininitration option if user is admin //////////////

document.addEventListener("DOMContentLoaded", function () {
  // Check if the user is an admin
  if (userActive.userID == 1) {
    // Create the Administration list item
    const adminListItem = document.createElement("li");
    adminListItem.className = "AdminBound";
    adminListItem.textContent = "Administration";

    // Append the Administration list item to the SideNavBarOptions
    document.getElementById("SideNavBarOptions").appendChild(adminListItem);
  }

  // Existing click event listeners
  $(document).on("click", ".UserBound", function () {
    if (userActive.userID == 0) {
      window.location.href = "../main/loginSignupPage.html";
    } else {
      sessionStorage.removeItem("viewUserID");
      window.location.href = "../main/userPage.html";
    }
  });

  // Add click event listener for the AdminBound list item
  $(document).on("click", ".AdminBound", function () {
    window.location.href = "../main/adminPage.html";
  });
});

function getFlashSaleBooks() {
  let api = `${BASE_URL}/Book/GetFlashSaleBooks`;
  return ajaxCallPromise("GET", api, "")
    .then((saleBooks) => {
      if (saleBooks.length > 0) {
        showDiscretePopup("Sale", "Flash sale is live!");
        initConfetti();
        if (window.location.href.includes("index.html")) {
          //Draw();
          $("#BookFlashSaleContainer").css("display", "block");
          renderFlashSaleBooks(saleBooks);
        }
      } else {
        return;
      }
    })
    .catch((error) => {
      showDiscretePopup(
        "Error",
        "Error fetching flash sale data. Please check your interent connection or submit a bug report. " +
          error,
      );
    });
}

function renderFlashSaleBooks(saleBooks) {
  //Draw();
  if (window.location.href.includes("index.html")) {
    //Draw();
    //initConfetti();
  }
  const macroBox = $("#BookFlashSaleMacroBox"); // make it visible as the sale starts, it should be invisible!
  // Render all the flash sale books
  saleBooks.forEach((book) => {
    const bookHTML = `
      <div class="flash-sale-book" data-book-id="${book.bookID}" style="background-image: url('${book.thumbnail}');">
        <div class="flash-sale-book-title">${book.title}</div>
      </div>`;
    macroBox.append(bookHTML);
  });

  // Display the first book in the micro sale box by default
  if (saleBooks.length > 0) {
    updateMicroBox(saleBooks[0]);
  }

  // Add click event listener to each book
  $(".flash-sale-book").on("click", function () {
    const bookID = $(this).data("book-id");
    const selectedBook = saleBooks.find((book) => book.bookID === bookID);
    if (selectedBook) {
      updateMicroBox(selectedBook);
    }
  });
}

// Function to update the micro box with selected book's info
function updateMicroBox(book) {
  // Clear previous button if it exists
  $("#BookFlashSaleMicroBox .go-to-book-page-btn").remove();

  // Update the photo and prices
  $("#BookFlashSaleMicroBookPhoto")
    .css("background-image", `url('${book.thumbnail}')`)
    .css({ width: "150px", height: "200px" });

  $("#BookFlashSaleAfterPrice").text(`$${book.salePrice.toFixed(2)}`);
  $("#BookFlashSaleBeforePrice").text(`$${(book.salePrice * 2).toFixed(2)}`);

  // Add the "Go to book page" button with an event listener
  const buttonHTML = `<button class="go-to-book-page-btn" data-book-id="${book.bookID}">Go to book page</button>`;
  $("#BookFlashSaleMicroBox").append(buttonHTML);

  // Attach event listener to the new button
  $(".go-to-book-page-btn").on("click", function () {
    const bookID = $(this).data("book-id");
    localStorage.setItem("bookID", bookID);
    window.location.href = "bookPage.html";
  });
}

//----------------------------------------------------------------
//-- Confetti
//----------------------------------------------------------------
let W, H;
const maxConfettis = 150;
const particles = [];
let reentryCount = 0; // Counter for re-entries
const maxReentries = 2; // Number of times to allow re-entry

const possibleColors = [
  "DodgerBlue",
  "OliveDrab",
  "Gold",
  "Pink",
  "SlateBlue",
  "LightBlue",
  "Gold",
  "Violet",
  "PaleGreen",
  "SteelBlue",
  "SandyBrown",
  "Chocolate",
  "Crimson",
];

function randomFromTo(from, to) {
  return Math.floor(Math.random() * (to - from + 1) + from);
}

function confettiParticle() {
  this.x = Math.random() * W; // x
  this.y = Math.random() * H - H; // y
  this.r = randomFromTo(11, 33); // radius
  this.d = Math.random() * maxConfettis + 11;
  this.color =
    possibleColors[Math.floor(Math.random() * possibleColors.length)];
  this.tilt = Math.floor(Math.random() * 33) - 11;
  this.tiltAngleIncremental = Math.random() * 0.07 + 0.05;
  this.tiltAngle = 0;

  this.draw = function (context) {
    context.beginPath();
    context.lineWidth = this.r / 2;
    context.strokeStyle = this.color;
    context.moveTo(this.x + this.tilt + this.r / 3, this.y);
    context.lineTo(this.x + this.tilt, this.y + this.tilt + this.r / 5);
    return context.stroke();
  };
}

function Draw(context) {
  if (!window.location.href.includes("index.html")) {
    return;
  }

  requestAnimationFrame(() => Draw(context));

  context.clearRect(0, 0, W, H);

  for (let i = 0; i < maxConfettis; i++) {
    particles[i].draw(context);
  }

  let particle = {};
  let remainingFlakes = 0;
  for (let i = 0; i < maxConfettis; i++) {
    particle = particles[i];

    particle.tiltAngle += particle.tiltAngleIncremental;
    particle.y += (Math.cos(particle.d) + 3 + particle.r / 2) / 2;
    particle.tilt = Math.sin(particle.tiltAngle - i / 3) * 15;

    if (particle.y <= H) remainingFlakes++;

    // If a confetti has fluttered out of view,
    // bring it back to above the viewport and let it re-fall.
    if (reentryCount < maxReentries) {
      if (particle.x > W + 30 || particle.x < -30 || particle.y > H) {
        particle.x = Math.random() * W;
        particle.y = -30;
        particle.tilt = Math.floor(Math.random() * 10) - 20;
        reentryCount++;
      }
    }
  }
}

// Initialize canvas and confetti
function initConfetti() {
  const canvas = document.getElementById("canvas");
  if (!canvas) return; // Exit if canvas is not found

  const context = canvas.getContext("2d");
  if (!context) return; // Exit if context is not found

  W = window.innerWidth;
  H = window.innerHeight;

  canvas.width = W;
  canvas.height = H;

  // Push new confetti objects to `particles[]`
  for (let i = 0; i < maxConfettis; i++) {
    particles.push(new confettiParticle());
  }

  // Start the confetti animation
  Draw(context);
}

// Set up canvas on load and resize
//window.addEventListener("load", initConfetti);
window.addEventListener(
  "resize",
  function () {
    W = window.innerWidth;
    H = window.innerHeight;
    const canvas = document.getElementById("canvas");
    if (canvas) {
      canvas.width = window.innerWidth;
      canvas.height = window.innerHeight;
    }
  },
  false,
);

//----------------------------------------------------------------
//-- Popups
//----------------------------------------------------------------

function removePopupIfExists(popupId) {
  const existingPopup = document.getElementById(popupId);
  if (existingPopup) {
    existingPopup.remove();
  }
}

function showDiscretePopup(header, body) {
  removePopupIfExists("discrete-popup");

  // Create a new popup element
  const popup = document.createElement("div");
  popup.id = "discrete-popup";
  popup.className = "popup discrete-popup animate__animated animate__fadeInUp";
  popup.style.display = "block";

  // Create header and body elements
  const headerElement = document.createElement("div");
  headerElement.className = "popup-header";
  headerElement.textContent = header;

  const bodyElement = document.createElement("div");
  bodyElement.className = "popup-body";
  bodyElement.textContent = body;

  // Append header and body to the popup
  popup.appendChild(headerElement);
  popup.appendChild(bodyElement);

  // Append popup to the document body
  document.body.appendChild(popup);

  // Set timeout to remove the popup
  setTimeout(() => {
    popup.classList.remove("animate__fadeInUp");
    popup.classList.add("animate__fadeOutDown");
    popup.addEventListener(
      "animationend",
      () => {
        popup.remove();
      },
      { once: true },
    );
  }, 5000); // Display for 5 seconds
}

function showInvasivePopup(type, header, body) {
  removePopupIfExists("invasive-popup");

  // Create a new popup element
  const popup = document.createElement("div");
  popup.id = "invasive-popup";
  popup.className = "popup invasive-popup animate__animated animate__fadeIn";
  popup.style.display = "block";

  // Create header, icon, and body elements
  const headerElement = document.createElement("div");
  headerElement.className = "popup-header";
  headerElement.textContent = header;

  const iconElement = document.createElement("img");
  iconElement.id = "invasive-icon";
  iconElement.alt = "Icon";
  iconElement.width = 100;
  iconElement.height = 100;
  iconElement.src = type === "v" ? "../media/check.svg" : "../media/x.svg";

  const bodyElement = document.createElement("p");
  bodyElement.id = "invasive-message";
  bodyElement.textContent = body;

  // Append header, icon, and body to the popup
  const bodyContainer = document.createElement("div");
  bodyContainer.className = "popup-body";
  bodyContainer.appendChild(iconElement);
  bodyContainer.appendChild(bodyElement);

  popup.appendChild(headerElement);
  popup.appendChild(bodyContainer);

  // Append popup to the document body
  document.body.appendChild(popup);

  // Set timeout to remove the popup
  setTimeout(() => {
    popup.classList.remove("animate__fadeIn");
    popup.classList.add("animate__fadeOut");
    popup.addEventListener(
      "animationend",
      () => {
        popup.remove();
      },
      { once: true },
    );
  }, 3000); // Display for 3 seconds
}

//----------------------------------------------------------------
//-- Confetti
//----------------------------------------------------------------

export {
  renderFlashSaleBooks,
  CheckForNotifications,
  showDiscretePopup,
  showInvasivePopup,
};
