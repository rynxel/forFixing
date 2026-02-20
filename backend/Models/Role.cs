namespace TaskManager.Models
{
    public class Role
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        // Example: Admin, User, Manager

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}