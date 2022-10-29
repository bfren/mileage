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
		var value = $(this).val().toString();

		// get item list id
		var filterItems = $(this).data("filter-for");

		// filter items that match the input value
		filterModalItems(filterItems, value);
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
	$("body").on("click", "#create .btn-save", () => $("#create form").submit());
}
ready(setupCreateModalSave);
