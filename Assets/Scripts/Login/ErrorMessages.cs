public static class ErrorMessages
{
    public const string GenericError = "Critical error. I have no idea what went wrong";
    public const string ConnectionToDBFailedError = "Connection to database could not be established. Try again later.";
    public const string UserNotFoundError = "User could not be found";
    public const string WrongPasswordError = "Wrong password!";
    public const string UsernamePromptEmptyError = "Enter a username!";
    public const string PasswordPromptEmptyError = "Password needs to be at least 5 characters long!";
    public const string UserAlreadyExistsError = "User with this name already exists!";
    public const string RegisterUserFailedError = "Could not register user.";
    public const string AddProgressionDataFailedError = "Could not add states of unit and assignment progressions";
    public const string UserIDNotFoundError = "Could not fetch user ID of user %s";
}