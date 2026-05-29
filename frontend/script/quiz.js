import { showDiscretePopup } from "./generalScript.js";
import { ajaxCallPromise } from "./ajaxCalls.js";
import { BASE_URL } from "./config.js";

var questionFunctions = [
  getDatePublishedQuestion,
  getPublisherQuestion,
  getPageCountQuestion,
  getAuthorQuestion,
  getAuthorImageQuestion,
];

var currentQuestionIndex = 0;
var questionCount = 5;
var timer;
var timeLimit = 15; // Seconds
var timerInterval;
var correctAnswer;
var questionAnswered = false;
var userScore = 0;
var seconds;

$(document).ready(function () {
  let currentUser =
    localStorage.getItem("user") || sessionStorage.getItem("user");
  let currentUserID = JSON.parse(currentUser).userID;
  if (currentUserID == 0) {
    window.location.href = "../main/index.html";
  }

  GetTop10UsersByScore();

  $("#startQuizButton").on("click", startQuiz);
});

function startQuiz() {
  $("#startQuizButton").css("display", "none");
  $("#timer-container").css("display", "block");
  $(".question-title").css("display", "inline");
  $("#QuizStartTitle").css("display", "none");
  currentQuestionIndex = 0;
  userScore = 0;
  askNextQuestion();
}

function askNextQuestion() {
  if (currentQuestionIndex >= questionCount) {
    endQuiz();
    return;
  }
  questionAnswered = false;
  var randomIndex = Math.floor(Math.random() * questionFunctions.length);
  var questionFunction = questionFunctions[randomIndex];
  questionFunction(); // Call the selected question function
}

function getDatePublishedQuestion() {
  var api = `${BASE_URL}/Book/GetBookQuestion_WhichDatePublished`;
  $.ajax({
    url: api,
    method: "GET",
    success: function (data) {
      displayDatePublishedQuestion(data);
    },
    error: function (error) {},
  });
}

function displayDatePublishedQuestion(data) {
  if (
    !data ||
    !data.bookTitle ||
    !data.correctAnswer ||
    !Array.isArray(data.incorrectAnswers)
  ) {
    return;
  }

  document.getElementById("QuestionTitle").textContent =
    `When was the book "${data.bookTitle}" published?`;
  $("#AuthorImageContainer").hide();

  const formattedCorrectAnswer = formatDate(data.correctAnswer);
  const formattedIncorrectAnswers = data.incorrectAnswers.map((date) =>
    formatDate(date),
  );

  const options = [...formattedIncorrectAnswers, formattedCorrectAnswer];
  options.sort(() => Math.random() - 0.5);

  renderOptions(options, formattedCorrectAnswer);
}

function getPublisherQuestion() {
  var api = `${BASE_URL}/Book/GetBookQuestion_WhichPublisher`;
  $.ajax({
    url: api,
    method: "GET",
    success: function (data) {
      displayPublisherQuestion(data);
    },
    error: function (error) {},
  });
}

function displayPublisherQuestion(data) {
  if (
    !data ||
    !data.bookTitle ||
    !data.correctAnswer ||
    !Array.isArray(data.incorrectAnswers)
  ) {
    return;
  }

  document.getElementById("QuestionTitle").textContent =
    `Who is the publisher of the book "${data.bookTitle}"?`;
  $("#AuthorImageContainer").hide();

  const options = [...data.incorrectAnswers, data.correctAnswer];
  options.sort(() => Math.random() - 0.5);

  renderOptions(options, data.correctAnswer);
}

function getPageCountQuestion() {
  var api = `${BASE_URL}/Book/GetBookQuestion_WhichPageCount`;
  $.ajax({
    url: api,
    method: "GET",
    success: function (data) {
      displayPageCountQuestion(data);
    },
    error: function (error) {},
  });
}

function displayPageCountQuestion(data) {
  if (
    !data ||
    !data.bookTitle ||
    !data.correctAnswer ||
    !Array.isArray(data.incorrectAnswers)
  ) {
    return;
  }

  document.getElementById("QuestionTitle").textContent =
    `What is the page count of the book "${data.bookTitle}"?`;
  $("#AuthorImageContainer").hide();

  const options = [...data.incorrectAnswers, data.correctAnswer];
  options.sort(() => Math.random() - 0.5);

  renderOptions(options, data.correctAnswer);
}

function getAuthorQuestion() {
  var api = `${BASE_URL}/Book/GetBookQuestion_WhichAuthor`;
  $.ajax({
    url: api,
    method: "GET",
    success: function (data) {
      displayAuthorQuestion(data);
    },
    error: function (error) {},
  });
}

