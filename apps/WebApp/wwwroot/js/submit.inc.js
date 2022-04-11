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
