using System;
using System.Collections.ObjectModel;

namespace KarakulovLib
{
    [Serializable]
    public class TaskContext : IDisposable
    {
        public ObservableCollection<Task> Tasks { get; set; } = new ObservableCollection<Task>();
        public static int TaskID { get; set; } = 0;

        public TaskContext()
        {
        }

        public void Dispose()
        {
            Tasks.Clear();
            GC.SuppressFinalize(this);
        }
    }
} 