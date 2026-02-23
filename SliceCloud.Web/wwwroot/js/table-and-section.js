var selectedSectionId = null;
var currentPage = 1;
var pageSize = 5;
let tableSearchQuery = $("#tableSearch").val() || "";
let selectedTables = new Set();
let allTableIds = new Set();
var isNewSectionAdded = false;

$(document).ready(function () {
  loadTableSection();
});

var selectedSectionId = null;

function loadTableSection(newSectionAdded = false) {
  isNewSectionAdded = newSectionAdded;

  var currentSelectedId = selectedSectionId;

  $.ajax({
    url: "/TableAndSection/LoadTableSection/",
    type: "GET",
    success: function (data) {
      $("#tableSectionContainer").html(data);

      if (isNewSectionAdded) {
        var lastSection = $("#sectionList .section-btn").last();
        if (lastSection.length > 0) {
          $(".section-btn").removeClass("active-section");
          lastSection.addClass("active-section");
          selectedSectionId = lastSection.data("id");
        }
        isNewSectionAdded = false;
      } else if (currentSelectedId) {
        var previouslySelectedSection = $(
          "#sectionList .section-btn[data-id='" + currentSelectedId + "']"
        );

        if (previouslySelectedSection.length > 0) {
          $(".section-btn").removeClass("active-section");
          previouslySelectedSection.addClass("active-section");
          selectedSectionId = currentSelectedId;
        } else {
          var firstSection = $("#sectionList .section-btn").first();
          if (firstSection.length > 0) {
            firstSection.addClass("active-section");
            selectedSectionId = firstSection.data("id");
          }
        }
      } else {
        var firstSection = $("#sectionList .section-btn").first();
        if (firstSection.length > 0) {
          firstSection.addClass("active-section");
          selectedSectionId = firstSection.data("id");
        }
      }

      if (selectedSectionId) {
        currentPage = 1;
        let tableSearchQuery = $("#tableSearch").val() || "";
        $("#sectionIdHidden").val(selectedSectionId);
        loadTablesPaginated(
          selectedSectionId,
          currentPage,
          pageSize,
          tableSearchQuery
        );
      }
    },
    error: function (xhr) {
      toastr.error("Failed to load data. Check for details.", "Error");
    },
  });
}

$(document).on("click", ".section-btn", function () {
  $(".section-btn").removeClass("addedit-section");
  $(this).addClass("addedit-section");
  $(".section-btn").removeClass("active-section");
  $(this).addClass("active-section");
  $("#iteSsearch").val("");

  selectedSectionId = $(this).data("id");
  currentPage = 1;
  $("#sectionIdHidden").val(selectedSectionId);
  let tableSearchQuery = $("#tableSearch").val() || "";
  selectedTables.clear();
  allTableIds.clear();
  loadTablesPaginated(
    selectedSectionId,
    currentPage,
    pageSize,
    tableSearchQuery
  );
});

function loadTablesPaginated(
  sectionId,
  pageNumber,
  pageSize,
  searchQuery = ""
) {
  $.ajax({
    url: "/TableAndSection/LoadTablesPaginated/",
    type: "GET",
    data: {
      sectionId: sectionId,
      pageNumber: pageNumber,
      pageSize: pageSize,
      searchQuery: searchQuery,
    },
    success: function (data) {
      $("#tablesContainer").html(data);
      if (searchQuery !== "") {
        $("#mainCheckBoxTable").addClass("d-none");
      }
      updatePaginationControls();

      fetchAllTableIds().then(() => {
        updateCheckboxesFromSelection();
        updateMainCheckboxState();
      });
    },
    error: function (xhr) {
      toastr.error("Failed to load paginated tables.", "Error");
    },
  });
}

function updatePaginationControls() {
  let totalPages = parseInt($("#totalPages").val()) || 1;
  let totalRecords = parseInt($("#totalRecords").val()) || 0;
  $("#prevPageBtn").prop("disabled", currentPage <= 1);
  $("#nextPageBtn").prop("disabled", currentPage >= totalPages);
  let fromRecord = (currentPage - 1) * pageSize + 1;
  let toRecord = Math.min(currentPage * pageSize, totalRecords);
  $("#showingInfo").text(
    `Showing ${fromRecord} - ${toRecord} of ${totalRecords}`
  );
}
$(document).on("click", "#prevPageBtn", function () {
  if (currentPage > 1) {
    currentPage--;
    let tableSearchQuery = $("#tableSearch").val() || "";
    loadTablesPaginated(
      selectedSectionId,
      currentPage,
      pageSize,
      tableSearchQuery
    );
  }
});

