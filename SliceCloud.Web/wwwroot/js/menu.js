// For the Items
let selectedItems = new Set();
let mainCheckboxState = { isChecked: false, isIndeterminate: false };
let allItemIds = new Set();
let currentPage = 1;
let pageSize = 5;
let totalPages = 1;
let selectedCategoryId = null;

// For the Modifiers
let selectedModifiers = new Set();
let mainCheckboxStateModifier = { isChecked: false, isIndeterminate: false };
let allModifierIds = new Set();
let currentPageModifier = 1;
let pageSizeModifier = 5;
let totalPagesModifier = 1;
let selectedModifierGroupId = null;

$(document).ready(async function () {
  pageSize = $("#pageSizeDropdown").val();
  pageSizeModifier = $("#pageSizeDropdownModifier").val();

  await loadPartialView("/Menu/LoadItems");

  await loadAllCategories(async function (firstCategoryId) {
    if (firstCategoryId) {
      selectedCategoryId = firstCategoryId;
      $("#categoryIdHidden").val(selectedCategoryId);
      await loadCategoryWiseItems(selectedCategoryId, currentPage, pageSize);
    }
  });

  // Pill Button Handling for Items
  $("#pills-home-tab").click(async function () {
    await loadPartialView("/Menu/LoadItems");
    await loadAllCategories(async function (firstCategoryId) {
      if (firstCategoryId) {
        selectedCategoryId = firstCategoryId;
        $("#categoryIdHidden").val(selectedCategoryId);
        await loadCategoryWiseItems(selectedCategoryId, currentPage, pageSize);
      }
    });
  });

  // Pill Button Handling for Modifiers
  $("#pills-profile-tab").click(async function () {
    await loadPartialView("/Menu/LoadModifiers");
    await loadAllModifierGroups(async function (firstModifierGroupId) {
      if (firstModifierGroupId) {
        selectedModifierGroupId = firstModifierGroupId;
        $("#modifierGroupIdHidden").val(selectedModifierGroupId);
        await laodModifierGroupWiseModifies(
          selectedModifierGroupId,
          currentPageModifier,
          pageSizeModifier
        );
      }
    });
  });
});

// Function to load partial views for both items and modifiers
async function loadPartialView(url) {
  $.ajax({
    url: url,
    type: "GET",
    success: async function (data) {
      $("#menu-content").html(data);

      // Initialize sortable after loading categories
      await initializeCategorySortable();
      // Initialize sortable after loading modifier-groups
      await initializeModifierGroupSortable();
    },
    error: function () {
      toastr.error("Error loading content", "Error");
    },
  });
}

// Paginatin controls Starts
async function updatePaginationControls() {
  totalPages = parseInt($("#totalPages").val()) || 1;
  $("#prevPageBtn").prop("disabled", currentPage <= 1);
  $("#nextPageBtn").prop("disabled", currentPage >= totalPages);
}

// Paginatin controls Starts
async function updatePaginationControlsModifier() {
  totalPagesModifier = parseInt($("#totalPagesModifier").val()) || 1;
  $("#prevPageBtnModifier").prop("disabled", currentPageModifier <= 1);
  $("#nextPageBtnModifier").prop(
    "disabled",
    currentPageModifier >= totalPagesModifier
  );
}
