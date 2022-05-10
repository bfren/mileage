/**
 * Handle a JSON Result object.
 * 
 * @param {any} r
 */
function handleResult(r) {
	// show alert
	var sticky = r.message.type == alertTypes.warning || r.message.type == alertTypes.error;
	showAlert(r.message.type, r.message.text, sticky);

	// redirect or show alert
	if (r.redirectTo) {
		if (r.redirectTo == "/") {
			loadPage(home);
		} else if (r.redirectTo == "refresh") {
			loadHash();
		} else {
			loadPage(r.redirectTo);
		}
	}
}
