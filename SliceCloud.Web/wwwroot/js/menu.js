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
    debugger;
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
});
