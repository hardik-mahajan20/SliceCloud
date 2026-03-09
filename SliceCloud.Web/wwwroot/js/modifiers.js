$(document).ready(function () {
  // Modifier Search
  $(document).on("keyup", "#modifierSearch", function () {
    let searchQuery = $(this).val();
    laodModifierGroupWiseModifies(
      selectedModifierGroupId,
      currentPageModifier,
      pageSizeModifier,
      searchQuery
    );
  });

  // PageSize Dropdown
  $(document).on("change", "#pageSizeDropdownModifier", function () {
    pageSize = $(this).val();
    currentPageModifier = 1;
    laodModifierGroupWiseModifies(
      selectedModifierGroupId,
      currentPageModifier,
      pageSizeModifier
    );
  });

  // Previous Page
  $(document).on("click", "#prevPageBtnModifier", function () {
    if (currentPageModifier > 1) {
      currentPageModifier--;
      laodModifierGroupWiseModifies(
        selectedModifierGroupId,
        currentPageModifier,
        pageSizeModifier
      );
    }
  });

  // Next Page
  $(document).on("click", "#nextPageBtnModifier", function () {
    if (currentPageModifier < totalPagesModifier) {
      currentPageModifier++;
      laodModifierGroupWiseModifies(
        selectedModifierGroupId,
        currentPageModifier,
        pageSizeModifier
      );
    }
  });

  // Load Add Item Modal
  $(document).on("click", ".add-modifier-btn", function () {
    $.ajax({
      url: "/Menu/GetAddModifier",
      type: "GET",
      success: function (data) {
        $("#modalContainer").empty();
        $("#modalContainer").html(data);

        let modal = document.getElementById("addModifierModalContainer");
        let modalInstance = new bootstrap.Modal(modal);
        modalInstance.show();

        initializeModifierCheckboxes();
        $.validator.unobtrusive.parse("#addModifierModalContainer");
      },
      error: function () {
        toastr.error("Failed to load modal data.", "Error");
      },
    });
  });

  // Add Modifier POST
  $(document).on("submit", "#addModifierForm", function (event) {
    event.preventDefault();

    updateModifierDropdownText();

    let formData = $(this).serialize();

    // Clear previous validation messages
    $("span[data-valmsg-for]").text("");

    $.ajax({
      type: "POST",
      url: "/Menu/AddModifier",
      data: formData,
      success: function (response) {
        if (response.success) {
          toastr.success("Modifier added successfully!");
          let modal = document.getElementById("addModifierModalContainer");
          let modalInstance = bootstrap.Modal.getOrCreateInstance(modal);
          modalInstance.hide();
        } else if (response.validationErrors) {
          // Loop through validation errors and show them under fields
          for (let key in response.validationErrors) {
            const messages = response.validationErrors[key];
            const span = $(`[data-valmsg-for="${key}"]`);
            if (span.length) {
              span.text(messages.join(", "));
            }
          }
        } else {
          toastr.error("Error: " + response.message, "Error");
        }
      },
      error: function () {
        toastr.error("An error occurred while saving the modifier.", "Error");
      },
    });
  });

  // Edit Modifier GET
  $(document).on("click", ".edit-modifier-btn", function () {
    var modifierId = $(this).data("id");

    $.ajax({
      url: "/Menu/GetModifierByIdEdit",
      type: "GET",
      data: {
        modifierId: modifierId,
      },
      success: function (response) {
        $("#modalContainer").empty();
        $("#modalContainer").html(response);
        let modal = document.getElementById("editModifierModalContainer");
        let modalInstance = new bootstrap.Modal(modal);
        modalInstance.show();

        $("#editModifierModalContainer").on("shown.bs.modal", function () {
          initializeEditModifierCheckboxes();
        });
      },
      error: function () {
        toastr.error("Failed to load modifier details.", "Error");
      },
    });
  });

  // Edit Modifier POST
  $(document).on("submit", "#editModifierForm", function (e) {
    e.preventDefault();

    updateEditModifierDropdownText();
    var modifierGroupId = $("#modifierGroupIdHidden").val();
    let formData = $(this).serialize();

    $.ajax({
      type: "POST",
      url: "/Menu/UpdateModifier",
      data: formData,
      success: function (response) {
        if (response.success) {
          toastr.success("Modifier Updated successfully");
          let modal = document.getElementById("editModifierModalContainer");
          let modalInstance = bootstrap.Modal.getOrCreateInstance(modal);
          modalInstance.hide();
        } else if (response.validationErrors) {
          // Loop through validation errors and show them under fields
          for (let key in response.validationErrors) {
            const messages = response.validationErrors[key];
            const span = $(`[data-valmsg-for="${key}"]`);
            if (span.length) {
              span.text(messages.join(", "));
            }
          }
        } else {
          toastr.error("Error: " + response.message, "Error");
        }
      },
      error: function () {
        toastr.error("An error occurred while saving the modifier.", "Error");
      },
    });
  });

  // Delete Modifier GET
  $(document).on("click", ".delete-modifier-btn", function () {
    var modifierId = $(this).data("id");
    $.ajax({
      type: "GET",
      url: "/Menu/LoadDeleteModifierModal",
      success: function (response) {
        $("#modalContainer").empty();
        $("#modalContainer").html(response);
        $("#deleteModifierId").val(modifierId);
        let modal = document.getElementById("deleteModifierModal");
        let modalInstance = new bootstrap.Modal(modal);
        modalInstance.show();
      },
      error: function () {
        toastr.error("An unexpected error occurred.");
      },
    });
  });

  // Delete Modifier Submission
  $(document).on("submit", "#deleteModifierForm", function (e) {
    e.preventDefault();
    var modifierId = $("#deleteModifierId").val();
    $.ajax({
      type: "POST",
      url: "/Menu/DeleteModifier/" + modifierId,
      data: {
        modifierId: modifierId,
      },
      dataType: "json",
      success: function (response) {
        if (response.success) {
          toastr.success("Modifier deleted successfully!");
          let modal = document.getElementById("deleteModifierModal");
          let modalInstance = bootstrap.Modal.getOrCreateInstance(modal);
          modalInstance.hide();
        } else {
          toastr.error("Error deleting category.");
        }
      },
      error: function () {
        toastr.error("An unexpected error occurred.");
      },
    });
  });

  // === Main Checkbox Change Event ===
  $(document).on("change", "#maincheckboxModifier", function () {
    const isChecked = this.checked;
    mainCheckboxStateModifier.isChecked = isChecked;
    mainCheckboxStateModifier.isIndeterminate = false;

    if (isChecked) {
      allModifierIds.forEach((id) => selectedModifiers.add(id));
    } else {
      selectedModifiers.clear();
    }

    $(".modifier-checkbox").prop("checked", isChecked);
    applyMainCheckboxStateModifier();
  });

  // === Individual Item Checkbox Change Event ===
  $(document).on("change", ".modifier-checkbox", function () {
    const modifierId = parseInt(
      $(this).closest("tr").find("input[name='ModifierId']").val()
    );

    if (this.checked) {
      selectedModifiers.add(modifierId);
    } else {
      selectedModifiers.delete(modifierId);
    }

    updateMainCheckboxStateModifier();
  });
});

