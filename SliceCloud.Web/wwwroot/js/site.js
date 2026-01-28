function toggle () {
  let input_toggle = document.getElementById('toggle_button_eye')
  let password_input = document.getElementById('custom-password-input')

  if (password_input.type === 'password') {
    password_input.type = 'text'
    input_toggle.src = '/images/icons/visibility.svg'
  } else {
    password_input.type = 'password'
    input_toggle.src = '/images/icons/visibility_off.svg'
  }
}
function toggle_new_password () {
  let input_toggle = document.getElementById('toggle_button_eye_new_password')
  let password_input = document.getElementById(
    'custom-password-input_new_password'
  )

  if (password_input.type === 'password') {
    password_input.type = 'text'
    input_toggle.src = '/images/icons/visibility.svg'
  } else {
    password_input.type = 'password'
    input_toggle.src = '/images/icons/visibility_off.svg'
  }
}
function toggle_confirm_password () {
  let input_toggle = document.getElementById(
    'toggle_button_eye_confirm_password'
  )
  let password_input = document.getElementById(
    'custom-password-input_confirm_password'
  )

  if (password_input.type === 'password') {
    password_input.type = 'text'
    input_toggle.src = '/images/icons/visibility.svg'
  } else {
    password_input.type = 'password'
    input_toggle.src = '/images/icons/visibility_off.svg'
  }
}
$(document).on('click', '[data-bs-target="#confirmDeleteModal"]', function () {
  const id = $(this).data('id');
  const url = $(this).data('url');
  $('#deleteEntityId').val(id);
  $('#confirmDeleteForm').attr('action', url);
});
