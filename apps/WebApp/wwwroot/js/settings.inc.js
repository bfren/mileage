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

	// get tab source URL
	var src = tab.data("src");

	// load source
	showPleaseWaitAlert()
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
