/**
 * Load the home page.
 * 
 */
function loadHome() {
	loadPage(home);
}

/**
 * Load a page into the main content block.
 * 
 * @param {any} url
 */
function loadPage(url) {
	if (getHash() == url) {
		loadHash();
	} else {
		window.location.hash = url;
	}
}

/**
 * Open whatever is in the URL hash.
 *
 */
function loadHash() {
	// get hash
	var url = getHash();
	if (!url || url.length == 2) {
		url = home;
	}

	// get URL contents
	setupAjaxAuth();
	$.ajax(
		{
			url: url,
			method: "GET"
		})

		.done(function (data, status, xhr) {
			// close info alert
			closeAlert();

			// handle JSON response
			if (xhr.responseJSON) {
				handleResult(xhr.responseJSON);
				return;
			}

			// replace HTML
			$("#content").html(data);

			// scroll to top of page
			window.scrollTo(0, 0);
		})

		.fail(function (xhr) {
			// close info alert
			closeAlert();

			// handle unauthorised
			if (xhr.status == 401) {
				setupAjaxAuth();
				$("#content").load(signIn);
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
ready(loadHash);
window.onhashchange = loadHash;

/**
 * Get the current window hash without the actual #.
 *
 */
function getHash() {
	return window.location.hash.replace("#", "");
}

/**
 * Capture link clicks and use AJAX to load pages.
 *
 */
function setupLinks() {
	$("body").on("click", "a", function (e) {
		// get hyperlink
		var href = $(this).attr("href");

		// ignore absolute links
		if (href.startsWith("http")) {
			return;
		}

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
