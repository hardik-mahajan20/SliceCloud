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
        applyMainCheckboxState();
        fetchAllItemIds();
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
            $("#categoryIdHidden").val(selectedCategoryId);
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
});
