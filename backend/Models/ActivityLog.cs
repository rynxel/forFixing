using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models
{
    public class ActivityLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // 🔹 Who performed the action
        public int? UserId { get; set; }
        public User? User { get; set; }

        // 🔹 Optional: Direct link to Task
        public int? TaskItemId { get; set; }
        public TaskItem? TaskItem { get; set; }

        // 🔹 What happened
        [Required]
        [MaxLength(100)]
        public string Action { get; set; } = string.Empty;
        // Example: CREATE_TASK, UPDATE_TASK, DELETE_TASK, LOGIN

        public string? Description { get; set; }

        // 🔹 For tracking changes
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }

        // 🔹 Metadata
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}