$(document).ready(function () {
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
      success: function (response) {
        if (response.success) {
          toastr.success(response.message);
          let modal = document.getElementById("addCategory");
          let modalInstance = new bootstrap.Modal(modal);
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
      success: function (response) {
        if (response.success) {
          toastr.success(response.message);
          let modal = document.getElementById("editCategory");
          let modalInstance = bootstrap.Modal.getOrCreateInstance(modal);
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
      success: function (response) {
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
