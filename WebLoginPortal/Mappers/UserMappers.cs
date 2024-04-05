using WebLoginPortal.DTO;
using WebLoginPortal.Models;

namespace WebLoginPortal.Mappers
{
    public static class UserMappers
    {
        public static UserInfoDTO ToUserInfoDTO(this UserInfo userInfo)
        {
            return new UserInfoDTO()
            {
                Id = userInfo.Id,
                FirstName = userInfo.FirstName,
                LastName = userInfo.LastName,
                Email = userInfo.Email,
            };
        }
    }
}
