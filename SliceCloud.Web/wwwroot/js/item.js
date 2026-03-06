$(document).ready(function () {
  let selectedModifierGroupData = [];
  let selectedModifiersData = {};
  // Add Item
  $("#ModifierGroupDropdown").select2({
    placeholder: "Select Modifier Groups",
    allowClear: true,
    width: "100%",
    tags: true,
    closeOnSelect: false,
  });

  // Load Add Item Modal
  $(document).on("click", ".add-item-btn", function () {
    $.ajax({
      url: "/Menu/GetMenuData",
      type: "GET",
      success: function (data) {
        $("#modalContainer").empty();
        $("#modalContainer").html(data);

        let modal = document.getElementById("addItemModalContainer");
        let modalInstance = new bootstrap.Modal(modal);
        modalInstance.show();

        $.validator.unobtrusive.parse("#addItemModalContainer");

        setTimeout(() => {
          bindModifierGroupEventAdd();
        }, 100);
      },
      error: function () {
        toastr.error("Failed to load modal data.", "Error");
      },
    });
  });

  // bind modifier group change event for Add Item Modal
  function bindModifierGroupEventAdd() {
    $(document).on("change", "#ModifierGroupDropdown", function () {
      var selectedGroupIds = $(this).val() || [];
      if (selectedGroupIds && selectedGroupIds.length > 0) {
        $.ajax({
          url: "/Menu/GetModifiersByGroup",
          type: "GET",
          traditional: true,
          data: {
            modifierGroupIds: selectedGroupIds,
          },
          success: function (response) {
            updateModifierItems(response);
          },
          error: function (xhr, status, error) {
            toastr.error("AJAX Error: " + error, "Error");
          },
        });
      } else {
        $("#ModifierItemsContainer").html(
          "<p class='text-muted'>Select a Modifier Group to load items.</p>"
        );
      }
    });
  }

  // Function to update modifier items based on selected groups
  function updateModifierItems(data) {
    var container = $("#ModifierItemsContainer");
    container.find(".no-modifier-message").remove();

    if (data.groups && data.groups.length > 0) {
      $.each(data.groups, function (_index, group) {
        if ($(`#group-${group.groupId}`).length === 0) {
          selectedModifiersData[group.groupId] = {
            groupName: group.groupName,
            modifiers: [],
            dropdown1Value: 0,
            dropdown2Value: 0,
          };
          selectedModifierGroupData.push({
            modifierGroupId: group.groupId,
            min: 0,
            max: 0,
          });

          var groupHtml = `
                 <div id="group-${group.groupId}" class="px-3 mt-3">
                     <div class="d-flex justify-content-between align-items-center mb-2">
                         <strong class="text-muted">${group.groupName}</strong>
                         <button class="remove-group" data-group="${
                           group.groupId
                         }">
                             <i class="fa-solid fa-trash" style="color:#808080;"></i>
                         </button>
                     </div>
                     <div>
                         <div class="row">
                             <div class="col-6 mb-2">
                                 <select class="form-select form-select-sm modifier-quantity rounded-pill border dropdown1" data-group="${
                                   group.groupId
                                 }">
                                     ${[...Array(6).keys()]
                                       .map(
                                         (i) =>
                                           `<option value="${i}">${i}</option>`
                                       )
                                       .join("")}
                                 </select>
                             </div>
                             <div class="col-6 mb-2">
                                 <select class="form-select form-select-sm modifier-quantity rounded-pill border dropdown2" data-group="${
                                   group.groupId
                                 }">
                                     ${[...Array(6).keys()]
                                       .map(
                                         (i) =>
                                           `<option value="${i}">${i}</option>`
                                       )
                                       .join("")}
                                 </select>
                             </div>
                         </div>
                         <ul>`;

          var groupModifiers = data.modifierItems.filter((m) =>
            m.groupId.some((id) => id === group.groupId)
          );
          if (groupModifiers.length > 0) {
            $.each(groupModifiers, function (idx, item) {
              groupHtml += `
                         <li>
                             <div class="d-flex justify-content-between align-items-center">
                                 <span>${item.modifierName}</span>
                                 <span>${item.price}</span>
                             </div>
                         </li>`;

              selectedModifiersData[group.groupId].modifiers.push({
                modifierId: item.modifierId,
                modifierName: item.modifierName,
                price: item.price,
              });
            });
          } else {
            groupHtml += `<p class='text-muted'>No modifiers available for this group.</p>`;
          }

          groupHtml += `</ul></div></div>`;
          container.prepend(groupHtml);
        }
      });
    }

    if ($("#ModifierItemsContainer").children().length === 0) {
      container.append(
        "<p class='text-muted no-modifier-message'>Select a Modifier Group to load items.</p>"
      );
    }
  }

  // Handle changes in dropdowns to maintain min/max logic and enable/disable options
  $(document).on("change", ".dropdown1, .dropdown2", function () {
    var groupId = $(this).data("group");
    var dropdownType = $(this).hasClass("dropdown1")
      ? "dropdown1Value"
      : "dropdown2Value";
    var value = parseInt($(this).val(), 10);

    let modifierGroup = selectedModifierGroupData.find(
      (x) => x.modifierGroupId == groupId
    );
    if (!modifierGroup) return;

    if (dropdownType === "dropdown1Value") {
      modifierGroup.min = value;

      if (modifierGroup.min > modifierGroup.max) {
        modifierGroup.max = modifierGroup.min;
        $(`.dropdown2[data-group='${groupId}']`).val(modifierGroup.min);
      }
    } else {
      // Max changed
      modifierGroup.max = value;

      if (modifierGroup.max < modifierGroup.min) {
        modifierGroup.min = modifierGroup.max;
        $(`.dropdown1[data-group='${groupId}']`).val(modifierGroup.max);
      }
    }

    // **Dynamically enable/disable options & fade invalid ones**
    $(`.dropdown2[data-group='${groupId}'] option`).each(function () {
      var optionVal = parseInt($(this).val(), 10);
      if (optionVal < modifierGroup.min) {
        $(this).prop("disabled", true).css("color", "#ccc");
      } else {
        $(this).prop("disabled", false).css("color", "");
      }
    });

    $(`.dropdown1[data-group='${groupId}'] option`).each(function () {
      var optionVal = parseInt($(this).val(), 10);
      if (optionVal > modifierGroup.max) {
        $(this).prop("disabled", true).css("color", "#ccc");
      } else {
        $(this).prop("disabled", false).css("color", "");
      }
    });
  });

  // Handle removing a modifier group
  $(document).on("click", ".remove-group", function () {
    var groupId = $(this).data("group");

    $(`#group-${groupId}`).remove();
    delete selectedModifiersData[groupId];
    selectedModifierGroupData = selectedModifierGroupData.filter(
      (x) => x.modifierGroupId != groupId
    );

    var selectedOptions = $("#ModifierGroupDropdown").val();
    selectedOptions = selectedOptions.filter((id) => id != groupId);
    $("#ModifierGroupDropdown").val(selectedOptions).trigger("change");

    if ($("#ModifierItemsContainer").children().length === 0) {
      $("#ModifierItemsContainer").html(
        "<p class='text-muted no-modifier-message'>Select a Modifier Group to load items.</p>"
      );
    }
  });

  // Add Item POST
  $(document).on("submit", "#menuItemForm", function (event) {
    event.preventDefault();

    let formData = new FormData(this);

    formData.set("IsAvailable", $("input[name='IsAvailable']").is(":checked"));
    formData.set(
      "IsDefaultTax",
      $("input[name='IsDefaultTax']").is(":checked")
    );

    let modifierGroups = selectedModifierGroupData.map((group) => ({
      ModifierGroupId: group.modifierGroupId,
      MinValue: parseInt(group.min) || 0,
      MaxValue: parseInt(group.max) || 0,
      Modifiers: selectedModifiersData[group.modifierGroupId]?.modifiers || [],
    }));

    formData.append("ModifierGroupsJson", JSON.stringify(modifierGroups));

    $(".text-danger").text(""); // Clear any text-danger (validation) messages
    $(".is-invalid").removeClass("is-invalid"); // Remove the is-invalid class

    $.ajax({
      type: "POST",
      url: "/Menu/AddMenuItem",
      data: formData,
      processData: false,
      contentType: false,
      success: function (response) {
        if (response.success) {
          toastr.success("Menu item added successfully!");
          let modal = document.getElementById("addItemModalContainer");
          let modalInstance = bootstrap.Modal.getOrCreateInstance(modal);
          modalInstance.hide();
        } else {
          if (response.message.includes("already exists")) {
            toastr.warning(response.message);
          } else {
            toastr.error("Error: " + response.message);
          }

          // Display validation errors
          if (response.errors) {
            for (const field in response.errors) {
              let inputField = $("[name='" + field + "']");
              if (inputField.length > 0) {
                let errorMessage = response.errors[field];
                let errorSpan = inputField
                  .closest(".form-floating")
                  .find("span.text-danger");
                errorSpan.text(errorMessage);
              }
            }
          }
        }
      },
      error: function (xhr) {
        toastr.error("An error occurred: " + xhr.responseText);
      },
    });
  });

  // Edit Item Get AJAX

  $(document).on("click", ".edit-item-btn", function () {
    // Initialize Select2 for Modifier Groups Dropdown
    $("#EditModifierGroupDropdown").select2({
      placeholder: "Select Modifier Groups",
      allowClear: true,
      width: "100%",
      tags: true,
      closeOnSelect: false,
    });

    var itemId = $(this).data("id");

    $.ajax({
      url: "/Menu/GetItemById",
      type: "GET",
      data: {
        id: itemId,
      },
      success: function (response) {
        // Once data is successfully returned, load the modal content
        $("#modalContainer").empty();
        $("#modalContainer").html(response);
        let editMenuItemModal = document.getElementById("editItemModal");
        let modalInstance = new bootstrap.Modal(editMenuItemModal);
        modalInstance.show();

        setTimeout(function () {
          bindModifierGroupEvent();
          fetchModifierMappings(itemId);
        }, 500);
      },
      error: function () {
        toastr.error("Failed to load item data.", "Error");
      },
    });
  });

  // Fetch and prepopulated modifier groups & items
  function fetchModifierMappings(itemId) {
    $.ajax({
      url: "/Menu/GetModifierMappingsByItemId",
      type: "GET",
      data: {
        id: itemId,
      },
      success: function (modifierMappings) {
        if (modifierMappings && modifierMappings.length > 0) {
          prepopulatedModifierGroups(modifierMappings);
        } else {
          toastr.warning(
            "No modifier mappings found for this item.",
            "Warning"
          );
        }
      },
      error: function () {
        toastr.error("Failed to fetch modifier mappings.", "Error");
      },
    });
  }

  // Prepopulated Modifier Groups
  function prepopulatedModifierGroups(modifierMappings) {
    var modifierContainer = $("#modifierGroupContainer");
    modifierContainer.empty();

    let selectedGroupIds = [];

    modifierMappings.forEach(function (mapping) {
      selectedGroupIds.push(mapping.modifierGroupId);
      addModifierGroupToUI(
        mapping.modifierGroupId,
        mapping.modifierGroupName,
        mapping.minValue,
        mapping.maxValue,
        mapping.modifierItems
      );
    });

    $("#EditModifierGroupDropdown").val(selectedGroupIds);
  }

  // Function to add modifier group to UI
  function addModifierGroupToUI(groupId, groupName, min, max, items = []) {
    var modifierContainer = $("#modifierGroupContainer");

    if ($("#group-" + groupId).length) {
      return;
    }

    var groupHtml = `
       <div id="group-${groupId}" class="modifier-group px-3 mt-1">
           <div class="d-flex justify-content-between align-items-center mb-2">
               <strong class="text-muted">${groupName}</strong>
               <button class="remove-group" data-group="${groupId}">
                   <i class="fa-solid fa-trash" style="color:#808080;"></i>
               </button>
           </div>
           <div class="row">
               <div class="col-6 mb-2">
                   <label class="small">Min</label>
                   <select class="form-select form-select-sm dropdown1 rounded-pill" data-group="${groupId}">
                       ${[...Array(6).keys()]
                         .map(
                           (i) =>
                             `<option value="${i}" ${
                               i == min ? "selected" : ""
                             }>${i}</option>`
                         )
                         .join("")}
                   </select>
               </div>
               <div class="col-6 mb-2">
                   <label class="small">Max</label>
                   <select class="form-select form-select-sm dropdown2 rounded-pill" data-group="${groupId}">
                       ${[...Array(6).keys()]
                         .map(
                           (i) =>
                             `<option value="${i}" ${
                               i == max ? "selected" : ""
                             }>${i}</option>`
                         )
                         .join("")}
                   </select>
               </div>
           </div>
           <ul>`;

    items.forEach(function (item) {
      groupHtml += `
        <li>
            <div class="d-flex justify-content-between align-items-center">
            <span>${item.modifierItemName}</span>
                <span>${item.Price || item.price || 0}</span>
            </div>
        </li>`;
    });

    groupHtml += `</ul></div>`;

    modifierContainer.prepend(groupHtml);

    let minDropdown = $(`#group-${groupId} .dropdown1`);
    let maxDropdown = $(`#group-${groupId} .dropdown2`);

    minDropdown.val(min).trigger("change");
    maxDropdown.val(max).trigger("change");

    updateDropdownConstraints(groupId);
  }

  // Bind Modifier Group Dropdown Events
  function bindModifierGroupEvent() {
    $(document).on("change", "#EditModifierGroupDropdown", function () {
      var selectedGroups = $(this).val() || [];

      if (selectedGroups.length === 0) {
        toastr.warning("No modifier groups selected.", "Warning");
        return;
      }

      $.ajax({
        url: "/Menu/GetModifiersByGroup",
        type: "GET",
        data: {
          modifierGroupIds: selectedGroups,
        },
        traditional: true,
        success: function (response) {
          if (response && response.groups) {
            response.groups.forEach(function (group) {
              if ($("#group-" + group.groupId).length === 0) {
                addModifierGroupToUI(
                  group.groupId,
                  group.groupName,
                  0,
                  0,
                  response.modifierItems.filter((m) =>
                    m.groupId.some((id) => id === group.groupId)
                  )
                );
              }
            });
          }
        },
        error: function (xhr, status, error) {
          toastr.error("Error fetching modifiers: " + error, "Error");
        },
      });
    });

    // Handle Removal of Modifier Group
    $(document).on("click", ".remove-group", function () {
      var groupId = $(this).data("group");
      $("#group-" + groupId).remove();

      // Update dropdown after removal
      var selectedValues = $("#EditModifierGroupDropdown").val() || [];
      $("#EditModifierGroupDropdown").val(
        selectedValues.filter((id) => id !== groupId)
      );
    });
  }

  $(document).on("change", ".dropdown1, .dropdown2", function () {
    var groupId = $(this).data("group");
    updateDropdownConstraints(groupId);
  });

  function updateDropdownConstraints(groupId) {
    var minDropdown = $(`#group-${groupId} .dropdown1`);
    var maxDropdown = $(`#group-${groupId} .dropdown2`);

    var minValue = parseInt(minDropdown.val());
    var maxValue = parseInt(maxDropdown.val());

    if (minValue > maxValue) {
      maxDropdown.val(minValue).trigger("change");
    }

    if (maxValue < minValue) {
      minDropdown.val(maxValue).trigger("change");
    }

    // Disable invalid options and fade them
    minDropdown.find("option").each(function () {
      var val = parseInt($(this).val());
      if (val > maxValue) {
        $(this).prop("disabled", true).css("color", "#ccc"); // Fade out
      } else {
        $(this).prop("disabled", false).css("color", "black"); // Restore color
      }
    });

    maxDropdown.find("option").each(function () {
      var val = parseInt($(this).val());
      if (val < minValue) {
        $(this).prop("disabled", true).css("color", "#ccc"); // Fade out
      } else {
        $(this).prop("disabled", false).css("color", "black"); // Restore color
      }
    });
  }

  // Edit Item POST
  $(document).on("submit", "#editItemForm", function (event) {
    event.preventDefault();

    let formData = new FormData(this);

    formData.set("IsAvailable", $("input[name='IsAvailable']").is(":checked"));
    formData.set(
      "IsDefaultTax",
      $("input[name='IsDefaultTax']").is(":checked")
    );

    // Collect Modifier Group Data
    let modifierGroups = [];
    $(".modifier-group").each(function () {
      let groupId = $(this).attr("id").replace("group-", "");
      let min = parseInt($(this).find(".dropdown1").val()) || 0;
      let max = parseInt($(this).find(".dropdown2").val()) || 0;

      let modifiers = [];
      $(this)
        .find("ul li")
        .each(function () {
          let modifierId = $(this).data("modifier-id");
          let modifierName = $(this).find(".modifier-name").text();
          let price = parseFloat($(this).find(".modifier-price").text()) || 0;

          modifiers.push({
            ModifierId: modifierId,
            ModifierName: modifierName,
            Price: price,
          });
        });

      modifierGroups.push({
        ModifierGroupId: groupId,
        MinValue: min,
        MaxValue: max,
        Modifiers: modifiers,
      });
    });

    formData.append("ModifierGroupsJson", JSON.stringify(modifierGroups));

    $.ajax({
      type: "POST",
      url: "/Menu/UpdateMenuItem",
      data: formData,
      processData: false,
      contentType: false,
      success: function (response) {
        if (response.success) {
          toastr.success("Menu item updated successfully!");
          let editItemModal = document.getElementById("editItemModal");
          let modalInstance =
            bootstrap.Modal.getOrCreateInstance(editItemModal);
          modalInstance.hide();
          // loadItems(response.categoryId, 1, 5, "");
        } else {
          if (response.message.includes("already exists")) {
            toastr.warning(response.message);
          } else {
            toastr.error("Error: " + response.message);
          }

          // Display validation errors
          if (response.errors) {
            for (const field in response.errors) {
              let inputField = $("[name='" + field + "']");
              if (inputField.length > 0) {
                let errorMessage = response.errors[field];
                let errorSpan = inputField
                  .closest(".form-floating")
                  .find("span.text-danger");
                errorSpan.text(errorMessage);
              }
            }
          }
        }
      },
      error: function (xhr) {
        toastr.error("An error occurred: " + xhr.responseText);
      },
    });
  });

  // Delete Menu Item
  $(document).on("click", ".delete-item-btn", function () {
    var menuItemId = $(this).data("id");
    $.ajax({
      type: "GET",
      url: "/Menu/LoadDeleteMenuItemModal",
      success: function (response) {
        $("#modalContainer").empty();
        $("#modalContainer").html(response);
        $("#deleteItemId").val(menuItemId);
        let modal = document.getElementById("deleteItemModal");
        let modalInstance = new bootstrap.Modal(modal);
        modalInstance.show();
      },
      error: function () {
        toastr.error("An unexpected error occurred.");
      },
    });
  });

  // Delete Menu Item Submission
  $(document).on("submit", "#deleteItemForm", function (e) {
    e.preventDefault();
    var menuItemId = $("#deleteItemId").val();
    $.ajax({
      type: "POST",
      url: "/Menu/DeleteMenuItem/" + menuItemId,
      data: {
        itemId: menuItemId,
      },
      dataType: "json",
      success: function (response) {
        if (response.success) {
          toastr.success("Category deleted successfully!");
          let modal = document.getElementById("deleteItemModal");
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
  $(document).on("change", "#maincheckbox", function () {
    const isChecked = this.checked;
    mainCheckboxState.isChecked = isChecked;
    mainCheckboxState.isIndeterminate = false;

    if (isChecked) {
      allItemIds.forEach((id) => selectedItems.add(id));
    } else {
      selectedItems.clear();
    }

    $(".item-checkbox").prop("checked", isChecked);
    applyMainCheckboxState();
  });

  // === Individual Item Checkbox Change Event ===
  $(document).on("change", ".item-checkbox", function () {
    const itemId = parseInt(
      $(this).closest("tr").find("input[name='ItemId']").val()
    );

    if (this.checked) {
      selectedItems.add(itemId);
    } else {
      selectedItems.delete(itemId);
    }

    updateMainCheckboxState();
  });

  // Delete Menu Item
  $(document).on("click", ".delete-multiple-item-btn", function () {
    let selectedItemsArray = Array.from(selectedItems);

    if (selectedItemsArray.length === 0) {
      toastr.warning("Please select at least one item to delete.");
      return;
    }
    $.ajax({
      type: "GET",
      url: "/Menu/LoadDeleteMultipleMenuItemModal",
      success: function (response) {
        $("#modalContainer").empty();
        $("#modalContainer").html(response);
        let modal = document.getElementById("deleteConfirmationModalItems");
        let modalInstance = new bootstrap.Modal(modal);
        modalInstance.show();
      },
      error: function () {
        toastr.error("An unexpected error occurred.");
      },
    });
  });

  // Confirm delete AJAX
  $(document).on("click", "#confirmDelete", function () {
    $.ajax({
      url: "/Menu/DeleteMultipleMenuItem",
      type: "POST",
      contentType: "application/json",
      data: JSON.stringify(Array.from(selectedItems)),
      success: function (response) {
        if (response.success) {
          toastr.success("Items deleted successfully.");
          let modal = document.getElementById("deleteConfirmationModalItems");
          let modalInstance = bootstrap.Modal.getOrCreateInstance(modal);
          modalInstance.hide();
          selectedItems.clear();
          mainCheckboxState = { isChecked: false, isIndeterminate: false };

          // Uncheck all checkboxes
          $("#maincheckbox")
            .prop("checked", false)
            .prop("indeterminate", false);
          $(".item-checkbox").prop("checked", false);

          // Optional: Refetch IDs if needed
          fetchAllItemIds();
        } else {
          toastr.error("Error: " + response.message);
        }
      },
      error: function () {
        toastr.error("Something went wrong. Please try again.");
      },
    });
  });
});
async function fetchAllItemIds() {
  let categoryId = selectedCategoryId || $("#categoryIdHidden").val();

  try {
    const response = await $.ajax({
      url: "/Menu/GetAllItemIds",
      type: "GET",
      data: { categoryId: categoryId },
    });

    allItemIds = new Set(response);
  } catch (error) {
    toastr.error("Failed to fetch item IDs.");
  }
}

// === Update Main Checkbox State ===
function updateMainCheckboxState() {
  if (selectedItems.size === 0) {
    mainCheckboxState.isChecked = false;
    mainCheckboxState.isIndeterminate = false;
  } else if (selectedItems.size === allItemIds.size) {
    mainCheckboxState.isChecked = true;
    mainCheckboxState.isIndeterminate = false;
  } else {
    mainCheckboxState.isChecked = false;
    mainCheckboxState.isIndeterminate = true;
  }
  applyMainCheckboxState();
}

// === Apply Main Checkbox State ===
async function applyMainCheckboxState() {
  $("#maincheckbox")
    .prop("checked", mainCheckboxState.isChecked)
    .prop("indeterminate", mainCheckboxState.isIndeterminate);
}
