

namespace TaskManager.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public ICollection<TaskItem> Tasks { get; set; } = [];
        public bool IsActive { get; set; } = true;

        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }
}