function displayAuthorQuestion(data) {
  if (
    !data ||
    !data.bookTitle ||
    !data.correctAnswer ||
    !Array.isArray(data.incorrectAnswers)
  ) {
    return;
  }

  document.getElementById("QuestionTitle").textContent =
    `Who is the author of the book "${data.bookTitle}"?`;
  $("#AuthorImageContainer").hide();

  const options = [...data.incorrectAnswers, data.correctAnswer];
  options.sort(() => Math.random() - 0.5);

  renderOptions(options, data.correctAnswer);
}

function getAuthorImageQuestion() {
  var api = `${BASE_URL}/Book/GetAuthorQuestion_WhichAuthorImage`;
  $.ajax({
    url: api,
    method: "GET",
    success: function (data) {
      displayAuthorImageQuestion(data);
    },
    error: function (error) {},
  });
}

function displayAuthorImageQuestion(data) {
  if (
    !data ||
    !data.correctAnswer ||
    !data.authorImage ||
    !Array.isArray(data.incorrectAnswers)
  ) {
    return;
  }

  $("#QuestionTitle").text("Who is the author in this picture?");
  $("#AuthorImageContainer").show();
  $("#authorImage").attr("src", data.authorImage);

  const options = data.incorrectAnswers.map((item) => item.item1);
  options.push(data.correctAnswer);
  options.sort(() => Math.random() - 0.5);

  renderOptions(options, data.correctAnswer);
}

function renderOptions(options, correctAnswer) {
  const optionsContainer = document.querySelector(".quiz-options");
  optionsContainer.innerHTML = "";
  let answerSelected = false;

  options.forEach((option, index) => {
    const optionDiv = document.createElement("div");
    optionDiv.className = "quiz-option";
    optionDiv.innerHTML = `<strong>${String.fromCharCode(
      65 + index,
    )})&nbsp;</strong> ${option}`;

    // An event listener with a check to prevent multiple selections
    optionDiv.onclick = () => {
      if (!answerSelected) {
        checkAnswer(option, correctAnswer);
        answerSelected = true; // Set the flag after the first selection
      }
    };

    optionsContainer.appendChild(optionDiv);
  });

  correctAnswer = correctAnswer;
  startTimer(timeLimit, correctAnswer);
}

function startTimer(duration, correctAnswer) {
  var timerDisplay = $("#timer-countdown");
  var timerBar = $("#timer-progress");
  var startTime = Date.now();
  var endTime = startTime + duration * 1000;
  var interval = 10; // Update the timer every 10 milliseconds for smooth effect

  // Ensure timer bar is immediately filled without transition
  timerBar.css({
    width: "100%",
    transition: "none", // Disable transition initially
  });

  // Initial update to show the timer bar as fully filled
  function initialUpdate() {
    timerBar.css({
      width: "100%",
      transition: "none",
    });
  }

  // Update the timer and transition the bar width
  function updateTimer() {
    var now = Date.now();
    var remainingTime = Math.max(0, endTime - now);
    seconds = Math.ceil(remainingTime / 1000); // Use Math.ceil to avoid skips
    var percentage = (remainingTime / (duration * 1000)) * 100;

    // Update the timer display
    timerDisplay.text(seconds + "s");

    // Update the timer bar width with a smooth transition
    timerBar.css({
      "background-color": "#4caf50",
      transition: "width 0.1s linear", // Apply transition for smoothness
      width: percentage + "%",
    });

    if (remainingTime <= 0) {
      clearInterval(timerInterval); // Stop the interval

      // Remove the background color to give the effect of the bar finishing
      timerBar.css({
        "background-color": "transparent",
        width: "0%",
        transition: "width 0.1s linear", // Transition for smooth shrinking
      });

      if (!questionAnswered) {
        // If no answer was selected, treat it as an incorrect answer
        var options = document.querySelectorAll(".quiz-option");
        options.forEach((option) => {
          if (option.textContent.includes(correctAnswer)) {
            option.classList.add("correct");
          } else {
            option.classList.add("incorrect");
          }
        });

        setTimeout(() => {
          handleAnswer(false);
        }, 100);
      }
    }
  }

  // Initial update to set the bar to full width
  initialUpdate();
  // Update every interval
  timerInterval = setInterval(updateTimer, interval);
}

function checkAnswer(selectedOption, correctAnswer) {
  if (questionAnswered) return; // Prevent multiple answers

  questionAnswered = true;
  clearInterval(timerInterval); // Stop the timer when an answer is selected

  var options = document.querySelectorAll(".quiz-option");
  options.forEach((option) => {
    if (option.textContent.includes(correctAnswer)) {
      option.classList.add("correct");
    } else {
      option.classList.add("incorrect");
    }

    if (option.textContent.includes(selectedOption)) {
      option.classList.add("selected");
    }
  });

  // if (option.textContent.includes(selectedOption)) {
  //     option.classList.add('selected');
  // }

  var isCorrect = selectedOption === correctAnswer;
  // Delay the call to handleAnswer by 1 second (1000 milliseconds)
  setTimeout(() => {
    handleAnswer(isCorrect);
  }, 100);
}

