using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SliceCloud.Repository.Constants;

namespace SliceCloud.Service.Utils;

public static class ToastExtensions
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Used to pop-up toast message.
    /// </summary>
    /// <param name="tempData">Temp data for passing the toast.</param>
    /// <param name="type">The type of the toast.</param>
    /// <param name="message">The message to show in the toast.</param>
    public static void SetToast(
        this ITempDataDictionary tempData,
        string type,
        string message)
    {
        tempData[GeneralConstants.TOAST] = JsonSerializer.Serialize(
            new
            {
                type,
                message
            },
            _jsonOptions
        );
    }
}