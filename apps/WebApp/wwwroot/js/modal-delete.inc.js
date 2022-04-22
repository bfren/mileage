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
