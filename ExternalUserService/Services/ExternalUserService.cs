using ExternalUserService.Clients;
using ExternalUserService.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExternalUserService.Services
{
    public class ExternalUserServiceImpl : IExternalUserService
    {
        private readonly IUserApiClient _apiClient;

        public ExternalUserServiceImpl(IUserApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            var response = await _apiClient.GetUserByIdAsync(userId);

            if (response?.Data == null)
                throw new Exception($"User with id {userId} not found.");

            return MapToUser(response.Data);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var users = new List<User>();
            int page = 1;
            ApiResponse<User> response;

            do
            {
                response = await _apiClient.GetUsersByPageAsync(page);
                if (response?.Data != null)
                    users.AddRange(response.Data);
                page++;
            }
            while (response != null && page <= response.TotalPages);

            return users;
        }

        private User MapToUser(User dto)
        {
            return new User
            {
                Id = dto.Id,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName
            };
        }
    }
}
