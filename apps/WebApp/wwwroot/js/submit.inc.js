/**
 * Submit all forms via AJAX and handle results.
 *
 */
function setupAjaxSubmit() {
	$("body").on("submit", "form", function (e) {
		// stop default submit behaviour
		e.preventDefault();

		// check validity
		var form = $(this);
		if (this.checkValidity() === false) {
			form.find(":input:visible").not("[formnovalidate]")
				.parent().addClass("was-validated");
			return;
		}

		// support GET requests
		if (form.attr("method").toLowerCase() == "get") {
			var data = form.serialize();
			var url = form.attr("action") + "?" + data;
			return loadPage(url);
		}

		// submit form
		submitForm(form);
	});
}
ready(setupAjaxSubmit);

/**
 * Close a modal and submit a form, optionally overriding URL and data
 * 
 * @param {JQuery<any>} form Form to submit
 * @param {string} url Override form action and submit to this URL instead
 * @param {any} data Override form data and submit this data instead
 */
function submitForm(form, url, data) {
	// get form info
	var method = form.attr("method") ?? "POST";
	var replaceId = form.data("replace");
	var replaceContents = form.data("replace-contents");

	// hide modal
	if (modal) {
		modal.hide();
	}

	// post data and handle result
	setupAjaxAuth();
	$.ajax(
		{
			method: method,
			url: url || form.attr("action"),
			data: data || form.serialize()
		})

		.done(function (data, status, xhr) {
			// handle JSON response
			if (xhr.responseJSON) {
				handleResult(xhr.responseJSON);
				return;
			}

			// handle HTML response
			if (data && replaceId) {
				// get the DOM element to be replaced
				var replace = $("#" + replaceId);

				// replace contents or the element itself
				if (replaceContents) {
					replace.html(data);
				} else {
					replace.replaceWith(data);
				}

				// show alert
				if (method == "POST") {
					showAlert(alertTypes.success, "Done.");
				}

				return;
			}

			// something unexpected has happened
			showAlert(alertTypes.warning, "Something went wrong, refreshing the page.", true);
			loadHash();
		})

		.fail(function (xhr) {
			// the response is a JSON result
			if (xhr && xhr.responseJSON) {
				handleResult(xhr.responseJSON);
				return;
			}

			// something else has gone wrong
			showAlert(alertTypes.error, "Something went wrong, please try again.");
		});
}
