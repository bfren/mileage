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
