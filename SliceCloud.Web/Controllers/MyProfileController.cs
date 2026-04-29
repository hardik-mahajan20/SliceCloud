using Microsoft.AspNetCore.Mvc;
using SliceCloud.Repository.Constants;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Attributes;
using SliceCloud.Service.Interfaces;
using SliceCloud.Service.Utils;

namespace SliceCloud.Web.Controllers;

/// <summary>
/// This controller is referenced for the my-profile module related end points.
/// </summary>
public class MyProfileController(ICountryService countryService, IStateService stateService, ICityService cityService, IMyProfileService profileService, IAuthService authenticateUserService, ICurrentUserService currentUserService) : Controller
{

    private readonly IMyProfileService _profileService = profileService;
    private readonly ICountryService _countryService = countryService;
    private readonly IStateService _stateService = stateService;
    private readonly ICityService _cityService = cityService;
    private readonly IAuthService _authenticateUserService = authenticateUserService;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    #region MyProfile GET

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpGet]
    public async Task<IActionResult> MyProfile()
    {
        int userId = _currentUserService.UserId;

        if (userId <= 0)
        {
            return RedirectToAction("Login", "Auth");
        }

        MyProfileViewModel? myProfileViewModel = await _profileService.GetProfileByIdAsync(userId);

        if (myProfileViewModel == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        myProfileViewModel.Countries = await _countryService.GetAllCountriesAsync();
        myProfileViewModel.States = await _stateService.GetStatesByCountryIdAsync(myProfileViewModel.CountryId);
        myProfileViewModel.Cities = await _cityService.GetCitiesByStateIdAsync(myProfileViewModel.StateId);
        return View(myProfileViewModel);
    }

    #endregion

    #region MyProfile - POST

    [CustomAuthorize(PermissionConstants.CAN_VIEW, RolesConstants.ADMIN, RolesConstants.MANAGER, RolesConstants.CHEF)]
    [HttpPost]
    public async Task<IActionResult> MyProfile(MyProfileViewModel myProfileViewModel)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"Invalid Field: {state.Key}, Error: {error.ErrorMessage}");
                    }
                }

                return View(myProfileViewModel);
            }

            int userId = _currentUserService.UserId;

            if (userId <= 0)
            {
                return RedirectToAction("Login", "Auth");
            }

            MyProfileViewModel? user = await _profileService.GetProfileByIdAsync(userId);

            bool isDuplicateUsername = await _profileService.IsUsernameTakenAsync(myProfileViewModel.UserName, userId);
            
            if (isDuplicateUsername)
            {
                ModelState.AddModelError("Username", "Username already exists. Please choose a different one.");

                myProfileViewModel.Countries = await _countryService.GetAllCountriesAsync();
                myProfileViewModel.States = await _stateService.GetStatesByCountryIdAsync(myProfileViewModel.CountryId);
                myProfileViewModel.Cities = await _cityService.GetCitiesByStateIdAsync(myProfileViewModel.StateId);

                return View(myProfileViewModel);
            }

            MyProfileViewModel? updatedProfile = await _profileService.UpdateProfileAsync(userId, myProfileViewModel);

            if (updatedProfile is not null)
            {
                TempData.SetToast("success", "Profile Updated Successfully!");
                return RedirectToAction(nameof(MyProfile));
            }

            TempData.SetToast("error", "Profile update failed!");
            return RedirectToAction(nameof(MyProfile));
        }
        catch (Exception)
        {
            TempData.SetToast("error", "An error occurred while processing your request. Please try again.");
            return RedirectToAction(nameof(MyProfile));
        }
    }

    #endregion

}
