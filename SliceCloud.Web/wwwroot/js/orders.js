$(document).ready(function () {
  let currentPage = 1;
  let totalPages = parseInt($("#totalPages").val()) || 1;
  let pageSize = parseInt($("#pageSizeDropdown").val()) || 5;
  let sortColumn = "OrderDate";
  let sortOrder = "ac";

  function loadOrders() {
    $.ajax({
      url: "/Orders/LoadOrders",
      type: "GET",
      data: {
        search: $("#search").val(),
        status: $("#statusFilter").val(),
        timeRange: $("#timeFilter").val(),
        startDate: $("#startDate").val(),
        endDate: $("#endDate").val(),
        sortColumn: sortColumn,
        sortOrder: sortOrder,
        page: currentPage,
        pageSize: pageSize,
      },
      success: function (response) {
        $("#ordersContainer").html(response);
        totalPages = parseInt($("#totalPages").val()) || 1;
        updatePaginationControls();
      },
      error: function (xhr, status, error) {
        toastr.error("AJAX Error: " + xhr.responseText, "Request Failed");
      },
    });
  }
  loadOrders();

  $(document).on("click", ".sortable-column", function () {
    let column = $(this).data("column");
    if (sortColumn === column) {
      sortOrder = sortOrder === "asc" ? "desc" : "asc";
    } else {
      sortColumn = column;
      sortOrder = "asc";
    }
    $(".sortable-column").removeAttr("data-sort-order");
    $(this).attr("data-sort-order", sortOrder);

    let query = $("#search").val();
    let status = $("#statusFilter").val();
    let timeRange = $("#timeFilter").val();
    let startDate = $("#startDate").val();
    let endDate = $("#endDate").val();

    loadOrders(
      query,
      status,
      timeRange,
      startDate,
      endDate,
      currentPage,
      pageSize
    );
  });

  function updateSortingIcons() {
    $(".sortable-column i").css("color", "#ccc");
    $(".sortable-column").each(function () {
      let column = $(this).data("column");
      if (column === sortColumn) {
        $(this)
          .find(sortOrder === "asc" ? ".bi-arrow-up" : ".bi-arrow-down")
          .css("color", "black");
      }
    });
  }

  $("#startDate").on("change", function () {
    let startDate = new Date($("#startDate").val());
    let today = new Date();

    if ($("#startDate").val()) {
      $("#endDate").attr("min", $("#startDate").val());
      $("#endDate").attr("max", today.toISOString().split("T")[0]);
    }
  });

  $("#endDate").on("change", function () {
    let endDate = new Date($("#endDate").val());

    if ($("#endDate").val()) {
      $("#startDate").attr("max", $("#endDate").val());
    }
  });

  function updateDateRange() {
    let timeRange = $("#timeFilter").val();
    let today = new Date();
    let pastDate = new Date();

    if (timeRange === "all") {
      $("#startDate").val("");
      $("#endDate").val("");
    }
    if (timeRange === "7") {
      pastDate.setDate(today.getDate() - 7);
    } else if (timeRange === "30") {
      pastDate.setDate(today.getDate() - 30);
    } else if (timeRange === "month") {
      pastDate = new Date(today.getFullYear(), today.getMonth(), 2);
      today = new Date();
    } else if (timeRange === "year") {
      pastDate = new Date(today.getFullYear(), 0, 2);
      today = new Date();
    } else {
      return;
    }

    $("#startDate").val(pastDate.toISOString().split("T")[0]);
    $("#endDate").val(today.toISOString().split("T")[0]);
  }

  $("#search, #statusFilter, #timeFilter").on("change keyup", function () {
    updateDateRange();

    let query = $("#search").val();
    let status = $("#statusFilter").val();
    let timeRange = $("#timeFilter").val();
    let startDate = $("#startDate").val();
    let endDate = $("#endDate").val();

    loadOrders(query, status, timeRange, startDate, endDate);
  });

  $(".btn-primary").on("click", function () {
    let query = $("#search").val();
    let status = $("#statusFilter").val();
    let timeRange = $("#timeFilter").val();
    let startDate = $("#startDate").val();
    let endDate = $("#endDate").val();

    loadOrders(query, status, timeRange, startDate, endDate);
  });

  $("#clear").on("click", function () {
    $("#search").val("");
    $("#statusFilter").val("");
    $("#timeFilter").val("all");
    $("#startDate").val("");
    $("#endDate").val("");

    loadOrders();
  });

  $("#startDate, #endDate").on("change", function () {
    $("#timeFilter").val("");
  });

  $("#exportBtn").click(function () {
    let query = $("#search").val();
    let startDate = $("#startDate").val();
    let endDate = $("#endDate").val();
    let status = $("#statusFilter").val();

    $.ajax({
      url: "/Orders/ExportOrders",
      type: "GET",
      data: {
        searchText: query,
        startDate: startDate,
        endDate: endDate,
        orderStatus: status,
        sortColumn: sortColumn,
        sortOrder: sortOrder,
      },
      xhrFields: { responseType: "blob" },
      success: function (data, status, xhr) {
        let disposition = xhr.getResponseHeader("Content-Disposition");
        let filename =
          "Orders_" + new Date().toISOString().replace(/[:.]/g, "_") + ".xlsx";

        if (disposition && disposition.indexOf("filename=") !== -1) {
          let matches = disposition.match(/filename="([^"]+)"/);
          if (matches != null && matches[1]) filename = matches[1];
        }

        let blob = new Blob([data], {
          type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        });
        saveAs(blob, filename);
      },
      error: function (xhr) {
        if (xhr.status === 403) {
          toastr.error(
            "Access Denied: You don't have permission to perform this action."
          );
        } else {
          toastr.error("An error occurred while exporting orders.");
        }
      },
    });
  });

  $(document).on("click", "#downloadInvoice", function () {
    var orderId = $(this).data("orderid");

    $.ajax({
      url: "/Orders/downloadInvoice",
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

  $(document).on("click", ".view-details", function () {
    let orderId = $(this).data("orderid");
    $.ajax({
      url: "/Orders/OrderDetails",
      type: "GET",
      data: { orderId: orderId },
      success: function (response) {
        $("#orderModal .modal-content").html(response);
        $("#orderModal").modal("show");
      },
    });
  });

  function updatePaginationControls() {
    $("#prevPageBtn").prop("disabled", currentPage <= 1);
    $("#nextPageBtn").prop("disabled", currentPage >= totalPages);
  }

  $(document).on("click", "#nextPageBtn", function () {
    if (currentPage < totalPages) {
      currentPage++;
      loadOrders();
    }
  });

  $(document).on("click", "#prevPageBtn", function () {
    if (currentPage > 1) {
      currentPage--;
      loadOrders();
    }
  });

  $(document).on("change", "#pageSizeDropdown", function () {
    pageSize = $(this).val();
    currentPage = 1;
    loadOrders();
  });
});
