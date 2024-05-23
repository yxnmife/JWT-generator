namespace Authenticator.Interfaces
{
    public interface IPasswordLogic
    {
        void CreateHashPassword(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt);
    }
}
