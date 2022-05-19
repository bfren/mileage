/**
 * Handle a JSON Result object.
 * 
 * @param {any} r
 */
function handleResult(r) {
	// show alert
	var sticky = r.message.type == alertTypes.warning || r.message.type == alertTypes.error;
	showAlert(r.message.type, r.message.text, sticky);

	// if message is sign in, set JWT and load home page
	if (r.message.text == "You were signed in." && r.value) {
		setAuth(r.value);
		return loadPage(home);
	}

	// redirect if value is a URL	
	if (r.value) {
		if (r.value == "/") {
			return loadPage(home);
		} else if (r.value == "refresh") {
			return loadHash();
		} else if (v.value.startsWith("/")) {
			return loadPage(r.redirectTo);
		}
	}
}
