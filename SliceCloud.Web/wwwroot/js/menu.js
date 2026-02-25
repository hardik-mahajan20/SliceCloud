$(document).ready(function () {
  loadPartialView("/Menu/LoadItems");

  function loadPartialView(url) {
    $.ajax({
      url: url,
      type: "GET",
      success: function (data) {
        $("#menu-content").html(data);
      },
      error: function () {
        toastr.error("Error loading content", "Error");
      },
    });
  }
});
