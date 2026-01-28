$(document).ready(function () {
  let currentPage = 1
  let pageSize = 5
  let sortBy = 'FirstName'
  let sortOrder = 'asc'
  let searchQuery = ''

  function loadUsers (page, size, sortBy, sortOrder, search) {
    $.ajax({
      url: '/Users/Users',
      type: 'GET',
      data: {
        pageNumber: page,
        pageSize: size,
        sortColumn: sortBy,
        sortOrder: sortOrder,
        search: search
      },
      success: function (response) {
        $('#userTableContainer').html(response)
        syncFromServer()
        updatePaginationControls()
        updateSortingIcons()
      }
    })
  }

  function updatePaginationControls () {
    let totalPages = parseInt($('#totalPages').val()) || 1
    let totalRecords = parseInt($('#totalRecords').val()) || 0
    $('#prevPageBtn')
      .prop('disabled', currentPage <= 1)
      .toggleClass('disabled', currentPage <= 1)
    $('#nextPageBtn')
      .prop('disabled', currentPage >= totalPages)
      .toggleClass('disabled', currentPage <= 1)
    let fromRecord = (currentPage - 1) * pageSize + 1
    let toRecord = Math.min(currentPage * pageSize, totalRecords)
    $('#showingInfo').text(
      `Showing ${fromRecord} - ${toRecord} of ${totalRecords}`
    )
  }

  function updateSortingIcons () {
    $('.sortable-column i').css('color', '#ccc')
    $('.sortable-column').each(function () {
      let column = $(this).data('column')
      if (column === sortBy) {
        $(this)
          .find(sortOrder === 'asc' ? '.bi-arrow-up' : '.bi-arrow-down')
          .css('color', 'black')
      }
    })
  }

  function syncFromServer () {
    currentPage = parseInt($('#currentPage').val()) || currentPage
    pageSize = parseInt($('#pageSize').val()) || pageSize
    searchQuery = $('#search').val() || searchQuery
  }

  // Next Page Button
  $(document).on('click', '#nextPageBtn', function () {
    let totalPages = parseInt($('#totalPages').val()) || 1
    if (currentPage < totalPages) {
      loadUsers(currentPage + 1, pageSize, sortBy, sortOrder, searchQuery)
    }
  })

  // Previous Page Button
  $(document).on('click', '#prevPageBtn', function () {
    if (currentPage > 1) {
      loadUsers(currentPage - 1, pageSize, sortBy, sortOrder, searchQuery)
    }
  })

  // Page size dropdown
  $(document).on('change', '#pageSizeDropdown', function () {
    let newSize = parseInt($(this).val())
    loadUsers(1, newSize, sortBy, sortOrder, searchQuery)
  })

  // Sortable Column
  $(document).on('click', '.sortable-column', function () {
    let column = $(this).data('column')
    if (sortBy === column) {
      sortOrder = sortOrder === 'asc' ? 'desc' : 'asc'
    } else {
      sortBy = column
      sortOrder = 'asc'
    }
    loadUsers(1, pageSize, sortBy, sortOrder, searchQuery)
  })

  // Search
  $(document).on('keyup', '#search', function () {
    let search = $(this).val().trim()
    searchQuery = search
    loadUsers(1, pageSize, sortBy, sortOrder, search)
  })

  $('.dropdown-toggle').on('click', function (e) {
    e.preventDefault()
    e.stopPropagation()
    let dropdownMenu = $(this).siblings('.dropdown-menu')
    $('.dropdown-menu').not(dropdownMenu).removeClass('show')
    dropdownMenu.toggleClass('show')
    let isExpanded = dropdownMenu.hasClass('show')
    $(this).attr('aria-expanded', isExpanded)
  })

  $(document).on('click', function (e) {
    if (!$(e.target).closest('.dropdown').length) {
      $('.dropdown-menu').removeClass('show')
      $('.dropdown-toggle').attr('aria-expanded', 'false')
    }
  })
})
