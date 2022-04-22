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