// Fetch All Modifier Ids
async function fetchAllModifierIds() {
  let modifierGroupId =
    selectedModifierGroupId || $("#modifierGroupIdHidden").val();

  try {
    const response = await $.ajax({
      url: "/Menu/GetAllModifierIds",
      type: "GET",
      data: { modifierGroupId: modifierGroupId },
    });

    allModifierIds = new Set(response);
  } catch (error) {
    toastr.error("Failed to fetch item IDs.");
  }
}

// === Update Main Checkbox State ===
function updateMainCheckboxStateModifier() {
  if (selectedModifiers.size === 0) {
    mainCheckboxStateModifier.isChecked = false;
    mainCheckboxStateModifier.isIndeterminate = false;
  } else if (selectedModifiers.size === allModifierIds.size) {
    mainCheckboxStateModifier.isChecked = true;
    mainCheckboxStateModifier.isIndeterminate = false;
  } else {
    mainCheckboxStateModifier.isChecked = false;
    mainCheckboxStateModifier.isIndeterminate = true;
  }
  applyMainCheckboxStateModifier();
}

// === Apply Main Checkbox State ===
async function applyMainCheckboxStateModifier() {
  $("#maincheckboxModifier")
    .prop("checked", mainCheckboxStateModifier.isChecked)
    .prop("indeterminate", mainCheckboxStateModifier.isIndeterminate);
}

