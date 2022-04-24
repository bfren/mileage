/**
 * With responsive design the navbar needs to be closed when links are clicked.
 *
 */
function closeNavWhenClicked() {
	$(".navbar-nav .nav-item a").click(function () {
		$(".navbar-toggler:visible").click();
	})
}
ready(closeNavWhenClicked);
