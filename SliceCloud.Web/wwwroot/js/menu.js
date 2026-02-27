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

  // Edit Cateogry
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
});
