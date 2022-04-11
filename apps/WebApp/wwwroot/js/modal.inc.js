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
