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

/**
 * Load a page into the main content block.
 * 
 * @param {any} url
 */
function loadPage(url) {
	showAlert(alertTypes.info, "Loading page, please wait...", true);
	window.location.hash = url;

	$.ajax({ url: url, method: "GET" })

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
				$("#content").load("/Auth/SignIn");
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

/**
 * Open whatever is in the URL hash on page load.
 *
 */
function loadHash() {
	var hash = window.location.hash;
	if (hash) {
		loadPage(hash.replace("#", ""));
	} else {
		loadPage(home);
	}
}
ready(loadHash);
window.onhashchange = loadHash;

/**
 * Capture link clicks and use AJAX to load pages.
 *
 */
function setupLinks() {
	$("body").on("click", "a", function (e) {
		// get hyperlink
		var href = $(this).attr("href");

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
	$(selector).load(url, function (response, status, xhr) {
		// handle unauthorised
		if (xhr.status == 401) {
			showAlert(alertTypes.error, "You are not authorised to do this, please sign in.");
		}

		// save replaceId
		var form = $(this).find("form");
		if (replaceId) {
			form.attr("data-replace", replaceId);
			form.attr("data-replace-contents", replaceContents);
		}

		// create the modal object
		var wrapper = $(this);
		var modalId = $(this).find(".modal").attr("id");
		var modalEl = document.getElementById(modalId)

		// select item when modal is opened
		modalEl.addEventListener("shown.bs.modal", () => {
			var s = $(".modal-select");
			if (s.length == 1) {
				s.select();
			} else {
				s.first().select();
			}

			var f = $(".modal-focus");
			if (f.length == 1) {
				f.focus();
			} else {
				f.first().focus();
			}
		});

		// fade out background when modal is closed
		modalEl.addEventListener("hide.bs.modal", () => wrapper.fadeOut("fast"));

		// fade in background
		wrapper.fadeIn("fast", () => closeAlert());

		// close modal when wrapped is clicked
		$("body").on("click", (e) => {
			if ($(e.target).is(".modal")) {
				modal.hide();
			}
		});

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
 * Open the create modal.
 * 
 * @param {any} url
 */
function openCreateModal(url) {
	openModal("#create", url, null, true, () => {
		setupCreateModalSearch();
		setupCreateModalUnknown();
	});
}

/**
 * Filter list elements based on search field entry.
 *
 */
function setupCreateModalSearch() {
	// save new item on enter
	$("#create .list-filter").keydown(function (e) {
		if (e.keyCode == 13) {
			e.preventDefault();
		}
	});

	// filter as the user types
	$("#create .list-filter").keyup(function () {
		// get value from input
		var value = $(this).val();

		// get item list id
		var filterItems = $(this).data("filter-for");

		// filter items that match the input value
		$("#" + filterItems + " label").filter(function () {
			var show = $(this).data("text").indexOf(value.toLowerCase()) > -1;
			$(this).toggle(show);
		});
	});
}

/**
 * Enable unknown toggle switch for ending miles.
 *
 */
function setupCreateModalUnknown() {
	$(".unknown-toggle").on("change", function () {
		if ($(this).is(":checked")) {
			$(".unknown").val("").attr("disabled", true).blur();
		} else {
			var value = $(".unknown-value").val();
			$(".unknown").val(value).removeAttr("disabled").select();
		}
	})
}

/**
 * Open create modal when create button is clicked.
 *
 */
function setupCreateModalOpen() {
	$("body").on("click", ".btn-create", function (e) {
		// don't do whatever the link / button was going to do
		e.preventDefault();

		// get info
		var createUrl = $(this).data("create");

		// open modal
		openCreateModal(createUrl);
	});
}
ready(setupCreateModalOpen);

/**
 * Submit modal create form when the save button is pressed.
 *
 */
function setupCreateModalSave() {
	$("body").on("click", "#create .btn-save", function () {
		$("#create form").submit();
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
 * Open delete modal when delete buttons are clicked.
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
 * Setup button to open the change password modal when clicked.
 *
 */
function setupChangePasswordUpdateModal() {
	$("body").on("click", ".change-password", function (e) {
		e.preventDefault();
		var changeUrl = $(this).data("change");
		openUpdateModal(changeUrl, null);
	});
}
ready(setupChangePasswordUpdateModal);

/**
 * Open the update modal.
 * 
 * @param {string} url
 * @param {string} replaceId
 * @param {boolean} replaceContents
 */
function openUpdateModal(url, replaceId, replaceContents) {
	openModal("#update", url, replaceId, replaceContents, () => setupUpdateModalSearch());
}

/**
 * Open delete modal when delete buttons are clicked.
 *
 */
function setupUpdateModalOpen() {
	$("body").on("click", ".btn-update", function (e) {
		// don't do whatever the link / button was going to do
		e.preventDefault();

		// get info
		var updateUrl = $(this).data("update");
		var replaceId = $(this).data("replace");
		var replaceContents = $(this).data("replace-contents");

		// open modal
		openUpdateModal(updateUrl, replaceId, replaceContents);
	});
}
ready(setupUpdateModalOpen);

/**
 * Filter list elements based on search field entry.
 *
 */
function setupUpdateModalSearch() {
	// hide add item button
	var addItem = $("#update .btn-add");
	addItem.hide();

	// if add item button is not disabled, run save function on click
	if (addItem.not(":disabled")) {
		addItem.click(addNewItemToJourney);
	}

	// save new item on enter
	$("#update .list-filter").keydown(function (e) {
		if (e.keyCode == 13) {
			e.preventDefault();

			// if add item button is not disabled, run click method
			if (addItem.not(":disabled")) {
				addItem.click();
			}
		}
	});

	// filter as the user types
	$("#update .list-filter").keyup(function () {
		// get value from input
		var value = $(this).val().toString();

		// get item list id
		var filterItems = $(this).data("filter-for");

		// holds whether or not there is an exact match
		var exactMatch = false;

		// filter items that match the input value
		$("#" + filterItems + " label").filter(function () {
			// get text values as lowercase for comparison
			var itemText = $(this).data("text").toString().toLowerCase();
			var searchText = value.toLowerCase();

			// if the item contains the search text, show it
			var show = itemText.indexOf(searchText) > -1;
			$(this).toggle(show);

			// if the search text exactly matches an item, hide the add item button
			if (itemText == searchText) {
				exactMatch = true;
			}
		});

		// if the add item button is disabled, stop
		if (addItem.is(":disabled")) {
			return;
		}

		// show add item button
		if (value && !exactMatch) {
			addItem.text("Add: " + value).show();
		} else {
			addItem.text("").hide();
		}
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
	}

	// submit on button click, auto-save input change, and enter
	$("body").on("click", "#update .btn-save", (e) => submit(e));
	$("body").on("change", "#update .auto-save", (e) => submit(e));
	$("body").on("keypress", "#update input", function (e) {
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
	$("body").on("click", ".token > a, .btn-complete", function () {
		var updateUrl = $(this).data("update");
		var replaceId = $(this).data("replace");
		openUpdateModal(updateUrl, replaceId, false);
	});
}
ready(setupTokenUpdateModals);

/**
 * Add a new item to a Journey - called from the search box of an update item modal.
 *
 */
function addNewItemToJourney() {
	// get values
	var data = {
		id: $("#Journey_Id").val(),
		version: $("#Journey_Version").val(),
		value: $("#update .list-filter").val(),
		__requestVerificationToken: $("#update [name=__RequestVerificationToken]").val()
	};
	var url = $(this).data("url");

	// submit form
	submitForm($("#update form"), url, data);
}

/**
 * With responsive design the navbar needs to be closed when links are clicked.
 *
 */
function closeNavWhenClicked() {
	$(".navbar-nav .nav-item a").click(function () {
		$(".navbar-toggler:visible").click();
	})
}
ready(closeNavWhenClicked);

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
		loadPage(r.redirectTo);
	}
}

/**
 * Setup tab buttons to load settings on click.
 *
 */
function setupSettingsTabs() {
	$("body").on("click", "#settingsButtons .nav-link", function () {
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
 * Submit all forms via AJAX and handle results.
 *
 */
function setupAjaxSubmit() {
	$("body").on("submit", "form", function (e) {
		// stop default submit
		e.preventDefault();

		// check validity
		if (this.checkValidity() === false) {
			form.find(":input:visible").not("[formnovalidate]")
				.parent().addClass("was-validated");
			return;
		}

		// submit form
		submitForm($(this));
	});
}
ready(setupAjaxSubmit);

/**
 * Close a modal and submit a form, optionally overriding URL and data
 * 
 * @param {JQuery<any>} form Form to submit
 * @param {string} url Override form action and submit to this URL instead
 * @param {any} data Override form data and submit this data instead
 */
function submitForm(form, url, data) {
	// get form info
	var replaceId = form.data("replace");
	var replaceContents = form.data("replace-contents");

	// hide modal and show please wait message
	if (modal) {
		modal.hide();
	}
	showPleaseWaitAlert();

	// post data and handle result
	$.ajax(
		{
			method: "POST",
			url: url || form.attr("action"),
			data: data || form.serialize()
		}
	).done(
		function (data, status, xhr) {
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
			showAlert(alertTypes.warning, "Something went wrong, refreshing the page.", true);
			loadHash();
		}
	).fail(
		function (xhr) {
			// the response is a JSON result
			if (xhr && xhr.responseJSON) {
				handleResult(xhr.responseJSON);
				return;
			}

			// something else has gone wrong
			showAlert(alertTypes.error, "Something went wrong, please try again.");
		}
	);
}