function initializeEditModifierCheckboxes() {
  $(".edit-modifier-checkbox").change(function () {
    updateEditModifierDropdownText();
  });

  $("#editSearchModifier").on("keyup", function () {
    let searchText = $(this).val().toLowerCase();
    $("#editModifierCheckboxList li").each(function () {
      $(this).toggle($(this).text().toLowerCase().includes(searchText));
    });
  });

  updateEditModifierDropdownText(); // Update dropdown on modal load
}

// Function to update dropdown text and hidden inputs based on selected modifier groups in Edit Modifier Modal
function updateEditModifierDropdownText() {
  let selectedValues = $(".edit-modifier-checkbox:checked")
    .map(function () {
      return $(this).val();
    })
    .get();

  $("#editModifierForm input[name='ModifierGroupIds[]']").remove();

  let form = $("#editModifierForm");
  selectedValues.forEach((value) => {
    form.append(
      `<input type="hidden" name="ModifierGroupIds[]" value="${value}">`
    );
  });

  let selectedText = $(".edit-modifier-checkbox:checked")
    .map(function () {
      return $(this).parent().text().trim();
    })
    .get();

  let buttonText = "Select Modifier Groups";
  if (selectedText.length > 0) {
    buttonText = selectedText[0]; // Show first selected item
    if (selectedText.length > 1) {
      buttonText += ` +${selectedText.length - 1} Other`;
    }
  }

  $("#editModifierDropdownBtn").text(buttonText);
}

// Initialize checkboxes in Add Modifier Modal
function initializeModifierCheckboxes() {
  $(".modifier-checkbox").change(function () {
    updateModifierDropdownText();
  });

  $("#searchModifier").on("keyup", function () {
    let searchText = $(this).val().toLowerCase();
    $("#modifierCheckboxList li").each(function () {
      $(this).toggle($(this).text().toLowerCase().includes(searchText));
    });
  });
}

// Function to update dropdown text and hidden inputs based on selected modifier groups
function updateModifierDropdownText() {
  let selectedValues = $(".modifier-checkbox:checked")
    .map(function () {
      return $(this).val();
    })
    .get();

  $("#addModifierForm input[name='ModifierGroupIds[]']").remove();

  let form = $("#addModifierForm");
  selectedValues.forEach((value) => {
    form.append(
      `<input type="hidden" name="ModifierGroupIds[]" value="${value}">`
    );
  });

  let selectedText = $(".modifier-checkbox:checked")
    .map(function () {
      return $(this).parent().text().trim();
    })
    .get();

  let buttonText = "Select Modifier Groups";
  if (selectedText.length > 0) {
    buttonText = selectedText[0];
    if (selectedText.length > 1) {
      buttonText += ` +${selectedText.length - 1} Other`;
    }
  }

  $("#modifierDropdownBtn").text(buttonText);
}

// Function to load modifiers dynamically based on selected modifier group
function laodModifierGroupWiseModifies(
  modifierGroupId,
  pageNumber = 1,
  pageSize = 5,
  searchQuery = ""
) {
  $.ajax({
    url: "/Menu/LoadModifiersByModifierGroup",
    type: "GET",
    data: {
      modifierGroupId: modifierGroupId,
      pageNumber: pageNumber,
      pageSize: pageSize,
      searchQuery: searchQuery,
    },
    success: async function (data) {
      $("#modifiers-container").html(data);
      updatePaginationControlsModifier();

      $(".modifier-checkbox").each(function () {
        const modifierId = parseInt(
          $(this).closest("tr").find("input[name='ModifierId']").val()
        );
        $(this).prop("checked", selectedModifiers.has(modifierId));
      });
      if (searchQuery !== "") {
        $("#maincheckboxModifier").addClass("d-none");
      }

      await applyMainCheckboxStateModifier();
      await fetchAllItemIds();
    },
    error: function () {
      toastr.error("An unexpected error occurred.");
    },
  });
}
