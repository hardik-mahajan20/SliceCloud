using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Attributes;
using SliceCloud.Service.Interfaces;
using SliceCloud.Service.Utils;

namespace SliceCloud.Web.Controllers;

/// <summary>
/// This controller is referenced for the my-profile module related end points.
/// </summary>
public class MyProfileController(ICountryService countryService, IStateService stateService, ICityService cityService, IMyProfileService profileService, IAuthService authenticateUserService) : Controller
{

    private readonly IMyProfileService _profileService = profileService;
    private readonly ICountryService _countryService = countryService;
    private readonly IStateService _stateService = stateService;
    private readonly ICityService _cityService = cityService;
    private readonly IAuthService _authenticateUserService = authenticateUserService;

    #region MyProfile GET

    [CustomAuthorize]
    [HttpGet]
    public async Task<IActionResult> MyProfile()
    {
        UserCredentialViewModel userCredentialViewModel = _authenticateUserService.DecodeJwtToken(Request.Cookies["AuthToken"] ?? "");
        if (string.IsNullOrEmpty(userCredentialViewModel.Email))
        {
            return RedirectToAction("Login", "Auth");
        }

        MyProfileViewModel? myProfileViewModel = await _profileService.GetProfileByIdAsync(userCredentialViewModel.Id);

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

    [CustomAuthorize]
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


            UserCredentialViewModel userCredentialViewModel = _authenticateUserService.DecodeJwtToken(Request.Cookies["AuthToken"] ?? "");
            if (string.IsNullOrEmpty(userCredentialViewModel.Email))
            {
                return RedirectToAction("Dashboard", "Dashboard");
            }

            MyProfileViewModel? user = await _profileService.GetProfileByIdAsync(userCredentialViewModel.Id);

            bool isDuplicateUsername = await _profileService.IsUsernameTakenAsync(myProfileViewModel.UserName, myProfileViewModel.Id);
            if (isDuplicateUsername)
            {
                ModelState.AddModelError("Username", "Username already exists. Please choose a different one.");

                myProfileViewModel.Countries = await _countryService.GetAllCountriesAsync();
                myProfileViewModel.States = await _stateService.GetStatesByCountryIdAsync(myProfileViewModel.CountryId);
                myProfileViewModel.Cities = await _cityService.GetCitiesByStateIdAsync(myProfileViewModel.StateId);

                return View(myProfileViewModel);
            }

            UserCredentialViewModel? loggedInUser = _authenticateUserService.DecodeJwtToken(Request.Cookies["AuthToken"] ?? ""); ;

            MyProfileViewModel? updatedProfile = await _profileService.UpdateProfileAsync(loggedInUser.Id, myProfileViewModel);

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
