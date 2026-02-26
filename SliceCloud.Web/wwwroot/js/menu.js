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
});
