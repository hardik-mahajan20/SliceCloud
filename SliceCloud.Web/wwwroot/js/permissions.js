document.addEventListener("DOMContentLoaded", function () {
  if (window.TempData) {
    if (TempData.ErrorMessage) toastr.error(TempData.ErrorMessage);
    if (TempData.SuccessMessage) toastr.success(TempData.SuccessMessage);
    if (TempData.WarningMessage) toastr.warning(TempData.WarningMessage);
    if (TempData.InfoMessage) toastr.info(TempData.InfoMessage);
  }
});

$(document).ready(function () {
  const currentUserRole = document.getElementById("currentUserRole").value;
  const targetRole = document.getElementById("targetRole").value;

  if (currentUserRole === "Admin" && targetRole === "Admin") {
    $(".can-view, .module-checkbox .master-checkbox")
      .css("pointer-events", "none")
      .css("opacity", "0.6");
  }

  if (
    (currentUserRole === "Manager" && targetRole === "Admin") ||
    (currentUserRole === "Chef" && targetRole === "Admin") ||
    (currentUserRole === "Chef" && targetRole === "Manager")
  ) {
    $(".can-view, .can-add-edit, .can-delete").prop("readonly", true);
    $(".module-checkbox").prop("readonly", true);
    $(".master-checkbox").prop("readonly", true);
  }

  $(document).on(
    "click",
    ".can-view, .can-add-edit, .can-delete",
    function (event) {
      if (currentUserRole === "Manager" && targetRole === "Admin") {
        toastr.warning("You are not allowed to modify Admin's permissions.");

        event.preventDefault();
        event.stopImmediatePropagation();

        $(this).prop("checked", false);
      } else if (currentUserRole === "Chef" && targetRole === "Admin") {
        toastr.warning("You are not allowed to modify Admin's permissions.");

        event.preventDefault();
        event.stopImmediatePropagation();

        $(this).prop("checked", false);
      } else if (currentUserRole === "Chef" && targetRole === "Manager") {
        toastr.warning("You are not allowed to modify Manager's permissions.");

        event.preventDefault();
        event.stopImmediatePropagation();

        $(this).prop("checked", false);
      }
    }
  );

  function updateModuleCheckboxState() {
    $(".module-checkbox").each(function () {
      var row = $(this).closest("tr");
      var canView = row.find(".can-view");
      var canAddEdit = row.find(".can-add-edit");
      var canDelete = row.find(".can-delete");

      var totalChecked =
        canView.is(":checked") +
        canAddEdit.is(":checked") +
        canDelete.is(":checked");

      if (totalChecked === 3) {
        $(this).prop("checked", true).prop("indeterminate", false);
      } else if (totalChecked > 0) {
        $(this).prop("checked", false).prop("indeterminate", true);
      } else {
        $(this).prop("checked", false).prop("indeterminate", false);
      }
    });

    var totalModules = $(".module-checkbox").length;
    var checkedModules = $(".module-checkbox:checked").length;
    var indeterminateModules = $(".module-checkbox").filter(function () {
      return $(this).prop("indeterminate");
    }).length;

    if (checkedModules === totalModules) {
      $("#permissions-checkbox")
        .prop("checked", true)
        .prop("indeterminate", false);
    } else if (checkedModules > 0 || indeterminateModules > 0) {
      $("#permissions-checkbox")
        .prop("checked", false)
        .prop("indeterminate", true);
    } else {
      $("#permissions-checkbox")
        .prop("checked", false)
        .prop("indeterminate", false);
    }
  }

  updateModuleCheckboxState();

  $(".module-checkbox").click(function () {
    var row = $(this).closest("tr");
    var canView = row.find(".can-view");
    var canAddEdit = row.find(".can-add-edit");
    var canDelete = row.find(".can-delete");

    if (!$(this).prop("checked") && !$(this).prop("indeterminate")) {
      $(this).prop("indeterminate", true);
      canView.prop("checked", true);
      canAddEdit.prop("checked", false);
      canDelete.prop("checked", false);
    } else if ($(this).prop("indeterminate")) {
      $(this).prop("checked", true).prop("indeterminate", false);
      canView.prop("checked", true);
      canAddEdit.prop("checked", true);
      canDelete.prop("checked", true);
    } else {
      $(this).prop("checked", false).prop("indeterminate", false);
      canView.prop("checked", false);
      canAddEdit.prop("checked", false);
      canDelete.prop("checked", false);
    }

    updateModuleCheckboxState();
  });

  $(".can-view, .can-add-edit, .can-delete").change(function () {
    var row = $(this).closest("tr");
    var canView = row.find(".can-view");
    var canAddEdit = row.find(".can-add-edit");
    var canDelete = row.find(".can-delete");

    if ($(this).hasClass("can-add-edit") || $(this).hasClass("can-delete")) {
      if ($(this).is(":checked")) {
        canView.prop("checked", true);
      }
    }

    if (!canView.is(":checked")) {
      canAddEdit.prop("checked", false);
      canDelete.prop("checked", false);
    }

    updateModuleCheckboxState();
  });

  $("#permissions-checkbox").click(function () {
    var isChecked = $(this).prop("checked");
    var isIndeterminate = $(this).prop("indeterminate");

    if (!isChecked && !isIndeterminate) {
      $(this).prop("indeterminate", true);
      $(".module-checkbox").prop("indeterminate", true).prop("checked", false);
      $(".can-view").prop("checked", true);
      $(".can-add-edit, .can-delete").prop("checked", false);
    } else if (isIndeterminate) {
      $(this).prop("checked", true).prop("indeterminate", false);
      $(".module-checkbox").prop("checked", true).prop("indeterminate", false);
      $(".can-view, .can-add-edit, .can-delete").prop("checked", true);
    } else {
      $(this).prop("checked", false).prop("indeterminate", false);
      $(".module-checkbox, .can-view, .can-add-edit, .can-delete").prop(
        "checked",
        false
      );
    }

    updateModuleCheckboxState();
  });

  $(".can-add-edit, .can-delete").change(function () {
    let parentRow = $(this).closest("tr");
    let moduleCheckbox = parentRow.find(".module-checkbox");

    if ($(this).prop("checked")) {
      moduleCheckbox.prop("checked", true);
    }
  });
});
