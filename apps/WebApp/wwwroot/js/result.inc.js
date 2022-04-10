function handleResult(r) {
  // show message and pass value to success function
  showAlert(r.message.type, r.message.text);

  // redirect
  if (r.redirectTo) {
    showAlert("info", "Redirecting, please wait...");
    window.location.href = r.redirectTo;
  }
}
