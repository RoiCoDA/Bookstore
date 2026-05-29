import { checkAndSetUserStorage, userLoggedIn } from "./userUtilities.js";
import { updatePassword } from "https://www.gstatic.com/firebasejs/10.12.5/firebase-auth.js";
import { auth } from "./firebaseConfig.js";
import { showDiscretePopup, showInvasivePopup } from "./generalScript.js";
import { ajaxCallPromise } from "./ajaxCalls.js";
import { BASE_URL } from "./config.js";
let userActive = userLoggedIn();

$(document).ready(function () {
  renderUserData();

  $("#UserChangePasswordForm").submit(function (event) {
    event.preventDefault();
    let password1 = document.getElementById("new-password1").value;
    let password2 = document.getElementById("new-password2").value;

    if (password1 !== password2) {
      showDiscretePopup("Error", "Passwords do not match. Please try again.");
      return;
    }

    if (password1.length < 6) {
      showDiscretePopup(
        "Error",
        "Password must be at least 6 characters long.",
      );
      return;
    }
    if (password1.length > 40) {
      showDiscretePopup(
        "Error",
        "password must be less than 40 characters long.",
      );
      return;
    }

    let tempUser = auth.currentUser;
    if (!tempUser) {
      showDiscretePopup("Error", "User is not logged in.");
      return;
    }

    updateUserPassword(tempUser, password1).then(() => {
      let api = `${BASE_URL}/BookStoreUser/ChangeUserPassword`;
      return ajaxCallPromise(
        "PUT",
        api,
        JSON.stringify({ UserID: userActive.userID, UserPassword: password1 }),
      ).then(function (response) {
        if (response) {
          showInvasivePopup("v", "Success", "Password changed successfully!");
          $("#UserChangePasswordForm")[0].reset();
        } else {
          showInvasivePopup(
            "x",
            "Error",
            "Password change failed. Please try again , or check your internet connection.",
          );
        }
      });
    });
  });
});

function updateUserPassword(user, newPassword) {
  return updatePassword(user, newPassword) // Ensure this returns a promise
    .then(() => {
      showInvasivePopup("v", "Success", "Password changed successfully!");
    })
    .catch((error) => {
      showInvasivePopup(
        "x",
        "Error",
        "Password change failed. Please try again , or check your internet connection, or submit a bug report " +
          error,
      );
    });
}

// Render user data //
function renderUserData() {
  let tempDate = new Date(userActive.dateCreated);
  let options = { year: "numeric", month: "long", day: "numeric" };
  let formattedDate = tempDate.toDateString(undefined, options);
  $("#UserProfileNameHolder").text("Username: " + userActive.userName);
  $("#UserProfileJoinDateHolder").text("Date Joined: " + formattedDate);
  if (userActive.userSignature == "") {
    $("#UserSignature").text("Signature not available.");
  } else {
    $("#UserSignature").html(`Signature: <br>${userActive.userSignature}`);
  }

  var imgElement = $("<img>")
    .attr("alt", userActive.userName)
    .addClass("profilepic")
    .on("error", function () {
      // Fallback image URL
      $(this).attr("src", "../media/user.png");
    });

  // Set the source for the image
  if (
    userActive.image === "Image link unavailable." ||
    userActive.image === ""
  ) {
    imgElement.attr("src", "../media/user.png");
  } else {
    imgElement.attr("src", userActive.image);
  }

  // Append the image to the profile picture frame
  $("#UserProfilePicFrame").append(imgElement);
}

$("#UserChangePictureForm").submit(function (event) {
  event.preventDefault();
  uploadImageChoice();
});

/* Checking if there is an uploaded image, scb uses submitingForm */
function uploadImageChoice() {
  var data = new FormData();
  var files = $("#pictureProfileFile").get(0).files;

  if (files.length > 0) {
    for (let f = 0; f < files.length; f++) {
      data.append("files", files[f]);
    }
  } else {
    showDiscretePopup("Error", "No files selected.");
    return;
  }

  let api = `${BASE_URL}/Upload`;

  $.ajax({
    type: "POST",
    url: api,
    contentType: false,
    processData: false,
    data: data,
    success: uploadImageChoiceSCB,
    error: uploadImageChoiceECB,
  });
}
function uploadImageChoiceSCB(data) {
  if (data.length > 0) {
    // Use the response to set the image URL or identifier
    let uploadedImageUrl =
      `https://proj.ruppin.ac.il/cgroup79/test2/tar1/images/` + data;
    uploadProfilePicture(uploadedImageUrl);
  } else {
    showDiscretePopup(
      "Error",
      "Unable to upload image. Check your internet connection.",
    );
  }
}
function uploadImageChoiceECB(data) {
  showInvasivePopup(
    "x",
    "Error",
    "Unable to upload image. Check your internet connection or submit a bug report. " +
      data,
  );
}

