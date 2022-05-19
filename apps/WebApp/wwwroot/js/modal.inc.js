var modal;

/**
 * Open a modal and load contents via HTTP.
 * 
 * @param {any} url
 * @param {any} replaceId
 */
function openModal(selector, url, replaceId, replaceContents, setup) {
	// load modal HTML and then show modal
	setupAjaxAuth();
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

			var f = $(".modal-focus");
			if (f.length == 1) {
				f.focus();
			} else {
				f.first().focus();
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

/**
 * Filter items in a modal based on a value.
 * 
 * @param {string} parentId Parent ID holding the list of items
 * @param {string} value Search value
 * @returns {boolean} Whether or not there is an exact (case-insensitive) match
 */
function filterModalItems(parentId, value) {
	$("#" + parentId + " .with-data").filter(function () {
		// get text values as lowercase for comparison
		var itemText = $(this).data("text").toString().toLowerCase();
		var searchText = value.toLowerCase();

		// if the item contains the search text, show it
		var show = itemText.indexOf(searchText) > -1;
		$(this).toggle(show);

		// return whether or not there is an exact match
		return itemText == searchText;
	});
}
