using System;
using System.Collections.Generic;

namespace KarakulovLib
{
    public interface ITaskManager
    {
        void AddTask(Task task);
        void UpdateTask(Task task);
        void RemoveTask(Task task);
        Task GetTaskById(int id);
        IEnumerable<Task> GetAllTasks();
        
        void SaveToFile(string filePath);
        void LoadFromFile(string filePath);
        void ÑreateFile(string filePath);

    }
} 