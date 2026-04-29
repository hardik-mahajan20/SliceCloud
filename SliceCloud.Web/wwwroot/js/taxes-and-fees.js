$(document).ready(function () {
  let currentPage = 1
  let pageSize = $('#pageSizeDropdown').val()
  let searchQuery = $('#searchInput').val()

  loadTaxes(currentPage, pageSize, searchQuery)

  function loadTaxes (page, size, search) {
    $.ajax({
      url: '/TaxesAndFees/GetTaxesAndFeesTable',
      type: 'GET',
      data: {
        page: page,
        pageSize: size,
        search: search
      },
      success: function (data) {
        $('#TaxFeesContainer').html(data)
        updatePaginationControls()
        rebindPaginationEvents()
      },
      error: function () {
        toastr.error('Error loading tax data.')
      }
    })
  }

  function updatePaginationControls () {
    let totalPages = parseInt($('#totalPages').val())
    $('#prevPageBtn').prop('disabled', currentPage <= 1)
    $('#nextPageBtn').prop('disabled', currentPage >= totalPages)
  }

  function rebindPaginationEvents () {
    let totalPages = parseInt($('#totalPages').val())

    $('#prevPageBtn')
      .off('click')
      .on('click', function () {
        if (currentPage > 1) {
          currentPage--
          loadTaxes(currentPage, pageSize)
        }
      })

    $('#nextPageBtn')
      .off('click')
      .on('click', function () {
        if (currentPage < totalPages) {
          currentPage++
          loadTaxes(currentPage, pageSize)
        }
      })

    $('#pageSizeDropdown')
      .off('change')
      .on('change', function () {
        pageSize = $(this).val()
        currentPage = 1
        loadTaxes(currentPage, pageSize)
      })

    let debounceTimer
    $(document).on('change keyup', '#searchInput', function () {
      clearTimeout(debounceTimer)
      let search = $(this).val().trim()

      debounceTimer = setTimeout(function () {
        searchQuery = search
        loadTaxes(currentPage, pageSize, searchQuery)
      }, 300)
    })
  }

  $('#openAddTaxModal').click(function () {
    $('#addTaxModal').modal('show')
  })

  $('#addTaxForm').submit(function (e) {
    e.preventDefault()

    $('.field-validation-error').text('')

    var form = $(this)
    var formData = form.serialize()

    $.ajax({
      type: 'POST',
      url: form.attr('action'),
      data: formData,
      success: function (response) {
        if (response.success) {
          toastr.success('Tax added successfully!')

          $('#addTaxForm')[0].reset()

          $('.text-danger').text('')

          $('#addTaxModal').modal('hide')
          loadTaxes(currentPage, pageSize)
        } else if (response.errors) {
          $.each(response.errors, function (key, messages) {
            $(`[data-val-msg-for="${key}"]`).text(messages[0])
          })
        } else {
          toastr.error(response.message || 'Something went wrong.')
        }
      },
      error: function () {
        toastr.error('Something went wrong!')
      }
    })
  })

  $('#addTaxModal').on('hidden.bs.modal', function () {
    $('#addTaxForm')[0].reset()

    $('.text-danger').text('')
  })
  $('#editTaxModal').on('hidden.bs.modal', function () {
    $('#editTaxForm')[0].reset()

    $('.text-danger').text('')
  })

  $(document).on('click', '.edit-tax', function (e) {
    e.preventDefault()
    var taxId = $(this).data('id')

    $.ajax({
      url: '/TaxesAndFees/GetTaxById/' + taxId,
      type: 'GET',
      success: function (response) {
        if (response.success) {
          $('#editTaxId').val(response.taxId)
          $('#editTaxName').val(response.taxName)
          $('#editTaxType').val(response.taxType)
          $('#editTaxAmount').val(response.taxValue)
          $('#editIsEnabled').prop('checked', response.isEnabled)
          $('#editIsDefault').prop('checked', response.isDefault)

          $('#editTaxModal').modal('show')
        } else {
          toastr.error(response.message || 'Failed to load tax details.')
        }
      },
      error: function () {
        toastr.error('Error loading tax details.')
      }
    })
  })

  $('#saveChangesBtn').click(function (e) {
    e.preventDefault()

    var formData = $('#editTaxForm').serialize()

    $.ajax({
      type: 'POST',
      url: '/TaxesAndFees/UpdateTax',
      data: formData,
      success: function (response) {
        if (response.success) {
          toastr.success('Tax updated successfully!')
          $('#editTaxModal').modal('hide')
          loadTaxes(currentPage, pageSize)
        } else {
          toastr.error('Error updating tax.')
        }
      },
      error: function (response) {
        if (response.status === 400) {
          $('.text-danger').text('')

          var errors = response.responseJSON.errors
          $.each(errors, function (key, value) {
            $('#' + key + 'Error').text(value[0])
          })
        } else {
          toastr.error('Error updating tax.')
        }
      }
    })
  })

  $(document).on('click', '.delete-tax', function () {
    var taxId = $(this).data('id')
    $('#deleteTaxId').val(taxId)
    $('#deleteTaxModal').modal('show')
  })

  $('#deleteTaxForm').submit(function (e) {
    e.preventDefault()

    var taxId = $('#deleteTaxId').val()

    $.ajax({
      type: 'POST',
      url: '/TaxesAndFees/DeleteTax',
      data: { taxId: taxId },
      dataType: 'json',
      success: function (response) {
        if (response.success) {
          toastr.success('Tax deleted successfully!')
          $('#deleteTaxModal').modal('hide')
          loadTaxes(currentPage, pageSize)
        } else {
          toastr.error('Error deleting tax.')
        }
      },
      error: function () {
        toastr.error('Error deleting tax.')
      }
    })
  })

  $(document).on('change', '.form-check-input-quick', function () {
    var checkbox = $(this)
    var taxId = checkbox.data('id')
    var isChecked = checkbox.is(':checked')
    var toggleType = checkbox.data('type')
    var previousValue = !isChecked

    $.ajax({
      url: '/TaxesAndFees/ToggleTaxField',
      type: 'POST',
      data: { taxId: taxId, isChecked: isChecked, field: toggleType },
      success: function (response) {
        if (response.success) {
          let customMessage = ''

          switch (toggleType) {
            case 'IsEnabled':
              customMessage = isChecked
                ? 'Tax has been enabled successfully.'
                : 'Tax has been disabled successfully.'
              break
            case 'IsDefault':
              customMessage = isChecked
                ? 'This tax is now the default.'
                : 'This tax is no longer the default.'
              break
            case 'IsInclusive':
              customMessage = isChecked
                ? 'This tax is now Inclusive'
                : 'Item is now unavailable.'
              break
            default:
              customMessage = 'Field updated successfully.'
          }

          toastr.success(customMessage)
        } else {
          toastr.error(
            response.message || 'Something went wrong while updating.'
          )
          setTimeout(() => {
            checkbox.prop('checked', previousValue)
          }, 500)
        }
      },
      error: function (xhr) {
        if (xhr.status === 403) {
          toastr.error(
            "Access Denied: You don't have permission to perform this action."
          )
        } else {
          toastr.error('An error occurred while updating.')
        }
        setTimeout(() => {
          checkbox.prop('checked', previousValue)
        }, 500)
      }
    })
  })
})
