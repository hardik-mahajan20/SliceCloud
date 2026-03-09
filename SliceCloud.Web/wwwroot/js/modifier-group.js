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

  // Edit Modifier Group Submission
  $(document).on("submit", "#editModifierGroupForm", function (e) {
    e.preventDefault();
    const form = $(this)[0];
    const formData = new FormData(form);

    $.ajax({
      type: "POST",
      url: "/Menu/EditModifierGroup",
      data: formData,
      processData: false,
      contentType: false,
      success: async function (response) {
        if (response.success) {
          toastr.success(response.message);
          let modal = document.getElementById("editModifierGroup");
          let modalInstance = bootstrap.Modal.getOrCreateInstance(modal);
          modalInstance.hide();
          // Update later for modifiers
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
        toastr.error("An unexpected error occurred.");
      },
    });
  });

  // Delete Modifier Group
  $(document).on("click", ".delete-modifier-group-btn", function (e) {
    e.stopPropagation();
    var modifierGroupId = $(this).data("id");

    $.ajax({
      type: "GET",
      url: "/Menu/LoadDeleteModifierGroupModal",
      success: function (response) {
        $("#modalContainer").empty();
        $("#modalContainer").html(response);
        $("#deleteModifierGroupId").val(modifierGroupId);
        let modal = document.getElementById("deleteModifierGroupModal");
        let modalInstance = new bootstrap.Modal(modal);
        modalInstance.show();
      },
      error: function () {
        toastr.error("An unexpected error occurred.");
      },
    });
  });

  // Delete Modifier Group Submission
  $(document).on("submit", "#deleteModifierGroupForm", function (e) {
    e.preventDefault();
    var modifierGroupId = $("#deleteModifierGroupId").val();
    $.ajax({
      type: "POST",
      url: "/Menu/DeleteModifierGroup/" + modifierGroupId,
      data: {
        modifierGroupId: modifierGroupId,
      },
      dataType: "json",
      success: async function (response) {
        if (response.success) {
          toastr.success("Modifier Group deleted successfully!");
          let modal = document.getElementById("deleteModifierGroupModal");
          let modalInstance = bootstrap.Modal.getOrCreateInstance(modal);
          modalInstance.hide();
          // Update later for modifiers
          await loadAllModifierGroups();
        } else {
          toastr.error("Error deleting modifier group.");
        }
      },
      error: function () {
        toastr.error("An unexpected error occurred.");
      },
    });
  });
});

// Initialize sortable for modifierGroup
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

async function loadAllModifierGroups(callback) {
  $.ajax({
    url: "/Menu/GetAllModifierGroups",
    type: "GET",
    success: function (data) {
      let modifierGroupList = $(".modifier-group-list");
      modifierGroupList.empty();
      let firstModifierGroupId = null;

      if (Array.isArray(data) && data.length > 0) {
        firstModifierGroupId = data[0].modifierGroupId;

        if (firstModifierGroupId) {
          $.each(data, function (index, modifierGroup) {
            let activeClass = index === 0 ? "active-modifier-group" : "";

            modifierGroupList.append(`
                    <li class="d-flex p-1 align-items-center justify-content-between modifier-btn btn ${activeClass}" 
                        data-id="${modifierGroup.modifierGroupId}">
                        <div class="d-flex align-items-center flex-wrap m-0 gap-2 ms-2">
                            <div class="sort-handle">
                                <i class="bi bi-grip-vertical"></i>
                            </div>
                            <div class="text-truncate modifier-group-name">${modifierGroup.modifierGroupName}</div>
                        </div>
                        <div class="d-flex">
                            <button class="edit-modifier-group-btn btn p-0 m-1" 
                                data-id="${modifierGroup.modifierGroupId}">
                                <i class="bi bi-pen"></i>
                            </button>
                            <button class="delete-modifier-group-btn btn p-0 m-1" 
                                data-id="${modifierGroup.modifierGroupId}">
                                <i class="bi bi-trash"></i>
                            </button>
                        </div>
                    </li>
                `);
          });

          if (callback) callback(firstModifierGroupId);
        }
      } else {
        toastr.warning("No modifier groups found!", "Warning");
      }
    },
    error: function (xhr, status, error) {
      toastr.error("Failed to load modifier groups!", "Error");
    },
  });
}
