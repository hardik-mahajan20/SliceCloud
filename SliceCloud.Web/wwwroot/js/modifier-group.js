$(document).ready(function () {
  // Add Modifier Group
  $(document).on("click", "#openAddModifierGroupModal", function () {
    $.ajax({
      url: "/Menu/LoadAddModifierGroupModal",
      type: "GET",
      success: function (data) {
        $("#modalContainer").empty();
        $("#modalContainer").html(data);
        let modal = document.getElementById("addModifierGroup");
        let modalInstance = new bootstrap.Modal(modal);
        modalInstance.show();
      },
      error: function () {
        toastr.error("An unexpected error occurred.", "Error");
      },
    });
  });

  // Add Modifier Group Submission
  $(document).on("submit", "#addModifierGroupForm", function (e) {
    e.preventDefault();

    var form = $(this);
    $.ajax({
      url: form.attr("action"),
      type: form.attr("method"),
      data: form.serialize(),
      success: async function (response) {
        if (response.success) {
          toastr.success(response.message);
          let modal = document.getElementById("addModifierGroup");
          let modalInstance = bootstrap.Modal.getOrCreateInstance(modal);
          modalInstance.hide();
          await loadAllModifierGroups();
        } else {
          // Display validation errors
          $(".text-danger").html(""); // Clear existing errors
          $.each(response.errors, function (key, errorMessages) {
            $("#" + key)
              .next(".text-danger")
              .html(errorMessages.join("<br>"));
          });
        }
      },
      error: function () {
        toastr.error("An unexpected error occurred.", "Error");
      },
    });
  });

  // Edit Modifier Group
  $(document).on("click", ".edit-modifier-group-btn", function (e) {
    e.stopPropagation();
    var modifierGroupId = $(this).data("id");

    $.ajax({
      type: "GET",
      url: "/Menu/GetModifierGroupById/" + modifierGroupId,
      success: function (response) {
        $("#modalContainer").empty();
        $("#modalContainer").html(response);
        let modal = document.getElementById("editModifierGroup");
        let modalInstance = new bootstrap.Modal(modal);
        modalInstance.show();
      },
      error: function () {
        toastr.error("An unexpected error occurred.", "Error");
      },
    });
  });
});

// Initialize sortable for categories
function initializeModifierGroupSortable() {
  $("#modifierGroupList").sortable({
    update: function (event, ui) {
      var sortedIDs = $(this)
        .sortable("toArray")
        .map(function (id) {
          return id.split("-")[1];
        });

      $.ajax({
        url: "/Menu/UpdateModifierGroupOrder",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(sortedIDs),
        success: function () {
          toastr.success("Modifier group order updated successfully!");
        },
        error: function () {
          toastr.error("An unexpected error occurred.", "Error");
        },
      });
    },
  });
}
