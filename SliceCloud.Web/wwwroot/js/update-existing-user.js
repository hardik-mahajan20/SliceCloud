$(document).ready(function () {
  // Country Dropdown Change
  $('#Country').change(function () {
    var id = $(this).val()
    if (id) {
      $.ajax({
        url: '/Users/GetStatesByCountry',
        data: { countryId: id },
        success: function (data) {
          var stateDropdown = $('#State')
          stateDropdown.empty()
          stateDropdown.append('<option value="">Select State</option>')

          data.forEach(function (state) {
            stateDropdown.append(
              '<option value="' +
                state.stateId +
                '">' +
                state.stateName +
                '</option>'
            )
          })
          $('#City').empty().append('<option value="">Select City</option>')
        }
      })
    } else {
      $('#State').empty().append('<option value="">Select State</option>')
      $('#City').empty().append('<option value="">Select City</option>')
    }
  })

  // State Dropdown Change
  $('#State').change(function () {
    var id = $(this).val()
    if (id) {
      $.ajax({
        url: '/Users/GetCitiesByState',
        data: { stateId: id },
        success: function (data) {
          var cityDropdown = $('#City')
          cityDropdown.empty()
          cityDropdown.append('<option value="">Select City</option>')

          data.forEach(function (city) {
            cityDropdown.append(
              '<option value="' +
                city.cityId +
                '">' +
                city.cityName +
                '</option>'
            )
          })
        }
      })
    } else {
      $('#City').empty().append('<option value="">Select City</option>')
    }
  })

  // Image Handling
  handleImageUpload = function (input) {
    const file = input.files[0]
    const $preview = $('#imagePreview')
    const $container = $('#imagePreviewContainer')
    const $uploadBtn = $('#uploadButton')

    if (file && file.type.startsWith('image/')) {
      const reader = new FileReader()
      reader.onload = function (e) {
        $preview.attr('src', e.target.result)
        $container.removeClass('d-none')
        $uploadBtn.addClass('d-none')

        $('#RemoveImage').val('false')
        $('#profileImagePath').val('')
        $profileImagePath.val('')
      }
      reader.readAsDataURL(file)
    }
  }

  editImage = function () {
    $('#imageInput').click()
    $input.val('')
    $input.click()
    $('#RemoveImage').val('false')
  }

  // Delete image
  removeImage = function () {
    const $input = $('#imageInput')
    const $preview = $('#imagePreview')
    const $container = $('#imagePreviewContainer')
    const $uploadBtn = $('#uploadButton')
    const $profileImagePath = $('#profileImagePath')

    const imageName = $profileImagePath.val()

    if (imageName) {
      $.ajax({
        url: '/Users/DeleteProfileImage',
        type: 'POST',
        data: { imageName: imageName },
        success: function (response) {
          if (response.success) {
            toastr.success('Image deleted successfully.')
          } else {
            toastr.warning('Image not found or could not be deleted.')
          }
        },
        error: function () {
          toastr.error('An error occurred while deleting the image.')
        }
      })
    }

    $input.val('')
    $profileImagePath.val('')
    $preview.attr('src', '')
    $container.addClass('d-none')
    $uploadBtn.removeClass('d-none')

    $('#RemoveImage').val('true')
  }
})
