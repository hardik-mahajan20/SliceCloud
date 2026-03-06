// Item Mass Delete Start
let selectedItems = new Set();
let mainCheckboxState = { isChecked: false, isIndeterminate: false };
let allItemIds = new Set();
let currentPage = 1;
let pageSize = 10;
let totalPages = 1;
let selectedCategoryId = null;
$(document).ready(async function () {
  pageSize = $("#pageSizeDropdown").val();

  await loadPartialView("/Menu/LoadItems");

  await loadAllCategories(function (firstCategoryId) {
    if (firstCategoryId) {
      selectedCategoryId = firstCategoryId;
      $("#categoryIdHidden").val(selectedCategoryId);
      loadCategoryWiseItems(selectedCategoryId, currentPage, pageSize);
    }
  });
});

async function loadPartialView(url) {
  $.ajax({
    url: url,
    type: "GET",
    success: function (data) {
      $("#menu-content").html(data);

      // Initialize sortable after loading categories
      initializeCategorySortable();
    },
    error: function () {
      toastr.error("Error loading content", "Error");
    },
  });
}

async function loadAllCategories(callback) {
  $.ajax({
    url: "/Menu/GetAllCategories",
    type: "GET",
    success: function (data) {
      let categoryList = $(".list-group");
      categoryList.empty();
      let firstCategoryId = null;

      if (Array.isArray(data) && data.length > 0) {
        firstCategoryId = data[0].id || data[0].categoryId;

        if (firstCategoryId) {
          $.each(data, function (index, category) {
            let activeClass = index === 0 ? "active-category" : "";
            categoryList.append(`
                    <li class="d-flex p-1 align-items-center justify-content-between category-btn btn ${activeClass}" data-id="${category.categoryId}">
                                       <div class="d-flex align-items-center flex-wrap m-0 gap-2 ms-2">
                        <div class="sort-handle">
                            <i class="bi bi-grip-vertical"></i>
                        </div>
                        <div class="text-truncate category-name">${category.categoryName}</div>
                    </div>
                    <div class="d-flex">
                        <button class="edit-category-btn btn p-0 m-1" data-id="${category.categoryId}">
                            <i class="bi bi-pen"></i>
                        </button>
                        <button class="delete-category-btn btn p-0 m-1" data-id="${category.categoryId}">
                            <i class="bi bi-trash"></i>
                        </button>
                    </div>
                    </li>
                `);
          });

          if (callback) callback(firstCategoryId);
        }
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
    success: async function (data) {
      $("#items-container").html(data);
      updatePaginationControls();

      $(".item-checkbox").each(function () {
        const itemId = parseInt(
          $(this).closest("tr").find("input[name='ItemId']").val()
        );
        $(this).prop("checked", selectedItems.has(itemId));
      });
      if (searchQuery !== "") {
        $("#maincheckbox").addClass("d-none");
      }

      await applyMainCheckboxState();
      await fetchAllItemIds();
    },
    error: function () {
      toastr.error("An unexpected error occurred.");
    },
  });
}

// Paginatin controls Starts
function updatePaginationControls() {
  totalPages = parseInt($("#totalPages").val()) || 1;
  $("#prevPageBtn").prop("disabled", currentPage <= 1);
  $("#nextPageBtn").prop("disabled", currentPage >= totalPages);
}
