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

    public enum AccountEditResult : byte
    {
        Success,
        AccountNotFound
    }
}
