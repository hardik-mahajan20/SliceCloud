$(document).ready(function () {
  loadPartialView("/Menu/LoadItems");

  function loadPartialView(url) {
    $.ajax({
      url: url,
      type: "GET",
      success: function (data) {
        $("#menu-content").html(data);

        // Load all the categories
        loadAllCategories();

        // Initialize sortable after loading categories
        initializeCategorySortable();
      },
      error: function () {
        toastr.error("Error loading content", "Error");
      },
    });
  }

  // Initialize sortable for categories
  function initializeCategorySortable() {
    $("#categoryList").sortable({
      update: function (event, ui) {
        var sortedIDs = $(this)
          .sortable("toArray")
          .map(function (id) {
            return id.split("-")[1];
          });

        $.ajax({
          url: "/Menu/UpdateCategoryOrder",
          type: "POST",
          contentType: "application/json",
          data: JSON.stringify(sortedIDs),
          success: function () {
            toastr.success("Category order updated successfully!");
          },
          error: function () {
            toastr.error("An unexpected error occurred.", "Error");
          },
        });
      },
    });
  }

  // Add Category
  $(document).on("click", "#openAddCategoryModal", function () {
    $.ajax({
      url: "/Menu/LoadAddCategoryModal",
      type: "GET",
      success: function (data) {
        $("#modalContainer").html(data);
        let addCategoryModal = document.getElementById("addCategory");
        let modalInstance = new bootstrap.Modal(addCategoryModal);
        modalInstance.show();
      },
      error: function () {
        toastr.error("An unexpected error occurred.", "Error");
      },
    });
  });

  // Add Category Submission
  $(document).on("submit", "#addCategoryForm", function (e) {
    e.preventDefault();

    var form = $(this);
    $.ajax({
      url: form.attr("action"),
      type: form.attr("method"),
      data: form.serialize(),
      success: function (response) {
        if (response.success) {
          toastr.success(response.message);
          let addCategoryModal = document.getElementById("addCategory");
          let modalInstance =
            bootstrap.Modal.getOrCreateInstance(addCategoryModal);
          modalInstance.hide();
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

  // Edit Category
  $(document).on("click", ".edit-category-btn", function () {
    var categoryId = $(this).data("id");

    $.ajax({
      type: "GET",
      url: "/Menu/GetCategoryById/" + categoryId,
      success: function (response) {
        $("#modalContainer").empty();
        $("#modalContainer").html(response);
        let editCategoryModal = document.getElementById("editCategory");
        let editCategoryModalInstance = new bootstrap.Modal(editCategoryModal);
        editCategoryModalInstance.show();
      },
      error: function () {
        toastr.error("An unexpected error occurred.", "Error");
      },
    });
  });

  // Edit Category Submission
  $(document).on("submit", "#editCategoryForm", function (e) {
    e.preventDefault();
    const form = $(this)[0];
    const formData = new FormData(form);

    $.ajax({
      type: "POST",
      url: "/Menu/EditCategory",
      data: formData,
      processData: false,
      contentType: false,
      success: function (response) {
        if (response.success) {
          toastr.success(response.message);
          let editCategoryModal = document.getElementById("editCategory");
          let modalInstance =
            bootstrap.Modal.getOrCreateInstance(editCategoryModal);
          modalInstance.hide();
        } else {
          // Clear all old errors first
          $("#editCategoryForm .text-danger").text("");

          // Display validation errors inline
          if (response.errors) {
            for (const key in response.errors) {
              const errorMessages = response.errors[key].join(", ");
              $(`[name="${key}"]`).siblings(".text-danger").text(errorMessages);
            }
          }
        }
      },
      error: function () {
        toastr.error("An unexpected error occurred.");
      },
    });
  });

  // Delete Category
  $(document).on("click", ".delete-category-btn", function () {
    var categoryId = $(this).data("id");
    $.ajax({
      type: "GET",
      url: "/Menu/LoadDeleteCategoryModal",
      success: function (response) {
        $("#modalContainer").empty();
        $("#modalContainer").html(response);
        $("#deleteCategoryId").val(categoryId);
        let deleteCategoryModal = document.getElementById(
          "deleteCategoryModal"
        );
        let deleteCategoryModalInstance = new bootstrap.Modal(
          deleteCategoryModal
        );
        deleteCategoryModalInstance.show();
      },
      error: function () {
        toastr.error("An unexpected error occurred.");
      },
    });
  });

  // Delete Category Submission
  $(document).on("submit", "#deleteCategoryForm", function (e) {
    e.preventDefault();
    var categoryId = $("#deleteCategoryId").val();
    $.ajax({
      type: "POST",
      url: "/Menu/DeleteCategory/" + categoryId,
      data: {
        categoryId: categoryId,
      },
      dataType: "json",
      success: function (response) {
        if (response.success) {
          toastr.success("Category deleted successfully!");
          let deleteCategoryModal = document.getElementById(
            "deleteCategoryModal"
          );
          let modalInstance =
            bootstrap.Modal.getOrCreateInstance(deleteCategoryModal);
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

  // Function to load items dynamically

  function loadCategoryWiseItems(
    categoryId,
    pageNumber = 1,
    pageSize = 5,
    searchQuery = ""
  ) {
    $.ajax({
      url: "/Menu/LoadItemsByCategory",
      type: "GET",
      data: {
        categoryId: categoryId,
        pageNumber: pageNumber,
        pageSize: pageSize,
        searchQuery: searchQuery,
      },
      success: function (data) {
        $("#items-container").html(data);
      },
      error: function () {
        toastr.error("An unexpected error occurred.");
      },
    });
  }

  function loadAllCategories(callback) {
    $.ajax({
      url: "/Menu/GetAllCategories",
      type: "GET",
      success: function (data) {
        let firstCategoryId = null;

        if (Array.isArray(data) && data.length > 0) {
          firstCategoryId = data[0].id || data[0].categoryId;

          if (firstCategoryId) {
            selectedCategoryId = firstCategoryId;
            loadCategoryWiseItems(selectedCategoryId, 1, 5);
          }

          if (callback) callback(firstCategoryId);
        } else {
          toastr.warning("No categories found!", "Warning");
        }
      },
      error: function () {
        toastr.error("An unexpected error occurred.");
      },
    });
  }

  // Add Item GET
  let selectedModifierGroupData = [];
  let selectedModifiersData = {};

  // Add Item GET

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
        $("#modalContainer").html(data);

        let addMenuItemModal = document.getElementById("addItemModalContainer");
        let modalInstance = new bootstrap.Modal(addMenuItemModal);
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
      $.each(data.groups, function (index, group) {
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
          let addItemModal = document.getElementById("addItemModalContainer");
          let modalInstance = bootstrap.Modal.getOrCreateInstance(addItemModal);
          modalInstance.hide();
          loadItems(response.categoryId, 1, 5, "");
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
          toastr.warning("No modifier mappings found for this item.", "Warning");
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
            <span>${
              item.modifierItemName
            }</span>
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
});
