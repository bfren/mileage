/**
 * Setup button to open the change password modal when clicked.
 *
 */
function setupChangePasswordUpdateModal() {
	$(".change-password").click(function (e) {
		e.preventDefault();
		var changeUrl = $(this).data("change");
		openUpdateModal(changeUrl, null);
	});
}
ready(setupChangePasswordUpdateModal);
