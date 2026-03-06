$(document).ready(function () {});

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