function uploadProfilePicture(uploadedImageUrl) {
  let api = `${BASE_URL}/BookStoreUser/ChangeUserImage`;
  let data = {
    UserID: userActive.userID,
    UserImage: uploadedImageUrl,
  };

  ajaxCallPromise("PUT", api, JSON.stringify(data))
    .then(function (response) {
      if (response) {
        showDiscretePopup("Success", "Image changed successfully!");
        updateUserImage(uploadedImageUrl);
      } else {
        showInvasivePopup(
          "x",
          "Error",
          "Image change failed. Please try again. Check your internet connection.",
        );
      }
    })
    .catch(function (error) {
      showInvasivePopup(
        "x",
        "Error",
        "An error has occured uploading a profile picture. Check your internet connection or submit a bug report. " +
          error,
      );
    });
}

function updateUserImage(uploadedImageUrl) {
  // Function to update the user image in storage
  function updateStorage(storage) {
    if (storage && storage.getItem("user")) {
      // Parse the stored user object
      let user = JSON.parse(storage.getItem("user"));
      // Update the user's image property
      user.image = uploadedImageUrl;
      // Save the updated user object back to storage
      storage.setItem("user", JSON.stringify(user));
      return user.image; // Return the updated image URL
    }
    return null;
  }

  // Check and update localStorage
  let updatedImageUrl = updateStorage(localStorage);
  // Check and update sessionStorage if not found in localStorage
  if (!updatedImageUrl) {
    updatedImageUrl = updateStorage(sessionStorage);
  }
  // Fallback to default image if not found in either storage
  if (!updatedImageUrl) {
    updatedImageUrl =
      "https://static.vecteezy.com/system/resources/thumbnails/022/014/063/small_2x/missing-picture-page-for-website-design-or-mobile-app-design-no-image-available-icon-vector.jpg";
  }

  // Update the user's image on the page
  document.getElementsByClassName("profilepic")[0].src = updatedImageUrl;
}

const quill = new Quill("#quillSignatureEditor", {
  theme: "snow",
  modules: {
    toolbar: [
      ["bold", "italic", "underline"],
      [{ list: "ordered" }, { list: "bullet" }],
      ["clean"], // Remove formatting button
    ],
  },
});

if (userActive.userSignature) {
  quill.root.innerHTML = userActive.userSignature;
}

const maxCharacters = 200;
const maxLineBreaks = 5;
const maxConsecutiveLineBreaks = 3;
const maxConsecutiveEmptyParagraphs = 2;

$("#UserChangeSignatureForm").submit(function (event) {
  event.preventDefault();

  let signature = quill.getText().trim();
  let htmlSignature = quill.root.innerHTML.trim();

  // Check character limit
  if (signature.length > maxCharacters) {
    showDiscretePopup("Error", "Signature must be less than 200 characters.");
    return;
  }
  if (signature.length === 0) {
    showDiscretePopup("Error", "Signature cannot be empty.");
    return;
  }

  // Reduce consecutive <br> to maxConsecutiveLineBreaks
  htmlSignature = htmlSignature.replace(/(<br\s*\/?>\s*){4,}/g, "<br><br>");

  // Reduce consecutive <p><br></p> to maxConsecutiveEmptyParagraphs
  htmlSignature = htmlSignature.replace(
    /(<p><br><\/p>\s*){3,}/g,
    "<p><br></p><p><br></p>",
  );

  // Count total line breaks and empty paragraphs
  const totalLineBreaks = (htmlSignature.match(/<br\s*\/?>/g) || []).length;
  const totalEmptyParagraphs = (htmlSignature.match(/<p><br><\/p>/g) || [])
    .length;

  // Total number of breaks including both line breaks and empty paragraphs
  const totalBreaks = totalLineBreaks + totalEmptyParagraphs;

  if (totalBreaks > maxLineBreaks) {
    showDiscretePopup(
      "Error",
      `Your signature cannot exceed ${maxLineBreaks} combined line breaks and empty paragraphs. Please adjust your content accordingly.`,
    );
    return;
  }

  // Proceed with submitting the modified content
  let api = `${BASE_URL}/BookStoreUser/ChangeUserSignature`;
  let data = {
    UserID: userActive.userID,
    UserSignature: htmlSignature, // Use the modified signature here
  };

  ajaxCallPromise("PUT", api, JSON.stringify(data))
    .then(function (response) {
      if (response) {
        updateUserSignature(htmlSignature);
      } else {
        showDiscretePopup(
          "Error",
          "Signature change failed. Please check your internet connection and try again.",
        );
      }
    })
    .catch(function (error) {
      showInvasivePopup(
        "x",
        "Error",
        "Error changing signature. Please check your internet connection and try again, or submit a bug report. " +
          error,
      );
    });
});

function updateUserSignature(signature) {
  function updateStorage(storage) {
    if (storage && storage.getItem("user")) {
      let user = JSON.parse(storage.getItem("user"));
      user.userSignature = signature;
      storage.setItem("user", JSON.stringify(user));
      return user.userSignature;
    }
    return null;
  }

  let updatedSignature = updateStorage(localStorage);
  if (!updatedSignature) {
    updatedSignature = updateStorage(sessionStorage);
  }

  if (updatedSignature) {
    showInvasivePopup("v", "Success", "Signature changed successfully!");
    $("#UserSignature").html(`Signature: <br>${updatedSignature}`);
    quill.setText("");
  } else {
    $("#UserSignature").text("Signature not available.");
  }
}
