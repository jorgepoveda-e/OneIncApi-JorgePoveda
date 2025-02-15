using OneIncApi.Services.DTOs;

namespace OneIncApi.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserResponse> CreateUserAsync(CreateUserRequest request);
        Task<UserResponse> GetUserAsync(int id);
        Task<IEnumerable<UserResponse>> GetAllUsersAsync();
        Task UpdateUserAsync(int id, UpdateUserRequest request);
        Task DeleteUserAsync(int id);
    }
}
