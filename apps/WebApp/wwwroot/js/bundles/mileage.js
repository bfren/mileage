const alertIcons = {
  close: $("<i/>").addClass("fa-solid fa-xmark"),
  info: $("<i/>").addClass("fa-solid fa-circle-info"),
  success: $("<i/>").addClass("fa-solid fa-check"),
  warning: $("<i/>").addClass("fa-solid fa-triangle-exclamation"),
  error: $("<i/>").addClass("fa-solid fa-ban"),
  edit: $("<i/>").addClass("fa-solid fa-circle-plus"),
  delete: $("<i/>").addClass("fa-solid fa-circle-minus"),
  complete: $("<i/>").addClass("fa-solid fa-circle-check"),
  save: $("<i/>").addClass("fa-solid fa-ban")
}

const message = $(".statusbar .message");
var alertTimeout = 0;

/**
 * Show an alert.
 * 
 * @param {string} type Alert type - see alertIcons for valid values
 * @param {string} text Alert text
 */
function showAlert(type, text) {
  // set alert values
  message.find(".icon").html(alertIcons[type]);
  message.find(".text").text(text);
  message.find(".countdown").text("");
  //message.find(".manual").html(alertIcons["close"]);

  // show alert and clear any existing timeouts
  message.show();
  clearTimeout(alertTimeout);

  // make error alerts sticky
  if (type == "error") {
    return;
  }

  // start countdown to hide other alerts automatically
  updateAlert(5);
}

/**
 * Update the current alert - remove automatically when seconds gets to 0.
 * 
 * @param {number} seconds
 */
function updateAlert(seconds) {
  if (seconds == 0) {
    closeAlert();
    return;
  }

  message.find(".countdown").text(seconds);
  alertTimeout = setTimeout(() => updateAlert(seconds - 1), 1000);
}

/**
 * Close the alert.
 * 
 */
function closeAlert() {
  message.slideUp();
}

ready(function () {
  message.click(function () {
    closeAlert();
  });
})

/**
 * Show any alerts on page load.
 * 
 */
function showAlertsOnLoad() {
}

ready(showAlertsOnLoad);

function handleResult(r) {
  // show message and pass value to success function
  showAlert(r.message.type, r.message.text);

  // redirect
  if (r.redirectTo) {
    showAlert("info", "Redirecting, please wait...");
    window.location.href = r.redirectTo;
  }
}

function setupAjaxSubmit() {
  $("form").submit(function (e) {
    // stop default submit
    e.preventDefault();

    // show info message
    showAlert("info", "Please wait...");

    // get form info
    var form = $(this);
    var url = form.attr("action");
    var data = form.serialize();

    // post data and handle result
    $.ajax({
      url: url,
      method: "POST",
      data: data,
      dataType: "json"
    }).done(
      function (data) {
        handleResult(data);
      }
    ).fail(
      function (error) {
        handleResult(error.responseJSON);
      }
    );
  });
}
ready(setupAjaxSubmit);
