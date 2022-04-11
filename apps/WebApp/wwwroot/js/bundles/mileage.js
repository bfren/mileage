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
  message.fadeOut();
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



function setupTokenModals() {
  $(".token > a").click(function () {
    var editUrl = $(this).data("edit");
    var replaceId = $(this).data("replace");
    openModal(editUrl, replaceId);
  });
}
ready(setupTokenModals);

function openModal(url, replaceId) {
  // show messages
  showAlert("info", "Please wait...");
  console.log("Loading modal from: " + url);

  // load modal HTML and then show modal
  $("#edit").load(url, function () {
    // save replaceId
    var form = $(this).find("form");
    form.attr("data-replace", replaceId);

    // close an alert
    closeAlert();

    // create and show the modal
    var edit = document.getElementById("edit-modal");
    var modal = new bootstrap.Modal(edit);
    modal.show();

    // setup save button and ajax submit
    setupModalSave();
    setupAjaxSubmit();
  });
}

function setupModalSave() {
  $("#edit .btn-save").click(function () {
    $("#edit form").submit();
  });
}

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
    var replaceId = form.data("replace");
    var data = form.serialize();

    // post data and handle result
    $.ajax({
      url: url,
      method: "POST",
      data: data
    }).done(function (data) {
      if (replaceId) { // the response is expecting HTML to replace an object
        if (data) { // there is some HTML to use
          $("#" + replaceId).replaceWith(data);
          showAlert("success", "Saved.");
          setupTokenModals();
        } else { // there is no HTML to use
          showAlert("error", "Unable to save, please try again.");
        }
      } else if (data) { // the response is expecting a JSON result
        handleResult(data);
      }
    }).fail(function (error) {
      if (error && error.responseJSON) { // the response is a JSON result
        handleResult(error.responseJSON);
      } else { // something else has gone wrong
        showAlert("error", "Something went wrong, please try again.");
      }
    });
  });
}
ready(setupAjaxSubmit);
