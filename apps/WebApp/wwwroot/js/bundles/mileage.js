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

/**
 * Setup date picker defaults
 *
 */
function setupDatepickerDefaults() {
	$.fn.datepicker.defaults.autoclose = true;
	$.fn.datepicker.defaults.todayBtn = "linked";
	$.fn.datepicker.defaults.format = "yyyy-mm-dd";
}
ready(setupDatepickerDefaults);

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
function openCreateModal(url, replaceId) {
	openModal("#create", url, replaceId, true);
}

/**
 * Open delete modals when delete buttons are clicked.
 *
 */
function setupCreateModalOpen() {
	$("body").on("click", ".btn-delete-check", function (e) {
		checkDeleteItem($(this), e);
	});
}
ready(setupCreateModalOpen);

/**
 * Submit modal delete form when the delete button is pressed.
 *
 */
function setupCreateModalSave() {
	$("body").on("click", "#delete .btn-delete", function () {
		$("#delete form").submit();
		modal.hide();
	});
}
ready(setupCreateModalSave);

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
 * Open delete modals when delete buttons are clicked.
 *
 */
function setupDeleteModalOpen() {
	$("body").on("click", ".btn-delete-check", function (e) {
		checkDeleteItem($(this), e);
	});
}
ready(setupDeleteModalOpen);

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
 * Open the update modal.
 * 
 * @param {any} url
 * @param {any} replaceId
 */
function openUpdateModal(url, replaceId) {
	openModal("#update", url, replaceId, false, () => setupUpdateModalSearch());
}

/**
 * Filter list elements based on search field entry.
 *
 */
function setupUpdateModalSearch() {
	// hide add item button
	var addItem = $("#update .btn-add");
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
 * Submit update modal form when the save button is pressed.
 *
 */
function setupUpdateModalSave() {
	// submit function
	var submit = function (e) {
		e.preventDefault();
		$("#update form").submit();
		modal.hide();
	}

	// submit on button click, auto-save input change, and enter
	$("body").on("click", "#update .btn-save", (e) => submit(e));
	$("body").on("change", "#update .auto-save", (e) => submit(e));
	$("body").on("keypress", "#update input[type='text']", function (e) {
		if (e.keyCode == 13) {
			submit(e);
		}
	})
}
ready(setupUpdateModalSave);

/**
 * Setup token links to open the update modal when clicked.
 *
 */
function setupTokenUpdateModals() {
	$("body").on("click", ".token > a", function () {
		var updateUrl = $(this).data("update");
		var replaceId = $(this).data("replace");
		openUpdateModal(updateUrl, replaceId);
	});
}
ready(setupTokenUpdateModals);

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
	var cls = "btn-delete-check";
	if ($(e.target).hasClass(cls) || $(e.target).parents("." + cls).length > 0) {
		return;
	}

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
 * Select an input when HTML is loaded.
 *
 */
function selectInputOnLoad() {
	$(".select-on-load").select();
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
