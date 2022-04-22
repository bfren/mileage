/**
 * Open the create modal.
 * 
 * @param {any} url
 * @param {any} replaceId
 */
function openCreateModal(url) {
	openModal("#create", url, null, true);
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
		modal.hide();
	});
}
ready(setupCreateModalSave);
