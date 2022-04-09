function useAjaxSubmit() {
	$("form").submit(function (e) {
		// stop default submit
		e.preventDefault();

		// show info message
		showAlert("info", "Please wait...");

		// get form info
		var form = $(this);
		var url = form.attr("action");
		var data = form.serialize();

		// post data and handle result
		$.ajax({
			url: url,
			method: "POST",
			data: data,
			dataType: "json"
		}).done(
			function (data) {
				handleResult(data);
			}
		).fail(
			function (error) {
				handleResult(error.responseJSON);
			}
		);
	});
}
ready(useAjaxSubmit);
