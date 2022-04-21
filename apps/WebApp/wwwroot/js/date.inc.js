/**
 * Setup date picker defaults
 *
 */
function setupDatepickerDefaults() {
	$.fn.datepicker.defaults.autoclose = true;
	$.fn.datepicker.defaults.todayBtn = "linked";
	$.fn.datepicker.defaults.format = "yyyy-mm-dd";
}
ready(setupDatepickerDefaults);
