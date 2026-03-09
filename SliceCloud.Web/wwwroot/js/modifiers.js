$(document).ready(function () {
  // Load Add Item Modal
  $(document).on("click", ".add-modifier-btn", function () {
    $.ajax({
      url: "/Menu/GetAddModifier",
      type: "GET",
      success: function (data) {
        $("#modalContainer").empty();
        $("#modalContainer").html(data);

        let modal = document.getElementById("addModifierModalContainer");
        let modalInstance = new bootstrap.Modal(modal);
        modalInstance.show();

        initializeModifierCheckboxes();
        $.validator.unobtrusive.parse("#addModifierModalContainer");
      },
      error: function () {
        toastr.error("Failed to load modal data.", "Error");
      },
    });
  });

  // Add Modifier POST
  $(document).on("submit", "#addModifierForm", function (event) {
    event.preventDefault();

    updateModifierDropdownText();

    let formData = $(this).serialize();

    // Clear previous validation messages
    $("span[data-valmsg-for]").text("");

    $.ajax({
      type: "POST",
      url: "/Menu/AddModifier",
      data: formData,
      success: function (response) {
        if (response.success) {
          toastr.success("Modifier added successfully!");
          let modal = document.getElementById("addModifierModalContainer");
          let modalInstance = bootstrap.Modal.getOrCreateInstance(modal);
          modalInstance.hide();
        } else if (response.validationErrors) {
          // Loop through validation errors and show them under fields
          for (let key in response.validationErrors) {
            const messages = response.validationErrors[key];
            const span = $(`[data-valmsg-for="${key}"]`);
            if (span.length) {
              span.text(messages.join(", "));
            }
          }
        } else {
          toastr.error("Error: " + response.message, "Error");
        }
      },
      error: function () {
        toastr.error("An error occurred while saving the modifier.", "Error");
      },
    });
  });
});

function initializeModifierCheckboxes() {
  $(".modifier-checkbox").change(function () {
    updateModifierDropdownText();
  });

  $("#searchModifier").on("keyup", function () {
    let searchText = $(this).val().toLowerCase();
    $("#modifierCheckboxList li").each(function () {
      $(this).toggle($(this).text().toLowerCase().includes(searchText));
    });
  });
}

function updateModifierDropdownText() {
  let selectedValues = $(".modifier-checkbox:checked")
    .map(function () {
      return $(this).val();
    })
    .get();

  $("#addModifierForm input[name='ModifierGroupIds[]']").remove();

  let form = $("#addModifierForm");
  selectedValues.forEach((value) => {
    form.append(
      `<input type="hidden" name="ModifierGroupIds[]" value="${value}">`
    );
  });

  let selectedText = $(".modifier-checkbox:checked")
    .map(function () {
      return $(this).parent().text().trim();
    })
    .get();

  let buttonText = "Select Modifier Groups";
  if (selectedText.length > 0) {
    buttonText = selectedText[0];
    if (selectedText.length > 1) {
      buttonText += ` +${selectedText.length - 1} Other`;
    }
  }

  $("#modifierDropdownBtn").text(buttonText);
}
