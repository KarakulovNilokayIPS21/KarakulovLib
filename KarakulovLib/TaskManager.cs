using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace KarakulovLib
{
    public class TaskManager : IDisposable, ITaskManager
    {
        public TaskContext Context { get; private set; } = new TaskContext();
        private bool disposedValue;

        public TaskManager()
        {
        }

        public void AddTask(Task task)
        {
            if (!Context.Tasks.Any(t => t.Id == task.Id))
            {
                task.Id = ++TaskContext.TaskID;
                Context.Tasks.Add(task);
            }
        }

        public void UpdateTask(Task task)
        {
            var target = Context.Tasks.FirstOrDefault(t => t.Id == task.Id);
            if (target != null)
            {
                target.Title = task.Title;
                target.Description = task.Description;
                target.Priority = task.Priority;
                target.DueDate = task.DueDate;
                target.Status = task.Status;
            }
            else
            {
                throw new ArgumentException("Задача для обновления не найдена.");
            }
        }

        public void RemoveTask(Task task)
        {
            if (Context.Tasks.Contains(task))
            {
                Context.Tasks.Remove(task);
            }
        }

        public Task GetTaskById(int id)
        {
            return Context.Tasks.FirstOrDefault(t => t.Id == id);
        }

        public IEnumerable<Task> GetAllTasks()
        {
            return Context.Tasks;
        }

        
        public void SaveToFile(string filePath)
        {
            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Task>));
                    serializer.WriteObject(stream, Context.Tasks.ToList());
                }
            }
            catch (Exception ex)
            {
                throw new IOException($"Ошибка при сохранении задач: {ex.Message}", ex);
            }
        }

        public void СreateFile(string filePath)
        {
            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Task>));
                    serializer.WriteObject(stream, new List<Task>());
                }
            }
            catch (Exception ex)
            {
                throw new IOException($"Ошибка при сохранении задач: {ex.Message}", ex);
            }
        }

        public void LoadFromFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    using (FileStream stream = new FileStream(filePath, FileMode.Open))
                    {
                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Task>));
                        var tasks = serializer.ReadObject(stream) as List<Task>;
                        
                        if (tasks != null)
                        {
                            Context.Tasks.Clear();
                            foreach (var task in tasks)
                            {
                                Context.Tasks.Add(task);
                            }
                            
                            if (tasks.Any())
                            {
                                TaskContext.TaskID = tasks.Max(t => t.Id);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new IOException($"Ошибка при загрузке задач: {ex.Message}", ex);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (Context != null)
                    {
                        Context.Dispose();
                    }
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        ~TaskManager()
        {
            Dispose(disposing: false);
        }
    }
} 