/**
 * Handle a JSON Result object.
 * 
 * @param {any} r
 */
function handleResult(r) {
	// redirect or show alert
	if (r.redirectTo) {
		loadPage(r.redirectTo);
	} else {
		// show alert
		showAlert(r.message.type, r.message.text);
	}
}
