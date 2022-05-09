const alertIcons = {
	close: $("<i/>").addClass("fa-solid fa-xmark"),
	info: $("<i/>").addClass("fa-solid fa-circle-info"),
	success: $("<i/>").addClass("fa-solid fa-check"),
	warning: $("<i/>").addClass("fa-solid fa-triangle-exclamation"),
	error: $("<i/>").addClass("fa-solid fa-ban"),
	update: $("<i/>").addClass("fa-solid fa-circle-plus"),
	delete: $("<i/>").addClass("fa-solid fa-circle-minus"),
	complete: $("<i/>").addClass("fa-solid fa-circle-check"),
	save: $("<i/>").addClass("fa-solid fa-ban")
}

const alertTypes = {
	info: "info",
	success: "success",
	warning: "warning",
	error: "error"
}

const message = $(".statusbar .message");
const alertCountdown = 3;
var alertTimeout = 0;

/**
 * Returns true if there is currently a visible message.
 *
 */
function isAlert() {
	return message.is(":visible");
}

/**
 * Returns true if there is currently a visible message and it's sticky.
 *
 */
function isAlertSticky() {
	return isAlert() && message.data("sticky");
}

/**
 * Show an alert.
 * 
 * @param {string} type Alert type - see alertIcons for valid values
 * @param {string} text Alert text
 * @param {boolean} sticky If true the alert will not be automatically closed
 */
function showAlert(type, text, sticky) {
	if (type == alertTypes.info && isAlertSticky()) {
		return;
	}

	// set alert values
	message.find(".icon").html(alertIcons[type]);
	message.find(".text").text(text);
	message.find(".countdown").text("");
	message.data("type", type);
	message.data("sticky", sticky ? "true" : "false");

	// show alert and clear any existing timeouts
	message.stop().show();
	clearTimeout(alertTimeout);

	// make error alerts sticky
	if (type == alertTypes.error || sticky) {
		message.find(".close").show();
		return;
	}

	// start countdown to hide other alerts automatically
	message.find(".close").hide();
	updateAlert(alertCountdown);
}

/**
 * Update the current alert - remove automatically when seconds gets to 0.
 * 
 * @param {number} seconds
 */
function updateAlert(seconds) {
	if (seconds == 0) {
		closeAlert(true);
		return;
	}

	message.find(".countdown").text(seconds);
	alertTimeout = setTimeout(() => updateAlert(seconds - 1), 1000);
}

/**
 * Close the alert, if it is an info alert - or force is true.
 * 
 * @param {string} force If true the alert will be closed whatever type it is
 */
function closeAlert(force) {
	if (force || message.data("type") == alertTypes.info) {
		message.fadeOut();
	}
}
ready(() => message.click(
	() => closeAlert(true)
));
