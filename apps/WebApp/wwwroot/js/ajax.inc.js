const auth = "jwt";

/**
 * Set authorization token using local storage for persistence.
 *
 * @param {any} token Token value.
 */
function setAuth(token) {
	if (token) {
		localStorage.setItem(auth, "Bearer " + token);
		setupAjaxAuth();
	} else {
		localStorage.clear();
	}
}

/**
 * Add authorization header to the next AJAX request.
 *
 */
function setupAjaxAuth() {
	$.ajaxSetup({
		beforeSend: (xhr) => {
			xhr.setRequestHeader("Authorization", localStorage.getItem(auth))
		}
	});
}
