$(document).ready(function () {
  loadPartialView("/Menu/LoadItems");

  function loadPartialView(url) {
    $.ajax({
      url: url,
      type: "GET",
      success: function (data) {
        $("#menu-content").html(data);
        initializeCategorySortable();
      },
      error: function () {
        toastr.error("Error loading content", "Error");
      },
    });
  }

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
          error: function (xhr, _status, _error) {
            toastr.clear();
            var response = xhr.responseJSON;
            if (xhr.status === 401 && response && response.message) {
              toastr.error(response.message);
            } else if (xhr.status === 400) {
              toastr.error("Bad Request: Please check your input.");
            } else if (xhr.status === 500) {
              toastr.error("Internal Server Error: Please try again later.");
            } else {
              toastr.error(
                response?.message || "An unexpected error occurred!"
              );
            }
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
      error: function (xhr) {
        try {
          const response = JSON.parse(xhr.responseText);
          if (response.message === "Unauthorized") {
            toastr.warning(
              "You are not authorized to perform this action.",
              "Unauthorized"
            );
          } else {
            toastr.error("Something went wrong.", "Error");
          }
        } catch {
          toastr.error("An unexpected error occurred.", "Error");
        }
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

          const newCategoryId = response.categoryId;
          loadPartialView("/Menu/LoadItems");
          loadCategories(function (firstCategoryId) {
            if (firstCategoryId) {
              selectedCategoryId = firstCategoryId;
              loadItems(selectedCategoryId, currentPage, pageSize);
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
      error: function (xhr) {
        try {
          const response = JSON.parse(xhr.responseText);
          if (xhr.status === 403 || response.message === "Access Denied") {
            // Access Denied: Redirect to AccessDenied page
            window.location.href = "/Error/AccessDenied";
          } else {
            toastr.error("Something went wrong: " + response.message, "Error");
          }
        } catch (err) {
          toastr.error("Unexpected error.", "Error");
        }
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
        // @* loadPartialView('@Url.Action("LoadItems", "Menu")'); *@
      },
      error: function (xhr, status, error) {
        toastr.error("Failed to load category details.");
      },
    });
  });

  // Edit Category Submission
  $(document).on("submit", "#editCategoryForm", function (e) {
    e.preventDefault();
    var categoryId = $("#editCategoryForm input[name='CategoryId']").val();
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

          // @* loadPartialView('@Url.Action("LoadItems", "Menu")'); *@
          // @* loadPartialView('@Url.Action("LoadItems", "Menu")'); *@
          // loadItems(categoryId, currentPage, pageSize);
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
        // @* loadPartialView('@Url.Action("LoadItems", "Menu")'); *@
      },
      error: function (xhr, status, error) {
        console.log("Status:", xhr.status);
        console.log("Response:", xhr.responseText);
        toastr.error("Failed to load delete category modal.");
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

          // Close modal properly
          $("#deleteCategoryModal").modal("hide");
          $("body").removeClass("modal-open");
          $(".modal-backdrop").remove();

          // loadPartialView('@Url.Action("LoadItems", "Menu")');
        } else {
          toastr.error("Error deleting category.");
        }
      },
      error: function () {
        toastr.error(
          "An unexpected error occurred while deleting the category."
        );
      },
    });
  });
});
