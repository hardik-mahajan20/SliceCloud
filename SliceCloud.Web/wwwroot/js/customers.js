let currentPage = 1;
let totalPages = 1;
let pageSize = 5;
let sortColumn = "";
let sortOrder = "";
$(document).ready(function () {
  totalPages = parseInt($("#totalPages").val()) || 1;
  pageSize = parseInt($("#pageSizeDropdown").val()) || 5;
  sortColumn = "CustomerName";
  sortOrder = "asc";
  loadCustomers();

  $(document).on("click", ".sort-arrow", function (e) {
    e.stopPropagation();

    let column = $(this).closest(".sortable-column").data("column");
    sortColumn = column;
    sortOrder = $(this).data("order");

    loadCustomers();
  });

  $("#startDate").on("change", function () {
    let today = new Date();

    if ($("#startDate").val()) {
      $("#endDate").attr("min", $("#startDate").val());
      $("#endDate").attr("max", today.toISOString().split("T")[0]);
    }
  });

  $("#endDate").on("change", function () {
    if ($("#endDate").val()) {
      $("#startDate").attr("max", $("#endDate").val());
    }
  });

  $("#applyCustomDate").on("click", function () {
    let startDate = $("#customStartDate").val();
    let endDate = $("#customEndDate").val();

    if (!startDate || !endDate) {
      toastr.warning("Please select both start and end dates.");
      return;
    }

    $("#startDate").val(startDate);
    $("#endDate").val(endDate);

    $("#customDateModal").modal("hide");

    loadCustomers();
    $("#customStartDate").val("");
    $("#customEndDate").val("");
  });

  $("#search, #statusFilter, #timeFilter").on("change keyup", function () {
    updateDateRange();

    loadCustomers();
  });

  $(document).on("click", "#nextPageBtn", function () {
    if (currentPage < totalPages) {
      currentPage++;

      loadCustomers();
    }
  });

  $(document).on("click", "#prevPageBtn", function () {
    if (currentPage > 1) {
      currentPage--;

      loadCustomers();
    }
  });

  $(document).on("change", "#pageSizeDropdown", function () {
    pageSize = $(this).val();
    currentPage = 1;

    loadCustomers();
  });

  $("#exportBtn").click(function () {
    let query = $("#search").val();
    let startDate = $("#startDate").val();
    let endDate = $("#endDate").val();
    let status = $("#statusFilter").val();

    $.ajax({
      url: "/Customers/ExportCustomers",
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
        if (data.success === false) {
          toastr.warning(data.message, "Export Warning");
        } else {
          let disposition = xhr.getResponseHeader("Content-Disposition");
          let filename =
            "Customers_" +
            new Date().toISOString().replace(/[:.]/g, "_") +
            ".xlsx";

          if (disposition && disposition.indexOf("filename=") !== -1) {
            let matches = disposition.match(/filename="([^"]+)"/);
            if (matches != null && matches[1]) filename = matches[1];
          }

          let blob = new Blob([data], {
            type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
          });

          if (window.navigator && window.navigator.msSaveOrOpenBlob) {
            window.navigator.msSaveOrOpenBlob(blob, filename);
          } else {
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement("a");
            a.href = url;
            a.download = filename;
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
            document.body.removeChild(a);
          }

          toastr.success("File downloaded successfully", "Export Success");
        }
      },
      error: function (xhr, status, error) {
        if (xhr.status === 0) {
          toastr.error(
            "Network error - check if server is running",
            "Connection Error",
          );
        } else if (xhr.status === 404) {
          toastr.error("Export endpoint not found (404)", "URL Error");
        } else if (xhr.status === 500) {
          toastr.error(
            "Server error (500) - check server logs",
            "Server Error",
          );
        } else {
          toastr.warning("No records found to download");
        }
      },
    });
  });

  $(document).on("click", ".customer-row", function () {
    var customerId = $(this).data("id");

    $.ajax({
      url: "/Customers/GetCustomerHistory",
      type: "GET",
      data: { id: customerId },
      success: function (response) {
        if (response.success) {
          $("#customerName").text(response.data.name);
          $("#customerPhone").text(response.data.phoneNumber);
          $("#customerMaxOrder").text(response.data.maxOrder);
          $("#customerAvgBill").text(response.data.avgBill);
          $("#customerComingSince").text(response.data.comingSince);
          $("#customerVisits").text(response.data.visits);

          var orderTableBody = $("#orderTableBody");
          orderTableBody.empty(); // Clear existing rows

          $.each(response.data.orders, function (index, order) {
            var row = `<tr>
                              <td>${order.orderDate}</td>
                              <td>${
                                order.orderType || "N/A"
                              }</td>  <!-- Ensure fallback value -->
                              <td>${
                                order.paymentMode || "N/A"
                              }</td>  <!-- Fix mismatch (was paymentStatus) -->
                              <td>${
                                order.items ?? 0
                              }</td>  <!-- Fix mismatch (was items, now itemsCount) -->
                              <td>${order.amount}</td>
                              </tr>`;
            orderTableBody.append(row);
          });

          $("#customerHistoryModal").modal("show");
        } else {
          toastr.error(
            "Error exporting orders: " + xhr.responseText,
            "Export Error",
          );
        }
      },
      error: function () {
        toastr.error("Error fetching customer history.", "Fetch Error");
      },
    });
  });
});
function loadCustomers() {
  $.ajax({
    url: "/Customers/LoadCustomers",
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
      $("#customerTableContainer").html(response);
      totalPages = parseInt($("#totalPages").val()) || 1;
      updatePaginationControls();
      updateSortingIcons();
    },
    error: function (xhr, status, error) {
      toastr.warning(
        "There is something error please contact the support team",
      );
    },
  });
}
function updateSortingIcons() {
  $(".sortable-column i").css("color", "#ccc");

  $(`.sortable-column[data-column="${sortColumn}"]`).each(function () {
    if (sortOrder === "asc") {
      $(this).find(".bi-arrow-up").css("color", "black");
      $(this).find(".bi-arrow-down").css("color", "#ccc");
    } else {
      $(this).find(".bi-arrow-up").css("color", "#ccc");
      $(this).find(".bi-arrow-down").css("color", "black");
    }
  });
}
function updateDateRange() {
  let timeRange = $("#timeFilter").val();
  let today = new Date();
  let pastDate = new Date();

  if (timeRange === "all") {
    $("#startDate").val("");
    $("#endDate").val("");
    loadCustomers();
    return;
  } else if (timeRange === "7") {
    pastDate.setDate(today.getDate() - 7);
  } else if (timeRange === "30") {
    pastDate.setDate(today.getDate() - 30);
  } else if (timeRange === "currentDate") {
    pastDate = today;
  } else if (timeRange === "month") {
    pastDate = new Date(today.getFullYear(), today.getMonth(), 1);
  } else if (timeRange === "year") {
    pastDate = new Date(today.getFullYear(), 0, 1);
  } else if (timeRange === "custom") {
    $("#customDateModal").modal("show");
    return;
  } else {
    return;
  }

  $("#startDate").val(pastDate.toISOString().split("T")[0]);
  $("#endDate").val(today.toISOString().split("T")[0]);
}
function updatePaginationControls() {
  $("#prevPageBtn").prop("disabled", currentPage <= 1);
  $("#nextPageBtn").prop("disabled", currentPage >= totalPages);
}
