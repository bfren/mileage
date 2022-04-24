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
	$(selector).load(url, function (response, status, xhr) {
		// handle unauthorised
		if (xhr.status == 401) {
			showAlert(alertTypes.error, "You are not authorised to do this, please sign in.");
		}

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
		modalEl.addEventListener("shown.bs.modal", () => {
			var s = $(".modal-select");
			if (s.length == 1) {
				s.select();
			} else {
				s.first().select();
			}
		});

		// fade out background when modal is closed
		modalEl.addEventListener("hide.bs.modal", () => wrapper.fadeOut("fast"));

		// fade in background
		wrapper.fadeIn("fast", () => closeAlert());

		// close modal when wrapped is clicked
		$("body").on("click", (e) => {
			if ($(e.target).is(".modal")) {
				modal.hide();
			}
		});

		// create and show modal
		modal = new bootstrap.Modal(modalEl);
		modal.show();

		// Setup additional behaviour
		if (setup) {
			setup();
		}
	});
}