async function handleAnswer(isCorrect) {
  if (isCorrect) {
    // Handle correct answer (e.g., increment score)
    var score = seconds * 100; // Calculate score based on remaining time
    userScore += score; // Update user score
    //alert('Correct! You earn ' + score + ' points.');
    await showDiscretePopupAlert("Correct! You earn " + score + " points.");

    //alert('Correct!');
  } else {
    // Handle incorrect answer
    //alert('Incorrect!');
    await showDiscretePopupAlert("Incorrect!");
  }
  $("#authorImage").attr("src", "");
  // Move to the next question
  currentQuestionIndex++;
  askNextQuestion();
}

function showDiscretePopupAlert(header, body) {
  return new Promise((resolve) => {
    removePopupIfExists("discrete-popup");

    // Create a new popup element
    const popup = document.createElement("div");
    popup.id = "discrete-popup";
    popup.className =
      "popup discrete-popup animate__animated animate__fadeInUp";
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

    // Set a timeout to automatically close the popup after 3 seconds
    setTimeout(() => {
      popup.classList.remove("animate__fadeInUp");
      popup.classList.add("animate__fadeOutDown");
      popup.addEventListener(
        "animationend",
        () => {
          popup.remove();
          resolve(); // Resolves the promise after the popup is removed
        },
        { once: true },
      );
    }, 3000); // 3 seconds
  });
}

// Helper function to remove any existing popup with the same ID
function removePopupIfExists(popupId) {
  const existingPopup = document.getElementById(popupId);
  if (existingPopup) {
    existingPopup.remove();
  }
}

function endQuiz() {
  // Handle the end of the quiz (e.g., display results)
  //alert('Quiz ended');
  //showDiscretePopupAlert('Quiz ended');

  //alert('Quiz finished! Your score is ' + userScore + ' points.');
  $("#startQuizButton").css("display", "inline");
  $("#timer-container").css("display", "none");
  $(".question-title").css("display", "none");
  $("#QuizStartTitle").css("display", "block");
  document.getElementById("QuestionTitle").textContent = "";
  $("#AuthorImageContainer").hide();
  document.querySelector(".quiz-options").innerHTML = "";
  $("#QuizStartTitleHeader").text("Your score is " + userScore + " points.");
  $("#QuizStartTitleParagraph").text(
    "Press the button below to start the quiz again.",
  );
  let User = JSON.parse(
    localStorage.getItem("user") || sessionStorage.getItem("user"),
  );
  if (User && User.userID) {
    updateScore(User.userID, userScore);
  } else {
  }
}

/* Uses GetTop10UsersByScore() */
function updateScore(userID, score) {
  let api = `${BASE_URL}/BookStoreUser/UpdateUserScore`;
  let data = JSON.stringify({
    UserID: userID,
    UserScore: score,
  });

  ajaxCallPromise("PUT", api, data)
    .then((response) => {
      GetTop10UsersByScore();
    })
    .catch((error) => {});
}

function formatDate(dateString) {
  const options = { year: "numeric", month: "long", day: "numeric" };
  const date = new Date(dateString);
  return date.toLocaleDateString(undefined, options);
}

function GetTop10UsersByScore() {
  let api = `${BASE_URL}/BookStoreUser/GetTop10UsersByScore`;
  ajaxCallPromise("GET", api, "")
    .then((response) => {
      displayTop10UsersByScore(response);
    })
    .catch((error) => {});
}

function displayTop10UsersByScore(users) {
  const usersContainer = document.querySelector(".top-users");
  usersContainer.innerHTML = "";

  const filteredUsers = users.filter((user) => user.highestScore > 0);

  filteredUsers.forEach((user, index) => {
    const userDiv = document.createElement("div");
    userDiv.className = "user";
    userDiv.innerHTML = `<strong>${index + 1}.&nbsp;</strong> ${
      user.userName
    } - ${user.highestScore} points`;
    usersContainer.appendChild(userDiv);
  });

  if (filteredUsers.length < 10) {
    for (let i = filteredUsers.length; i < 10; i++) {
      const placeholderDiv = document.createElement("div");
      placeholderDiv.className = "user placeholder";
      placeholderDiv.innerHTML = `<strong>${i + 1}.&nbsp;</strong> ------`;
      usersContainer.appendChild(placeholderDiv);
    }
  }
}
