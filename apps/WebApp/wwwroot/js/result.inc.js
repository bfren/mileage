function handleResult(r, onSuccess, onFailure) {
	// log to console
	console.log(r);

	// pass value to success function
	if (r.success && onSuccess) {
		if (onSuccess) {
			onSuccess(r.value);
		}
	}

	// pass reason to failure function
	else if (onFailure) {
		onFailure(r.reason);
	}

	// redirect
	if (r.redirectTo) {
		window.location.href = r.redirectTo;
	}
}
