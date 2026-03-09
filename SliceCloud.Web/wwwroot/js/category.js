$(document).ready(function () {
  // Load category based on category click
  $(document).on("click", ".category-btn", function () {
    $(".category-btn").removeClass("active-category");
    $(this).addClass("active-category");
    $("#itemSearch").val("");

    selectedCategoryId = $(this).data("id");

    $("#categoryIdHidden").val(selectedCategoryId);
    loadCategoryWiseItems(selectedCategoryId, 1, 5, "");
  });

  // Add Category
  $(document).on("click", "#openAddCategoryModal", function () {
    $.ajax({
      url: "/Menu/LoadAddCategoryModal",
      type: "GET",
      success: function (data) {
        $("#modalContainer").empty();
        $("#modalContainer").html(data);
        let modal = document.getElementById("addCategory");
        let modalInstance = new bootstrap.Modal(modal);
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
      success: async function (response) {
        if (response.success) {
          toastr.success(response.message);
          let modal = document.getElementById("addCategory");
          let modalInstance = bootstrap.Modal.getOrCreateInstance(modal);
          modalInstance.hide();
          await loadAllCategories();
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
  $(document).on("click", ".edit-category-btn", function (e) {
    e.stopPropagation();
    var categoryId = $(this).data("id");

    $.ajax({
      type: "GET",
      url: "/Menu/GetCategoryById/" + categoryId,
      success: function (response) {
        $("#modalContainer").empty();
        $("#modalContainer").html(response);
        let modal = document.getElementById("editCategory");
        let modalInstance = new bootstrap.Modal(modal);
        modalInstance.show();
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
      success: async function (response) {
        if (response.success) {
          toastr.success(response.message);
          let modal = document.getElementById("editCategory");
          let modalInstance = bootstrap.Modal.getOrCreateInstance(modal);
          modalInstance.hide();
          await loadAllCategories(function (firstCategoryId) {
            if (firstCategoryId) {
              selectedCategoryId = firstCategoryId;
              $("#categoryIdHidden").val(selectedCategoryId);
              loadCategoryWiseItems(selectedCategoryId, currentPage, pageSize);
            }
          });
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

  // Delete Category
  $(document).on("click", ".delete-category-btn", function (e) {
    e.stopPropagation();
    var categoryId = $(this).data("id");

    $.ajax({
      type: "GET",
      url: "/Menu/LoadDeleteCategoryModal",
      success: function (response) {
        $("#modalContainer").empty();
        $("#modalContainer").html(response);
        $("#deleteCategoryId").val(categoryId);
        let modal = document.getElementById("deleteCategoryModal");
        let modalInstance = new bootstrap.Modal(modal);
        modalInstance.show();
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
      success: async function (response) {
        if (response.success) {
          toastr.success("Category deleted successfully!");
          let modal = document.getElementById("deleteCategoryModal");
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
});

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

// Load all categories and execute callback with the first category ID
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
