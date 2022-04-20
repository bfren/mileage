const alertIcons = {
	close: $("<i/>").addClass("fa-solid fa-xmark"),
	info: $("<i/>").addClass("fa-solid fa-circle-info"),
	success: $("<i/>").addClass("fa-solid fa-check"),
	warning: $("<i/>").addClass("fa-solid fa-triangle-exclamation"),
	error: $("<i/>").addClass("fa-solid fa-ban"),
	edit: $("<i/>").addClass("fa-solid fa-circle-plus"),
	delete: $("<i/>").addClass("fa-solid fa-circle-minus"),
	complete: $("<i/>").addClass("fa-solid fa-circle-check"),
	save: $("<i/>").addClass("fa-solid fa-ban")
}

const message = $(".statusbar .message");
var alertTimeout = 0;

/**
 * Show an alert.
 * 
 * @param {string} type Alert type - see alertIcons for valid values
 * @param {string} text Alert text
 */
function showAlert(type, text, sticky) {
	// set alert values
	message.find(".icon").html(alertIcons[type]);
	message.find(".text").text(text);
	message.find(".countdown").text("");
	//message.find(".manual").html(alertIcons["close"]);

	// show alert and clear any existing timeouts
	message.show();
	clearTimeout(alertTimeout);

	// make error alerts sticky
	if (type == "error" || sticky) {
		return;
	}

	// start countdown to hide other alerts automatically
	updateAlert(5);
}

/**
 * Show a sticky 'Please wait' alert
 *
 */
function showPleaseWaitAlert() {
	showAlert("info", "Please wait...", true);
}

/**
 * Update the current alert - remove automatically when seconds gets to 0.
 * 
 * @param {number} seconds
 */
function updateAlert(seconds) {
	if (seconds == 0) {
		closeAlert();
		return;
	}

	message.find(".countdown").text(seconds);
	alertTimeout = setTimeout(() => updateAlert(seconds - 1), 1000);
}

/**
 * Close the alert.
 * 
 */
function closeAlert() {
	message.fadeOut();
}
ready(() => message.click(() => closeAlert()));

/**
 * Show any alerts on page load.
 * 
 */
function showAlertsOnLoad() {
}
ready(showAlertsOnLoad);

/**
 * Setup token links to open the edit modal when clicked.
 *
 */
function setupTokenModals() {
	$("body").on("click", ".token > a", function () {
		var editUrl = $(this).data("edit");
		var replaceId = $(this).data("replace");
		openModal(editUrl, replaceId);
	});
}
ready(setupTokenModals);

var modal;

/**
 * Open the edit modal and load contents via HTTP.
 * 
 * @param {any} url
 * @param {any} replaceId
 */
function openModal(url, replaceId) {
	// show messages
	showPleaseWaitAlert();
	console.log("Loading modal from: " + url);

	// load modal HTML and then show modal
	$("#edit").load(url, function () {
		// save replaceId
		var form = $(this).find("form");
		form.attr("data-replace", replaceId);

		// create the modal object
		var wrapper = $(this);
		var modalId = $(this).find(".modal").attr("id");
		var modalEl = document.getElementById(modalId)

		// select item when modal is opened
		modalEl.addEventListener("shown.bs.modal", () => $(".modal-select").focus());

		// fade out background when modal is closed
		modalEl.addEventListener("hide.bs.modal", () => wrapper.fadeOut("fast"));

		// fade in background and show modal
		wrapper.fadeIn("fast", () => closeAlert());

		// create and show modal
		modal = new bootstrap.Modal(modalEl);
		modal.show();

		// Setup search behaviour
		setupModalSearch();
	});
}

/**
 * Filter list elements based on search field entry.
 *
 */
function setupModalSearch() {
	// hide add item button
	var addItem = $("#edit .btn-add");
	addItem.hide();

	// save new item on enter
	$("#list-filter").keydown(function (e) {
		if (e.keyCode == 13) {
			e.preventDefault();
			addItem.click();
		}
	});

	// filter as the user types
	$("#list-filter").keyup(function () {
		// get value from input
		var value = $(this).val();

		// show add item button
		if (value) {
			addItem.text("Add: " + value).show();
		} else {
			addItem.text("").hide();
		}

		// filter items that match the input value
		$("#list-items .list-item").filter(function () {
			var show = $(this).data("text").indexOf(value.toLowerCase()) > -1;
			$(this).toggle(show);
		});
	});
}

/**
 * Submit modal edit form when the save button is pressed.
 *
 */
function setupModalSave() {
	$("body").on("click", "#edit .btn-save", function () {
		$("#edit form").submit();
		modal.hide();
	});
}
ready(setupModalSave);

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

/**
 * Setup tab buttons to load settings on click
 *
 */
function setupSettingsTabs() {
	$("#settingsButtons .nav-link").click(function () {
		var tabId = $(this).data("bs-target");
		loadSettingsTab(tabId);
	});
}
ready(setupSettingsTabs);

/**
 * Load a settings tab
 * 
 * @param {any} tabId
 */
function loadSettingsTab(tabId) {
	// get tab target
	var tab = $(tabId);

	// get tab source URL
	var src = tab.data("src");

	// load source
	showPleaseWaitAlert()
	tab.load(src, () => closeAlert());
}

function loadSaveForm(wrapper, el, e) {
	// don't do whatever the link / button was going to do
	e.preventDefault();

	// get the URL to load
	var url = el.data("load");

	// show alert and load URL
	showPleaseWaitAlert();
	$("#save-" + wrapper).load(url, () => closeAlert());
}

/**
 * Submit all forms via AJAX and handle results
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

		// post data and handle result
		$.ajax({ url: form.attr("action"), method: "POST", data: form.serialize() })

			.done(function (data) {
				// we are expecting HTML to replace an object
				if (replaceId) {
					// there is some HTML to use
					if (data) {
						$("#" + replaceId).replaceWith(data);
						showAlert("success", "Saved.");
						setupTokenModals();
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
