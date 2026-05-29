export function ajaxCall(method, api, data, successCB, errorCB) {
  $.ajax({
    type: method,
    url: api,
    data: data,
    cache: false,
    contentType: "application/json",
    dataType: "json",
    success: successCB,
    error: function (jqXHR, textStatus, errorThrown) {
      console.error("AJAX error: ", textStatus, errorThrown);
      console.log("Response Text: ", jqXHR.responseText);
      if (errorCB) {
        errorCB(jqXHR, textStatus, errorThrown);
      }
    },
  });
}

export function ajaxCallPromise(method, api, data) {
  // Instead of chaining ajaxcalls, we can chain promises using then and catch while still utilizing ajaxcalls.
  return new Promise((resolve, reject) => {
    // We start by creating a promise and housing an ajaxcall inside it. upon success, we get the data we asked for and move on.
    $.ajax({
      // We use the data we got for our uses inside a function ( or set a value in localstorage/sessionstorage)
      type: method, // But if we fail, we get a reject to the promise with the errors of the ajaxcall.
      url: api, // This way we only need to provide method ( "GET", "POST", etc.. ), the api and the data.
      data: data,
      cache: false,
      contentType: "application/json",
      dataType: "json",
      success: resolve,
      error: function (jqXHR, textStatus, errorThrown) {
        console.error("AJAX error: ", textStatus, errorThrown);
        console.log("Response Text: ", jqXHR.responseText);
        reject(jqXHR, textStatus, errorThrown);
      },
    });
  });
}