$(document).on("click", "#nextPageBtn", function () {
  var totalPages = parseInt($("#totalPages").val());
  if (currentPage < totalPages) {
    currentPage++;
    let tableSearchQuery = $("#tableSearch").val() || "";
    loadTablesPaginated(
      selectedSectionId,
      currentPage,
      pageSize,
      tableSearchQuery
    );
  }
});

$(document).on("change", "#pageSizeDropdown", function () {
  pageSize = parseInt($(this).val());
  currentPage = 1;
  let tableSearchQuery = $("#tableSearch").val() || "";
  loadTablesPaginated(
    selectedSectionId,
    currentPage,
    pageSize,
    tableSearchQuery
  );
});

$(document).on("keyup", "#tableSearch", function () {
  currentPage = 1;
  let tableSearchQuery = $("#tableSearch").val() || "";
  loadTablesPaginated(
    selectedSectionId,
    currentPage,
    pageSize,
    tableSearchQuery
  );
});

$(document).on("click", "#addsection", function () {
  $.ajax({
    url: "/TableAndSection/GetAddSectionModal/",
    type: "GET",
    success: function (response) {
      $("#modalPlaceholder").html(response);

      $("#addSection").modal("show");

      $("#addSection").on("shown.bs.modal", function () {});
    },
    error: function () {
      toastr.error("Failed to load the modal.", "Error");
    },
  });
});

$(document).on("submit", "#addSectionForm", function (e) {
  e.preventDefault();
  clearSectionFormErrors();

  var sectionName = $("#SectionNameAdd").val();
  var description = $("#DescriptionAdd").val();

  if (!description.trim()) {
    description = null;
  }

  var formData = {
    SectionName: sectionName,
    Description: description,
  };

  $.ajax({
    type: "POST",
    url: "/TableAndSection/AddSection/",
    contentType: "application/json",
    data: JSON.stringify(formData),
    success: function (response) {
      if (response.success) {
        $("#SectionNameAdd").val("");
        $("#DescriptionAdd").val("");
        toastr.success("Section added successfully!");
        loadTableSection(true);

        $("#addSection").modal("hide");
      } else {
        if (response.validationErrors) {
          $(".text-danger").remove();

          for (let field in response.validationErrors) {
            const errorMessage = response.validationErrors[field];
            $(`[name="${field}"]`, document).after(
              `<span class="text-danger">${errorMessage}</span>`
            );
          }
        } else {
          toastr.error("Error adding section.", "Error");
        }
      }
    },
    error: function () {
      toastr.error("Error adding section.", "Error");
    },
  });
});

function clearSectionFormErrors() {
  $("#addSectionForm .text-danger").text("");
  $("#addSectionForm .input-validation-error").removeClass(
    "input-validation-error"
  );
  $("#editSectionForm .text-danger").text("");
  $("#editSectionForm .input-validation-error").removeClass(
    "input-validation-error"
  );
}

$("#addSection").on("hidden.bs.modal", function () {
  clearSectionFormErrors();
  $("#addSectionForm")[0].reset();
});
$("#editSection").on("hidden.bs.modal", function () {
  clearSectionFormErrors();
  $("#editSectionForm")[0].reset();
});

$(document).on("click", ".edit-section-btn", function () {
  var sectionId = $(this).data("id");

  $.ajax({
    type: "GET",
    url: "/TableAndSection/GetSectionById/" + sectionId,
    success: function (response) {
      $("#modalPlaceholder").html(response);
      clearSectionFormErrors();
      $("#editSection").modal("show");

      $("#editSection").on("shown.bs.modal", function () {});
    },
    error: function (xhr, status, error) {
      toastr.error("Failed to load category details.");
    },
  });
});
const editCategoryModalEl = document.getElementById("editCategory");

if (editCategoryModalEl) {
  editCategoryModalEl.addEventListener("hidden.bs.modal", function () {
    $("#modalPlaceholder").empty();
  });
}

