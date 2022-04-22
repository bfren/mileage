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
