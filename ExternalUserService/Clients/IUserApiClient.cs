using ExternalUserService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternalUserService.Clients
{
    public interface IUserApiClient
    {
        Task<ApiResponse<User>> GetUsersByPageAsync(int pageNumber);
        Task<SingleUserResponse> GetUserByIdAsync(int userId);
    }
}
