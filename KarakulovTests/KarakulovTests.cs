using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KarakulovLib;
using System.IO;
using System.Linq;

namespace KarakulovTests
{
    [TestClass]
    public class KarakulovTests
    {
        private TaskManager taskManager;
        private string testDataPath = "test_data.json";

        [TestInitialize]
        public void Setup()
        {
            taskManager = new TaskManager();

            if (File.Exists(testDataPath))
            {
                File.Delete(testDataPath);
            }
        }

        [TestCleanup]
        public void Teardown()
        {
            taskManager.Dispose();

            if (File.Exists(testDataPath))
            {
                File.Delete(testDataPath);
            }
        }

        [TestMethod]
        public void TestAddTask_ShouldAddTaskToList()
        {
            var testTask = new Task
            {
                Title = "test_title",
                Description = "test_description"
            };

            taskManager.AddTask(testTask);

            Assert.AreEqual(1, taskManager.GetAllTasks().Count());
            Assert.AreEqual("test_title", taskManager.GetAllTasks().First().Title);
        }

        [TestMethod]
        public void TestFindTask_ShouldFindTask()
        {
            var testTask = new Task
            {
                Title = "test_title",
                Description = "test_description"
            };

            taskManager.AddTask(testTask);

            Assert.AreEqual("test_title", taskManager.GetTaskById(testTask.Id).Title);
        }

        [TestMethod]
        public void TestGetAllTasks_ShouldGetAllTasks()
        {
            var testTask1 = new Task
            {
                Title = "test_title1",
                Description = "test_description1"
            };

            var testTask2 = new Task
            {
                Title = "test_title2",
                Description = "test_description2"
            };

            taskManager.AddTask(testTask1);
            taskManager.AddTask(testTask2);

            Assert.AreEqual(2, taskManager.GetAllTasks().Count());
            Assert.AreEqual("test_title1", taskManager.GetAllTasks().First().Title);
        }

        [TestMethod]
        public void TestUpdateTask_ShouldUpdateExistingTask()
        {
            var testTask = new Task
            {
                Title = "original_title",
                Description = "original_description"
            };
            taskManager.AddTask(testTask);

            testTask.Title = "updated_title";
            testTask.Description = "updated_description";

            taskManager.UpdateTask(testTask);

            var updatedTask = taskManager.GetTaskById(testTask.Id);
            Assert.IsNotNull(updatedTask);
            Assert.AreEqual("updated_title", updatedTask.Title);
            Assert.AreEqual("updated_description", updatedTask.Description);
        }

        [TestMethod]
        public void TestRemoveTask_ShouldRemoveTaskFromList()
        {
            var testTask = new Task
            {
                Title = "task_to_delete"
            };
            taskManager.AddTask(testTask);
            Assert.AreEqual(1, taskManager.GetAllTasks().Count());

            taskManager.RemoveTask(testTask);

            Assert.AreEqual(0, taskManager.GetAllTasks().Count());
        }

        [TestMethod]
        public void TestSaveAndLoadFromFile_ShouldPersistTasks()
        {
            var testTask1 = new Task { Title = "test_task1", Description = "test_desc1" };
            var testTask2 = new Task { Title = "test_task2", Description = "test_desc2" };

            taskManager.AddTask(testTask1);
            taskManager.AddTask(testTask2);

            taskManager.SaveToFile(testDataPath);

            var newTaskManager = new TaskManager();
            newTaskManager.LoadFromFile(testDataPath);

            var loadedTasks = newTaskManager.GetAllTasks().ToList();

            Assert.AreEqual(2, loadedTasks.Count);
            Assert.IsTrue(loadedTasks.Any(t => t.Title == "test_task1" && t.Description == "test_desc1"));
            Assert.IsTrue(loadedTasks.Any(t => t.Title == "test_task2" && t.Description == "test_desc2"));

            newTaskManager.Dispose();
        }
    }
}