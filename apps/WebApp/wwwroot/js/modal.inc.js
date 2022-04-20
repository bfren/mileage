/**
 * Setup token links to open the edit modal when clicked.
 *
 */
function setupTokenModals() {
	$("body").on("click", ".token > a", function () {
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
		modalEl.addEventListener("shown.bs.modal", () => $(".modal-select").focus());

		// fade out background when modal is closed
		modalEl.addEventListener("hide.bs.modal", () => wrapper.fadeOut("fast"));

		// fade in background and show modal
		wrapper.fadeIn("fast", () => closeAlert());

		// create and show modal
		modal = new bootstrap.Modal(modalEl);
		modal.show();

		// Setup search behaviour
		setupModalSearch();
	});
}

/**
 * Filter list elements based on search field entry.
 *
 */
function setupModalSearch() {
	// hide add item button
	var addItem = $("#edit .btn-add");
	addItem.hide();

	// save new item on enter
	$("#list-filter").keydown(function (e) {
		if (e.keyCode == 13) {
			e.preventDefault();
			addItem.click();
		}
	});

	// filter as the user types
	$("#list-filter").keyup(function () {
		// get value from input
		var value = $(this).val();

		// show add item button
		if (value) {
			addItem.text("Add: " + value).show();
		} else {
			addItem.text("").hide();
		}

		// filter items that match the input value
		$("#list-items .list-item").filter(function () {
			var show = $(this).data("text").indexOf(value.toLowerCase()) > -1;
			$(this).toggle(show);
		});
	});
}

/**
 * Submit modal edit form when the save button is pressed.
 *
 */
function setupModalSave() {
	$("body").on("click", "#edit .btn-save", function () {
		$("#edit form").submit();
		modal.hide();
	});
}
ready(setupModalSave);
