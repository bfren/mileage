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
