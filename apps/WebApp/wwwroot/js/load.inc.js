/**
 * Load a page into the main content block.
 * 
 * @param {any} url
 */
function loadPage(url) {
	showAlert(alertTypes.info, "Loading page, please wait...", true);
	$.ajax({ url: url, method: "GET" })

		.done(function (data, status, xhr) {
			// close loading alert
			closeAlert();

			// handle JSON response
			if (xhr.responseJSON) {
				handleResult(xhr.responseJSON);
				return;
			}

			// replace HTML
			$("#content").html(data);
		})

		.fail(function (xhr) {
			// close loading alert
			closeAlert();

			// handle unauthorised
			if (xhr.status == 401) {
				$("#content").load("/Auth/SignIn");
				return;
			}

			// the response is a JSON result
			if (xhr && xhr.responseJSON) {
				handleResult(xhr.responseJSON);
				return;
			}

			// something else has gone wrong
			showAlert(alertTypes.error, "Something went wrong, please try again.");
		});;
}

/**
 * Capture link clicks and use AJAX to load pages.
 *
 */
function setupLinks() {
	$("body").on("click", "a", function (e) {
		// get hyperlink
		var href = $(this).attr("href");

		// ignore javascript links
		if (href == "javascript:void(0)") {
			return;
		}

		// stop default behaviour
		e.preventDefault();

		// load URL
		loadPage(href);
	})
}
ready(setupLinks);
