/**
 * Setup token links to open the edit modal when clicked.
 *
 */
function setupTokenModals() {
	$(".token > a").click(function () {
		var editUrl = $(this).data("edit");
		var replaceId = $(this).data("replace");
		openModal(editUrl, replaceId);
	});
}
ready(setupTokenModals);

var modal;

/**
 * Open the edit modal and load contents via HTTP.
 * 
 * @param {any} url
 * @param {any} replaceId
 */
function openModal(url, replaceId) {
	// show messages
	showPleaseWaitAlert();
	console.log("Loading modal from: " + url);

	// load modal HTML and then show modal
	$("#edit").load(url, function () {
		// save replaceId
		var form = $(this).find("form");
		form.attr("data-replace", replaceId);

		// create the modal object
		var wrapper = $(this);
		var modalId = $(this).find(".modal").attr("id");

		var modalEl = document.getElementById(modalId)

		// select item when modal is opened
		modalEl.addEventListener("shown.bs.modal", function () {
			$(".modal-select").focus();
		});

		// fade out background when modal is closed
		modalEl.addEventListener("hide.bs.modal", function () {
			wrapper.fadeOut("fast");
		});

		// fade in background and show modal
		wrapper.fadeIn("fast", function () {
			closeAlert();
		});

		modal = new bootstrap.Modal(modalEl);
		modal.show();

		// Setup save and submit
		setupModalSearch();
		setupModalSave();
		setupAjaxSubmit();
	});
}

/**
 * Filter list elements based on search field entry.
 *
 */
function setupModalSearch() {
	$("#list-filter").keydown(function (e) {
		// ignore enter key
		if (e.keyCode == 13) {
			e.preventDefault();
			return;
		}

		// get value from input
		var value = $(this).val().toLowerCase();

		// filter items that match the input value
		$("#list-items .list-item").filter(function () {
			$(this).toggle($(this).data("text").indexOf(value) > -1);
		});
	});
}

/**
 * Submit modal edit form when the save button is pressed.
 *
 */
function setupModalSave() {
	$("#edit .btn-save").click(function () {
		$("#edit form").submit();
		modal.hide();
	});
}
