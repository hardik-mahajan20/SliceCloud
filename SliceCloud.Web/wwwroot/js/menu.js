// Item Mass Delete Start
let selectedItems = new Set();
let mainCheckboxState = { isChecked: false, isIndeterminate: false };
let allItemIds = new Set();
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
            $("#categoryIdHidden").val(selectedCategoryId);
            loadCategoryWiseItems(selectedCategoryId);
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

  // Function to load items dynamically

  function loadCategoryWiseItems(categoryId) {
    $.ajax({
      url: "/Menu/LoadItemsByCategory",
      type: "GET",
      data: {
        categoryId: categoryId,
      },
      success: async function (data) {
        $("#items-container").html(data);

        await applyMainCheckboxState();
        await fetchAllItemIds();
      },
      error: function () {
        toastr.error("An unexpected error occurred.");
      },
    });
  }
});
