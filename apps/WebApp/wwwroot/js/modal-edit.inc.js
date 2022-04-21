/**
 * Open the edit modal.
 * 
 * @param {any} url
 * @param {any} replaceId
 */
function openEditModal(url, replaceId) {
	openModal("#edit", url, replaceId, false, function() {
		setupEditModalSearch();
		$("#edit input.auto-save").change(function () {
			$("#edit form").submit();
			modal.hide();
		});
	});
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
