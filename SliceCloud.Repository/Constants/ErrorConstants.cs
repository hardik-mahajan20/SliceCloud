namespace SliceCloud.Repository.Constants;

/// <summary>
/// Contains string constants for error messages.
/// Used to avoid magic strings and ensure consistency across the project.
/// </summary>
public static class ErrorConstants
{
    public const string USER_ID_CLAIM_MISSING = "User ID claim is missing.";
    public const string JWT_KEY_MISSING = "JWT key is missing in configuration.";
    public const string JWT_ISSUER_MISSING = "JWT issuer is missing in configuration.";
    public const string JWT_AUDIENCE_MISSING = "JWT audience is missing in configuration.";
    public const string JWT_USER_NOT_FOUND = "User not found while generating JWT token.";
    public const string JWT_USER_ROLE_NOT_FOUND = "User role not found while generating JWT token.";
    public const string JWT_USER_ID_NOT_FOUND = "User id not found while generating JWT token.";
    public const string INVALID_PERMISSION_UPDATE_REQUEST = "Invalid permission update request.";
    public const string INVALID_FIELD_TYPE = "Invalid field type.";
    public const string TAX_NOT_FOUND = "Tax not found.";
    public const string ERROR_OCCURRED_WHILE_CREATING_USER = "An error occurred while creating the user login. Details: ";
    public const string EMAIL_ALREADY_EXISTS = "Email already exists.";
    public const string USERNAME_ALREADY_EXISTS = "UserName already exists.";
    public const string PHONE_NUMBER_ALREADY_EXISTS = "Phone number already exists.";
}
