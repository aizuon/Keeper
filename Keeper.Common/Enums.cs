namespace Keeper.Common
{
    public enum LoginResult : byte
    {
        Success,
        WrongPassword,
        UsernameDoesntExist
    }

    public enum RegisterResult : byte
    {
        Success,
        UsernameTaken
    }
}
