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

document.addEventListener('submit', (e) => {
	// get form
	const form = e.target;
	const data = new FormData(form);
	
	// post data using axios
	axios.request({
		url: form.action,
		method: form.method,
		data: data,
		headers: { "Content-Type": "multipart/form-data" }
	}).then((r) => {
		handleResult(r.data);
	}).catch((e) => {
		console.log(e);
	});

	// prevent default submit
	e.preventDefault();
});