$(document).on("submit", "#editSectionForm", function (e) {
  e.preventDefault();

  const form = $(this)[0];
  const formData = new FormData(form);

  $.ajax({
    type: "POST",
    url: "/TableAndSection/EditSection",
    data: formData,
    processData: false,
    contentType: false,
    success: function (response) {
      if (response.success) {
        toastr.success(response.message);
        let editCategoryModal = document.getElementById("editSection");
        let modalInstance =
          bootstrap.Modal.getOrCreateInstance(editCategoryModal);
        modalInstance.hide();
        $("#modalPlaceholder").empty();

        loadTableSection();
      } else {
        $("#editSectionForm .text-danger").text("");
        console.log(response.error);
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

$(document).on("click", ".delete-section-btn", function () {
  var sectionId = $(this).data("id");

  $.ajax({
    type: "GET",
    url: "/TableAndSection/CheckSectionTablesAvailability/",
    data: { sectionId: sectionId },
    success: function (response) {
      if (response.success) {
        $("#deleteSectionId").val(sectionId);
        $("#deleteSectionModal").modal("show");
      } else {
        toastr.error(
          "All tables in this section must have the status 'Available' to delete the section."
        );
      }
    },
    error: function () {
      toastr.error("Error checking table availability in the section.");
    },
  });
});
$("#deleteSectionForm").submit(function (e) {
  e.preventDefault();

  var sectionId = $("#deleteSectionId").val();

  $.ajax({
    type: "POST",
    url: "/TableAndSection/DeleteSection/",
    data: { sectionId: sectionId },
    dataType: "json",
    success: function (response) {
      if (response.success) {
        toastr.success("Section deleted successfully!");
        $("#deleteSectionModal").modal("hide");
        loadTableSection();
      } else {
        toastr.error("Error deleting section.");
      }
    },
    error: function () {
      toastr.error("Error occurred while deleting the section.");
    },
  });
});

$(document).on("click", ".add-table-btn", function () {
  if (!selectedSectionId) {
    toastr.warning("Please select a section first.", "Warning");
    return;
  }

  $.ajax({
    url: "/TableAndSection/GetTableData/",
    type: "GET",
    data: { selectedSectionId: selectedSectionId },
    success: function (data) {
      $("#modalPlaceholder").html(data);
      $("#addTableModal").modal("show");
    },
    error: function () {
      toastr.error("Failed to load modal data.", "Error");
    },
  });
});

$(document).on("click", "#saveTableBtn", function () {
  $(".text-danger[data-valmsg-for]").text("");

  var formData = new FormData();
  formData.append("TableName", $("#tableName").val());
  formData.append("SectionId", $("#tableSection").val());
  formData.append("Capacity", $("#tableCapacity").val());
  formData.append("Status", $("#tableStatus").val());

  $.ajax({
    type: "POST",
    url: "/TableAndSection/AddTable/",
    data: formData,
    processData: false,
    contentType: false,
    success: function (response) {
      if (response.success) {
        toastr.success("Table added successfully!");
        $("#addTableModal").modal("hide");
        loadTableSection();
      } else {
        if (response.validationErrors) {
          $.each(response.validationErrors, function (key, messages) {
            $(`[data-valmsg-for="${key}"]`).text(messages[0]);
          });
        } else {
          toastr.error(response.message || "Error adding table.", "Error");
        }
      }
    },
    error: function () {
      toastr.error("Error adding table.", "Error");
    },
  });
});

$(document).on("click", ".edit-table-btn", function () {
  var tableId = $(this).data("id");
  let status = $(this).data("status");
  if (status !== "Available") {
    toastr.error("Can't edit this table");
  } else {
    $.ajax({
      url: "/TableAndSection/GetTableById",
      type: "GET",
      data: { tableId: tableId },
      success: function (data) {
        $("#modalPlaceholder").html(data);
        $("#editTableModal").modal("show");
      },
      error: function () {
        toastr.error("Failed to load table data.", "Error");
      },
    });
  }
});

$(document).on("click", "#updateTableBtn", function (e) {
  e.preventDefault();

  $(".text-danger[data-valmsg-for]").text("");

  var capacity = $("#editTableCapacity").val();
  if (!capacity || isNaN(capacity) || capacity <= 0) {
    $("[data-valmsg-for='Capacity']").text(
      "Capacity must be a valid number greater than 0."
    );
    return;
  }

  var formData = {
    TableId: $("#TableId").val(),
    TableName: $("#TableName").val(),
    SectionId: $("#editTableSection").val(),
    Capacity: capacity,
    Status: $("#editTableStatus").val(),
  };

  $.ajax({
    type: "POST",
    url: "/TableAndSection/EditTable/",
    data: formData,
    success: function (response) {
      if (response.success) {
        toastr.success("Table updated successfully!");
        $("#editTableModal").modal("hide");
        loadTableSection();
      } else {
        if (response.validationErrors) {
          $.each(response.validationErrors, function (field, messages) {
            $(`[data-valmsg-for="${field}"]`).text(messages[0]);
          });
        } else {
          toastr.error(response.message || "Error updating table.", "Error");
        }
      }
    },
    error: function () {
      toastr.error("Error updating table.", "Error");
    },
  });
});

$(document).on("click", ".delete-item-btn", function (e) {
  e.preventDefault();

  let tableId = $(this).data("id");
  let status = $(this).data("status");

  if (status !== "Available") {
    toastr.error("Can't delete this table");
  } else {
    $("#deleteTableId").val(tableId);
    $("#deleteTableModal").modal("show");
  }
});

$(document).on("submit", "#deleteTableForm", function (e) {
  e.preventDefault();

  let tableId = $("#deleteTableId").val();

  $.ajax({
    type: "POST",
    url: "/TableAndSection/DeleteTable",
    contentType: "application/json",
    data: JSON.stringify(tableId),
    dataType: "json",
    beforeSend: function () {},
    success: function (response) {
      if (response.success) {
        toastr.success("Table deleted successfully!");
        $("#deleteTableModal").modal("hide");
        loadTableSection();
      } else {
        toastr.error(response.message || "Failed to delete the table.");
      }
    },
    error: function (xhr, status, error) {
      toastr.error("Error occurred while deleting the table.");
    },
  });
});

function fetchAllTableIds() {
  let sectionId = selectedSectionId || $("#sectionIdHidden").val();
  return new Promise((resolve, reject) => {
    $.ajax({
      url: "/TableAndSection/GetAllTableIds",
      type: "GET",
      data: { sectionId: sectionId },
      success: function (response) {
        allTableIds = new Set(response);
        resolve();
      },
      error: function () {
        toastr.error("Failed to fetch item IDs.");
        reject();
      },
    });
  });
}

function updateCheckboxesFromSelection() {
  $(".child-checkbox").each(function () {
    const tableId = parseInt(
      $(this).closest("tr").find("input[name='Tableid']").val()
    );
    $(this).prop("checked", selectedTables.has(tableId));
  });
}

$(document).on("change", "#mainCheckBoxTable", function () {
  const isChecked = this.checked;

  $(this).prop("indeterminate", false);

  if (isChecked) {
    allTableIds.forEach((id) => selectedTables.add(id));
  } else {
    selectedTables.clear();
  }

  $(".child-checkbox").prop("checked", isChecked);
});

$(document).on("change", ".child-checkbox", function () {
  const tableId = parseInt(
    $(this).closest("tr").find("input[name='Tableid']").val()
  );

  if (this.checked) {
    selectedTables.add(tableId);
  } else {
    selectedTables.delete(tableId);
  }

  updateMainCheckboxState();
});

function updateMainCheckboxState() {
  const mainCheckbox = $("#mainCheckBoxTable");

  if (selectedTables.size === 0) {
    mainCheckbox.prop("checked", false);
    mainCheckbox.prop("indeterminate", false);
  } else if (selectedTables.size === allTableIds.size) {
    mainCheckbox.prop("checked", true);
    mainCheckbox.prop("indeterminate", false);
  } else {
    mainCheckbox.prop("checked", false);
    mainCheckbox.prop("indeterminate", true);
  }
}

$(document).on("click", "#massDeleteBtnTable", function () {
  selectedTablesArray = Array.from(selectedTables);
  if (selectedTablesArray.length === 0) {
    toastr.warning("Please select at least one table to delete.");
    return;
  }
  $("#deleteConfirmationModal").modal("show");
});

$(document).on("click", "#confirmDelete", function () {
  $("#deleteConfirmationModal").modal("hide");

  $.ajax({
    url: "/TableAndSection/DeleteMultipleTable",
    type: "POST",
    contentType: "application/json",
    data: JSON.stringify(Array.from(selectedTables)),
    success: function (response) {
      if (response.success) {
        toastr.success("Tables deleted successfully.");

        selectedTables.clear();
        mainCheckboxState = { isChecked: false, isIndeterminate: false };

        $("#mainCheckBoxTable")
          .prop("checked", false)
          .prop("indeterminate", false);
        $(".child-checkbox").prop("checked", false);

        loadTableSection();
      } else {
        toastr.error("Error: " + response.message);
      }
    },
    error: function () {
      toastr.error("Something went wrong. Please try again.");
    },
  });
});
