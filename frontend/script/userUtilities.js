// Checking who's logged in, no matter remembered on not. If no one is - a Guest is born.

function checkAndSetUserStorage() {
  function isUserInStorage(storage) {
    const userString = storage.getItem("user");
    if (userString === null) {
      return false;
    }
    try {
      const user = JSON.parse(userString);
      return user !== null && Object.keys(user).length > 0;
    } catch (error) {
      return false;
    }
  }

  if (isUserInStorage(localStorage)) {
    return JSON.parse(localStorage.getItem("user"));
  }

  if (isUserInStorage(sessionStorage)) {
    return JSON.parse(sessionStorage.getItem("user"));
  }

  const defaultUser = {
    userID: 0,
    userName: "Guest",
    userEmail: "",
    dateCreated: "",
    image: "",
    isAdmin: false,
    signature: "",
  };
  sessionStorage.setItem("user", JSON.stringify(defaultUser));
  return defaultUser;
}

function userLoggedIn() {
  return checkAndSetUserStorage();
}

export { checkAndSetUserStorage, userLoggedIn };
