using OneIncApi.Domain.Models;

namespace OneIncApi.Services.DTOs
{
    public class UserResponse
    {
        public int Id { get; init; }
        public string FirstName { get; init; } = string.Empty;
        public string? LastName { get; init; }
        public string Email { get; init; } = string.Empty;
        public DateTime DateOfBirth { get; init; }
        public int Age => CalculateAge(DateOfBirth, DateTime.UtcNow);
        public string PhoneNumber { get; init; } = string.Empty;

        public static UserResponse FromUser(User user) => new()
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            DateOfBirth = user.DateOfBirth,
            PhoneNumber = user.PhoneNumber
        };

        private static int CalculateAge(DateTime dob, DateTime currentDate)
        {
            var age = currentDate.Year - dob.Year;
            if (currentDate < dob.AddYears(age)) age--;
            return age;
        }
    }
}
