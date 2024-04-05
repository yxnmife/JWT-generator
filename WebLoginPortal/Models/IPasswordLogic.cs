namespace WebLoginPortal.Models
{
    public interface IPasswordLogic
    {
       void CreateHashPassword(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool VerifyPassword(string password,byte[] passwordHash, byte[] passwordSalt);
        byte[] HashPassword(string password, out byte[] passwordHash, out byte[] passwordSalt);

    }
}
