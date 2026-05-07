namespace Models;

public static class AppResult
{
    //auth
    public static object Success = new { Result = nameof(Success) };
    public static object UserExist = new { Result = nameof(UserExist) };
    public static object UserExistNotConfirmed = new { Result = nameof(UserExistNotConfirmed) };
    public static object UserNotExist = new { Result = nameof(UserNotExist) };
    public static object PassResetComplete = new { Result = nameof(PassResetComplete) };
    public static object NeedEmailConfirm = new { Result = nameof(NeedEmailConfirm) };
    public static object IncorrectCredential = new { Result = nameof(IncorrectCredential) };
    public static object InvalidToken = new { Result = nameof(InvalidToken) };
    public static object RegistrationClosed = new { Result = nameof(RegistrationClosed) };

    //common
    public const string SuccessStr = nameof(Success);
}
