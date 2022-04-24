/**
 * Setup button to open the change password modal when clicked.
 *
 */
function setupChangePasswordUpdateModal() {
	$("body").on("click", ".change-password", function (e) {
		e.preventDefault();
		var changeUrl = $(this).data("change");
		openUpdateModal(changeUrl, null);
	});
}
ready(setupChangePasswordUpdateModal);
