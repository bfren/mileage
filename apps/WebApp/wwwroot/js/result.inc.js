/**
 * Handle a JSON Result object
 * 
 * @param {any} r
 */
function handleResult(r) {
	// show alert
	showAlert(r.message.type, r.message.text);

	// redirect
	if (r.redirectTo) {
		showAlert("info", "Redirecting...", true);
		window.location.href = r.redirectTo;
	}
}
