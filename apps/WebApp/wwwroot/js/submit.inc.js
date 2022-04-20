/**
 * Submit all forms via AJAX and handle results.
 *
 */
function setupAjaxSubmit() {
	$("body").on("submit", "form", function (e) {
		// stop default submit
		e.preventDefault();

		// show info message
		showPleaseWaitAlert();

		// get form info
		var form = $(this);
		var replaceId = form.data("replace");
		var replaceContents = form.data("replace-contents");

		// post data and handle result
		$.ajax({ url: form.attr("action"), method: "POST", data: form.serialize() })

			.done(function (data) {
				// we are expecting HTML to replace an object
				if (replaceId) {
					// there is some HTML to use
					if (data) {
						var replace = $("#" + replaceId);
						if (replaceContents) {
							replace.html(data);
						} else {
							replace.replaceWith(data);
						}
						showAlert("success", "Success.");
						return;
					}

					// there is no HTML to use
					showAlert("error", "Unable to save, please try again.");
					return;
				}

				// handle a JSON result object
				if (data) {
					handleResult(data);
				}
			})

			.fail(function (error) {
				// the response is a JSON result
				if (error && error.responseJSON) {
					handleResult(error.responseJSON);
					return;
				}

				// something else has gone wrong
				showAlert("error", "Something went wrong, please try again.");
			});
	});
}
ready(setupAjaxSubmit);
