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
 * Open update modal when update buttons are clicked.
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

		// filter items and store whether or not there is an exact match -
		// if there is, we need to hide the add item button
		var exactMatch = filterModalItems(filterItems, value);

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
	var submit = (e) => {
		e.preventDefault();
		$("#update form").submit();
	}

	// submit on button click, auto-save input change, and enter
	$("body").on("click", "#update .btn-save", submit);
	$("body").on("change", "#update .auto-save", submit);
	$("body").on("keypress", "#update input", function (e) {
		if (e.keyCode == 13) {
			submit(e);
		}
	});
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
