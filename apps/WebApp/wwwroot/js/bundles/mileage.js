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

const alertTypes = {
	info: "info",
	success: "success",
	warning: "warning",
	error: "error"
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
	if (type == alertTypes.error || sticky) {
		return;
	}

	// start countdown to hide other alerts automatically
	updateAlert(5);
}

/**
 * Show a sticky 'Please wait' alert.
 *
 */
function showPleaseWaitAlert() {
	showAlert(alertTypes.info, "Please wait...", true);
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

function setupDatepickers() {
	$.fn.datepicker.defaults.todayBtn = "linked";
	$.fn.datepicker.defaults.format = "yyyy-mm-dd";
}
ready(setupDatepickers);

var modal;

/**
 * Open a modal and load contents via HTTP.
 * 
 * @param {any} url
 * @param {any} replaceId
 */
function openModal(selector, url, replaceId, replaceContents, setup) {
	// show messages
	showPleaseWaitAlert();

	// load modal HTML and then show modal
	$(selector).load(url, function () {
		// save replaceId
		var form = $(this).find("form");
		form.attr("data-replace", replaceId);
		form.attr("data-replace-contents", replaceContents);

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

		// Setup additional behaviour
		if (setup) {
			setup();
		}
	});
}

/**
 * Open the delete modal.
 * 
 * @param {any} url
 * @param {any} replaceId
 */
function openDeleteModal(url, replaceId) {
	openModal("#delete", url, replaceId, true);
}

/**
 * Submit modal delete form when the delete button is pressed.
 *
 */
function setupDeleteModalSave() {
	$("body").on("click", "#delete .btn-delete", function () {
		$("#delete form").submit();
		modal.hide();
	});
}
ready(setupDeleteModalSave);

/**
 * Open the edit modal.
 * 
 * @param {any} url
 * @param {any} replaceId
 */
function openEditModal(url, replaceId) {
	openModal("#edit", url, replaceId, false, () => setupEditModalSearch());
}

/**
 * Filter list elements based on search field entry.
 *
 */
function setupEditModalSearch() {
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
function setupEditModalSave() {
	$("body").on("click", "#edit .btn-save", function () {
		$("#edit form").submit();
		modal.hide();
	});
}
ready(setupEditModalSave);

/**
 * Setup token links to open the edit modal when clicked.
 *
 */
function setupTokenEditModals() {
	$("body").on("click", ".token > a", function () {
		var editUrl = $(this).data("edit");
		var replaceId = $(this).data("replace");
		openEditModal(editUrl, replaceId);
	});
}
ready(setupTokenEditModals);

/**
 * Handle a JSON Result object.
 * 
 * @param {any} r
 */
function handleResult(r) {
	// show alert
	showAlert(r.message.type, r.message.text);

	// redirect
	if (r.redirectTo) {
		showAlert(alertTypes.info, "Redirecting...", true);
		window.location.href = r.redirectTo;
	}
}

/**
 * Setup tab buttons to load settings on click.
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
 * Load a settings tab.
 * 
 * @param {any} tabId
 */
function loadSettingsTab(tabId) {
	// get tab target
	var tab = $(tabId);

	// show loading alerts
	tab.html($("<div/>").text("Loading..."));
	showPleaseWaitAlert();

	// load source
	var src = tab.data("src");
	tab.load(src, () => closeAlert());
}

/**
 * Load a save form for creating / editing items.
 *
 * @param {any} item The name of the item being saved
 * @param {any} el The element to be reloaded
 * @param {any} e The click event
 */
function loadSaveForm(item, el, e) {
	// don't do whatever the link / button was going to do
	e.preventDefault();

	// get the URL to load
	var url = el.data("load");

	// show alert and load URL
	showPleaseWaitAlert();
	$("#save-" + item).load(url, () => closeAlert());
}

/**
 * Reload the form when the cancel button is clicked.
 * 
 * @param {any} form Form selector
 * @param {any} item The name of the item being saved
 * @param {any} el The element to be reloaded
 */
function setupReloadFormOnCancel(form, item, el) {
	$(form).on("click", ".btn-cancel", function (e) {
		loadSaveForm(item, el, e);
	});
}

/**
 * Submit the form when the enter key is pressed.
 * 
 * @param {any} form Form selector
 */
function setupSaveFormOnEnter(form) {
	$(form).on("keydown", "input", function (e) {
		if (e.keyCode == 13) {
			e.preventDefault();
			$(form).find(".btn-save").click();
		}
	});
}

/**
 * Check whether or not the user really wants to delete an item.
 * 
 * @param {any} el The element being deleted
 * @param {any} e The click event
 */
function checkDeleteItem(el, e) {
	// don't do whatever the link / button was going to do
	e.preventDefault();

	// get info
	var deleteUrl = el.data("delete");
	var replaceId = el.data("replace");

	// open modal to check delete
	openDeleteModal(deleteUrl, replaceId);
}

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

			.done(function (data, status, xhr) {
				// handle JSON response
				if (xhr.responseJSON) {
					handleResult(xhr.responseJSON);
					return;
				}

				// handle HTML response
				if (data && replaceId) {
					// get the DOM element to be replaced
					var replace = $("#" + replaceId);

					// replace contents or the element itself
					if (replaceContents) {
						replace.html(data);
					} else {
						replace.replaceWith(data);
					}
					
					// show alert
					showAlert(alertTypes.success, "Done.");
					return;
				}

				// something unexpected has happened
				showAlert(alertTypes.warning, "Something went wrong, refreshing the page...", true);
				location.reload();
			})

			.fail(function (xhr) {
				// the response is a JSON result
				if (xhr && xhr.responseJSON) {
					handleResult(xhr.responseJSON);
					return;
				}

				// something else has gone wrong
				showAlert(alertTypes.error, "Something went wrong, please try again.");
			});
	});
}
ready(setupAjaxSubmit);
