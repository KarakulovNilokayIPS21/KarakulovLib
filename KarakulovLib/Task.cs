using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Cryptography;

namespace KarakulovLib
{
    public class Task
    {
        public int Id { get; set; }
        
        public string Title { get; set; }
        
        public string Description { get; set; }
        
        public string Priority { get; set; }
        
        public DateTime? DueDate { get; set; }
        
        public string Status { get; set; }
        
        public DateTime CreatedAt { get; set; }       

        public Task()
        {
            CreatedAt = DateTime.Now;
        }
        public Task Clone()
        {
            return new Task
            {
                Id = this.Id,
                Title = this.Title,
                Description = this.Description,
                Priority = this.Priority,
                DueDate = this.DueDate,
                Status = this.Status,
                CreatedAt = this.CreatedAt
            };
        }

        public void CopyFrom(Task source)
        {
            this.Title = source.Title;
            this.Description = source.Description;
            this.Priority = source.Priority;
            this.DueDate = source.DueDate;
            this.Status = source.Status;
        }
        public Task(string title, string description, string priority, string status, DateTime? dueDate)
        {
            Title = title;
            Description = description;
            Priority = priority;
            DueDate = dueDate;
            Status = status;
            CreatedAt = DateTime.Now;
        }
    }
}

 