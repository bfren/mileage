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
	addItem.click(addNewItemToJourney);

	// save new item on enter
	$("#update .list-filter").keydown(function (e) {
		if (e.keyCode == 13) {
			e.preventDefault();
			addItem.click();
		}
	});

	// filter as the user types
	$("#update .list-filter").keyup(function () {
		// get value from input
		var value = $(this).val();

		// show add item button
		if (value) {
			addItem.text("Add: " + value).show();
		} else {
			addItem.text("").hide();
		}

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
