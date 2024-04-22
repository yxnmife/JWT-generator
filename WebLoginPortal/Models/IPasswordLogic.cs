using WebLoginPortal.DTO;

namespace WebLoginPortal.Models
{
    public interface IPasswordLogic
    {
        void CreateHashPassword(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt);
        byte[] HashPassword(string password, out byte[] passwordHash, out byte[] passwordSalt);
        Task<UserInfo?> CreateUser(CreateUserDTO user);
        Task<string> CreateToken(UserInfo user);
        Task<UserInfo?> CheckLoginDetails(LoginDTO logindetails);
        Task<UserInfo?> CheckIDDetails(int id);
        Task<List<UserInfoDTO?>> GetAllUsers();
    }
}
