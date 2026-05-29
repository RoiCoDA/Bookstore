$(document).ready(function () {
  // Create the HTML structure for the accessibility button container
  var $accessibilityButtonContainer = $(`
        <!-- Accessibility Button Container -->
        <div id="accessibilityButtonContainer">
            <button id="accessibilityButton">
                <img src="../media/accessibility.svg" alt="Accessibility Options" class="icon" />
                <span id="closeAccessibilityButton">X</span>
                <span id="closeAccessibilityButtonDescription">Close</span> <!-- Description for close button -->

            </button>

            <!-- Accessibility Options Container (Initially hidden) -->
            <div id="accessibilityOptionsContainer">
                <!-- Tab 1 -->
                <div class="option-tab">
                    <h3>Read Text Aloud Mode</h3>
                    <!-- Toggle Button for Speech -->
                    <label class="toggle-switch">
                        <input type="checkbox" id="toggleSpeech" />
                        <span class="toggle-slider">
                            <img src="../media/speech.svg" alt="SpeechToggleIcon" class="icon" />
                        </span>
                    </label>
                </div>
                <!-- Tab 2 -->
                <div class="option-tab">
                    <h3>Color Blind Mode</h3>
                    <!-- Toggle Button for Color Blind Mode -->
                    <label class="toggle-switch">
                        <input type="checkbox" id="toggleColorBlind" />
                        <span class="toggle-slider">
                            <img src="../media/eye-off.svg" alt="EyeOffToggleIcon" class="icon" />
                        </span>
                    </label>
                </div>
            </div>
        </div>
    `);

  // Append the created HTML structure to the body
  $("body").append($accessibilityButtonContainer);

  // Toggle visibility of accessibility options container
  $("#accessibilityButton").on("click", function () {
    $("#accessibilityOptionsContainer").toggleClass("visible");
  });

  // Close the accessibility button container when the X is clicked
  $("#closeAccessibilityButton").on("click", function (event) {
    event.stopPropagation(); // Prevent the event from bubbling up
    $("#accessibilityButtonContainer").fadeOut(); // Fade out to hide
    $("#toggleSpeech").prop("checked", false); // Turn off read aloud button
    $("#toggleColorBlind").prop("checked", false);
    $("body").removeClass("color-blind-mode"); // Turn off color blind button
  });

  // Show description on hover for close button
  $("#closeAccessibilityButton").hover(
    function () {
      $("#closeAccessibilityButtonDescription").fadeIn(); // Show description on hover
    },
    function () {
      $("#closeAccessibilityButtonDescription").fadeOut(); // Hide description when not hovering
    },
  );

  const synth = window.speechSynthesis;
  let hoverTimeout;

  // Function to read text aloud
  function readText(text) {
    const utterance = new SpeechSynthesisUtterance(text);
    synth.speak(utterance);
  }

  // Setup hover event for reading text with a delay
  document.body.addEventListener("mouseover", function (event) {
    if ($("#toggleSpeech").is(":checked")) {
      // Check if the toggle is "on"
      const element = event.target;

      if (element && element.innerText) {
        // Check if the element is Main__Container__Content
        if (element === document.querySelector("#Main__Container__Content")) {
          // Clear any existing timeout to prevent multiple triggers
          clearTimeout(hoverTimeout);
        } else {
          // Clear any existing timeout to prevent multiple triggers
          clearTimeout(hoverTimeout);

          // Set a timeout to read text after 1.2 seconds
          hoverTimeout = setTimeout(() => readText(element.innerText), 1200);
        }
      }
    }
  });

  // Clear timeout if mouse leaves the element before the delay
  document.body.addEventListener("mouseout", function () {
    clearTimeout(hoverTimeout);
  });

  // Stop reading when the toggle is turned off
  $("#toggleSpeech").on("change", function () {
    if (!$(this).is(":checked")) {
      synth.cancel(); // Stop reading
    }
  });

  //toggle color blind mode
  $("#toggleColorBlind").on("change", function () {
    if ($(this).is(":checked")) {
      $("body").addClass("color-blind-mode"); // Apply color blind mode
    } else {
      $("body").removeClass("color-blind-mode"); // Remove color blind mode
    }
  });
});
