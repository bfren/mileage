function setupDatepickers() {
	$.fn.datepicker.defaults.todayBtn = "linked";
	$.fn.datepicker.defaults.format = "yyyy-mm-dd";
}
ready(setupDatepickers);
