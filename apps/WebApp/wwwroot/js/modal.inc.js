function setupTokenModals() {
	$(".token > a").click(function () {
		var editUrl = $(this).data("edit");
		var replaceId = $(this).data("replace");
		openModal(editUrl, replaceId);
	});
}
ready(setupTokenModals);

var modal;

function openModal(url, replaceId) {
	// show messages
	showPleaseWaitAlert();
	console.log("Loading modal from: " + url);

	// load modal HTML and then show modal
	$("#edit").load(url, function () {
		// save replaceId
		var form = $(this).find("form");
		form.attr("data-replace", replaceId);

		// create the modal object
		var wrapper = $(this);
		var modalId = $(this).find(".modal").attr("id");

		var modalEl = document.getElementById(modalId)
		modal = new bootstrap.Modal(modalEl);

		// fade in background and show modal
		wrapper.fadeIn("fast", function () {
			closeAlert();
		});
		modal.show();

		// setup save button and ajax submit
		setupModalSave();
		setupAjaxSubmit();

		// fade out background when modal is closed
		modalEl.addEventListener("hide.bs.modal", function () {
			wrapper.fadeOut("fast");
		});
	});
}

function setupModalSave() {
	$("#edit .btn-save").click(function () {
		$("#edit form").submit();
		modal.hide();
	});
}
