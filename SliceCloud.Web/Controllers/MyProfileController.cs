using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Attributes;
using SliceCloud.Service.Interfaces;

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
        UserCredentialViewModel userCredentialViewModel = GetUserLoginDetails();
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

    #region Helper Methods

    private UserCredentialViewModel GetUserLoginDetails()
    {
        var token = Request.Cookies["AuthToken"];
        UserCredentialViewModel userCredentialViewModel = _authenticateUserService.DecodeJwtToken(token ?? "");
        return userCredentialViewModel;
    }

    private async Task PopulateDropdowns(int countryId, int stateId)
    {
        ViewBag.Countries = await _countryService.GetAllCountriesAsync();
        ViewBag.States = await _stateService.GetStatesByCountryIdAsync(countryId);
        ViewBag.Cities = await _cityService.GetCitiesByStateIdAsync(stateId);
    }

    #endregion

}
