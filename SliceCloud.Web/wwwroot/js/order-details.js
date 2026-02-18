$(document).ready(function () {
  $(document).on("click", "#downloadInvoice", function () {
    var orderId = $(this).data("orderid");

    $.ajax({
      url: "/Orders/DownloadInvoice",
      type: "GET",
      data: { orderId: orderId },
      xhrFields: {
        responseType: "blob",
      },
      success: function (response) {
        var blob = new Blob([response], { type: "application/pdf" });
        var link = document.createElement("a");
        link.href = window.URL.createObjectURL(blob);
        link.download = "Invoice_" + orderId + ".pdf";
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
      },
      error: function (xhr, status, error) {
        toastr.error("Error downloading PDF: " + error, "Download Failed");
      },
    });
  });
});
