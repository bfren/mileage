var modal;

/**
 * Open a modal and load contents via HTTP.
 * 
 * @param {any} url
 * @param {any} replaceId
 */
function openModal(selector, url, replaceId, replaceContents, setup) {
	// show messages
	showPleaseWaitAlert();

	// load modal HTML and then show modal
	$(selector).load(url, function () {
		// save replaceId
		var form = $(this).find("form");
		if (replaceId) {
			form.attr("data-replace", replaceId);
			form.attr("data-replace-contents", replaceContents);
		}

		// create the modal object
		var wrapper = $(this);
		var modalId = $(this).find(".modal").attr("id");
		var modalEl = document.getElementById(modalId)

		// select item when modal is opened
		modalEl.addEventListener("shown.bs.modal", () => $(".modal-select").focus());

		// fade out background when modal is closed
		modalEl.addEventListener("hide.bs.modal", () => wrapper.fadeOut("fast"));

		// fade in background and show modal
		wrapper.fadeIn("fast", () => closeAlert());

		// create and show modal
		modal = new bootstrap.Modal(modalEl);
		modal.show();

		// Setup additional behaviour
		if (setup) {
			setup();
		}
	});
}
