/// <reference path="axios.d.ts" />

document.addEventListener('submit', (e) => {
	console.log("Axios Submit.");

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
		if (r.data.redirectTo) {
			window.location.href = r.data.redirectTo;
		}
	}).catch((e) => {
		console.log(e);
	});

	// prevent default submit
	e.preventDefault();
});
