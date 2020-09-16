namespace Keeper.Common
{
    public class AccountInfo
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Password { get; set; }

        public AccountInfo(string name, string id, string password)
        {
            Name = name;
            Id = id;
            Password = password;
        }
    }
}
