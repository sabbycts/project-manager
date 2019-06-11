using Microsoft.VisualStudio.TestTools.UnitTesting;
using PMServer.Controllers;
using PMServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMServer.Test
{
    class MockTaskBC : BC.TaskBC
    {
        MockProjectManagerEntities _mockBd = null;
        public MockTaskBC(MockProjectManagerEntities mockBd)
        {
            _mockBd = mockBd;
        }

        public override List<Models.Task> RetrieveTaskByProjectId(int projectId)
        {
            return _mockBd.Tasks.Where(z => z.Project_ID == projectId).Select(x => new Models.Task()
            {
                TaskId = x.Task_ID,
                Task_Name = x.Task1,
                ParentTaskName = _mockBd.ParentTasks.Where(y => y.Parent_ID == x.Parent_ID).FirstOrDefault().Parent_Task,
                Start_Date = x.Start_Date,
                End_Date = x.End_Date,
                Priority = x.Priority,
                Status = x.Status,
                User = _mockBd.Users.Where(y => y.Task_ID == x.Task_ID).Select(z => new User()
                {
                    UserId = z.User_ID,
                    FirstName = z.First_Name
                }).FirstOrDefault(),
            }).ToList();
        }

        public override List<ParentTask> RetrieveParentTasks()
        {
            return _mockBd.ParentTasks.Select(x => new ParentTask()
            {
                ParentTaskId = x.Parent_ID,
                ParentTaskName = x.Parent_Task
            }).ToList();
        }

        public override int InsertTaskDetails(Models.Task task)
        {
            if (task.Priority == 0)
            {
                _mockBd.ParentTasks.Add(new DAC.ParentTask()
                {
                    Parent_Task = task.Task_Name

                });
            }
            else
            {
                DAC.Task taskDetail = new DAC.Task()
                {
                    Task1 = task.Task_Name,
                    Project_ID = task.Project_ID,
                    Start_Date = task.Start_Date,
                    End_Date = task.End_Date,
                    Parent_ID = task.Parent_ID,
                    Priority = task.Priority,
                    Status = task.Status
                };
                _mockBd.Tasks.Add(taskDetail);

                var editDetails = (from editUser in _mockBd.Users
                                   where editUser.User_ID.ToString().Contains(task.User.UserId.ToString())
                                   select editUser).First();
                // Modify existing records
                if (editDetails != null)
                {
                    editDetails.Task_ID = taskDetail.Task_ID;
                }
            }
            return 1;
        }

        public override int UpdateTaskDetails(Models.Task task)
        {
            var editDetails = (from editTask in _mockBd.Tasks
                               where editTask.Task_ID.ToString().Contains(task.TaskId.ToString())
                               select editTask).First();
            // Modify existing records
            if (editDetails != null)
            {
                editDetails.Task1 = task.Task_Name;
                editDetails.Start_Date = task.Start_Date;
                editDetails.End_Date = task.End_Date;
                editDetails.Status = task.Status;
                editDetails.Priority = task.Priority;

            }
            var editDetailsUser = (from editUser in _mockBd.Users
                                   where editUser.User_ID.ToString().Contains(task.User.UserId.ToString())
                                   select editUser).First();
            // Modify existing records
            if (editDetailsUser != null)
            {
                editDetails.Task_ID = task.TaskId;
            }
            return 1;
        }

        public override int DeleteTaskDetails(Models.Task task)
        {
            var deleteTask = (from editTask in _mockBd.Tasks
                              where editTask.Task_ID.ToString().Contains(task.TaskId.ToString())
                              select editTask).First();
            // Delete existing record
            if (deleteTask != null)
            {
                deleteTask.Status = 1;
            }
            return 1;
        }
    }

    [TestClass]
    public class TaskControllerTest
    {
        [TestMethod]
        public void TestRetrieveTasks_Success()
        {
            var context = new MockProjectManagerEntities();
            var tasks = new TestDbSet<DAC.Task>();
            var users = new TestDbSet<DAC.User>();
            var parentTasks = new TestDbSet<DAC.ParentTask>();

            parentTasks.Add(new DAC.ParentTask()
            {
                Parent_ID = 123456,
                Parent_Task = "PNB"

            });
            context.ParentTasks = parentTasks;
            users.Add(new DAC.User()
            {
                Employee_ID = "414942",
                First_Name = "ankita",
                Last_Name = "ghosh",
                User_ID = 123,
                Task_ID = 1
            });
            context.Users = users;
            int projectid = 1234;
            tasks.Add(new DAC.Task()
            {
                //Task_ID = 1,
                Task1 = "ASDQW",
                Parent_ID = 123456,
                Project_ID = 1234,
                Start_Date = DateTime.Now,
                End_Date = DateTime.Now.AddDays(2),
                Priority = 10,
                Status = 0

            });
            context.Tasks = tasks;
            var controller = new TaskController(new MockTaskBC(context));
            var result = controller.RetrieveTaskByProjectId(projectid) as JSendResponse;

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Data, typeof(List<PMServer.Models.Task>));
        }

        [TestMethod]
        public void TestRetrieveParentTasks_Success()
        {
            var context = new MockProjectManagerEntities();
            var parentTasks = new TestDbSet<DAC.ParentTask>();
            parentTasks.Add(new DAC.ParentTask()
            {
                Parent_ID = 12345,
                Parent_Task = "ANB"

            });
            parentTasks.Add(new DAC.ParentTask()
            {
                Parent_ID = 123456,
                Parent_Task = "PNB"

            });
            context.ParentTasks = parentTasks;

            var controller = new TaskController(new MockTaskBC(context));
            var result = controller.RetrieveParentTasks() as JSendResponse;

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Data, typeof(List<PMServer.Models.ParentTask>));
            Assert.AreEqual((result.Data as List<ParentTask>).Count, 2);
        }

        [TestMethod]
        public void TestInsertTasks_Success()
        {
            var context = new MockProjectManagerEntities();
            var users = new TestDbSet<DAC.User>();
            users.Add(new DAC.User()
            {
                Employee_ID = "414942",
                First_Name = "ankita",
                Last_Name = "ghosh",
                User_ID = 123,
                Task_ID = 123
            });
            context.Users = users;
            context.Tasks = new TestDbSet<DAC.Task>();

            var task = new PMServer.Models.Task()
            {

                Task_Name = "ASDQW",
                Parent_ID = 123674,
                Project_ID = 34856,
                Start_Date = DateTime.Now,
                End_Date = DateTime.Now.AddDays(2),
                Priority = 10,
                Status = 0,
                User = new User()
                {
                    FirstName = "ankita",
                    LastName = "ghosh",
                    EmployeeId = "123456",
                    UserId = 123
                }
            };

            var controller = new TaskController(new MockTaskBC(context));
            var result = controller.InsertTaskDetails(task) as JSendResponse;


            Assert.IsNotNull(result);

            Assert.IsNotNull((context.Users.Local[0]).Task_ID);
        }

        [TestMethod]
        public void TestUpdateProjects_Success()
        {
            var context = new MockProjectManagerEntities();
            var tasks = new TestDbSet<DAC.Task>();
            var users = new TestDbSet<DAC.User>();
            users.Add(new DAC.User()
            {
                Employee_ID = 418220.ToString(),
                First_Name = "TEST",
                Last_Name = "TEST2",
                Project_ID = 123,
                Task_ID = 123,
                User_ID = 123
            });
            tasks.Add(new DAC.Task()
            {
                Task_ID = 1,
                Task1 = "ASDQW",
                Parent_ID = 123674,
                Project_ID = 34856,
                Start_Date = DateTime.Now,
                End_Date = DateTime.Now.AddDays(2),
                Priority = 10,
                Status = 0
            });
            context.Tasks = tasks;
            context.Users = users;
            var testTask = new Models.Task()
            {
                TaskId = 1,
                Task_Name = "task1",
                Parent_ID = 123674,
                Project_ID = 34856,
                Start_Date = DateTime.Now,
                End_Date = DateTime.Now.AddDays(2),
                Priority = 30,
                Status = 0,
                User = new User()
                {
                    FirstName = "ankita",
                    LastName = "ghosh",
                    EmployeeId = "123456",
                    UserId = 123
                }
            };

            var controller = new TaskController(new MockTaskBC(context));
            var result = controller.UpdateTaskDetails(testTask) as JSendResponse;

            Assert.IsNotNull(result);
            Assert.AreEqual((context.Tasks.Local[0]).Priority, 30);
        }

        [TestMethod]
        public void TestDeleteProjects_Success()
        {
            var context = new MockProjectManagerEntities();
            var tasks = new TestDbSet<DAC.Task>();

            tasks.Add(new DAC.Task()
            {
                Task_ID = 1,
                Task1 = "task1",
                Parent_ID = 123674,
                Project_ID = 34856,
                Start_Date = DateTime.Now,
                End_Date = DateTime.Now.AddDays(2),
                Priority = 10,
                Status = 0
            });
            context.Tasks = tasks;
            var testTask = new Models.Task()
            {
                TaskId = 1,
                Task_Name = "task1",
                Parent_ID = 123674,
                Project_ID = 34856,
                Start_Date = DateTime.Now,
                End_Date = DateTime.Now.AddDays(2),
                Priority = 10,
                Status = 0
            };

            var controller = new TaskController(new MockTaskBC(context));
            var result = controller.DeleteTaskDetails(testTask) as JSendResponse;

            Assert.IsNotNull(result);
            Assert.AreEqual((context.Tasks.Local[0]).Status, 1);
        }

        [TestMethod]
        public void TestRetrieveTaskByProjectId_Success()
        {
            var context = new MockProjectManagerEntities();
            var tasks = new TestDbSet<DAC.Task>();
            var users = new TestDbSet<DAC.User>();
            var parentTasks = new TestDbSet<DAC.ParentTask>();
            parentTasks.Add(new DAC.ParentTask()
            {
                Parent_ID = 12345,
                Parent_Task = "ANB"

            });
            context.ParentTasks = parentTasks;
            users.Add(new DAC.User()
            {
                Employee_ID = "414942",
                First_Name = "ankita",
                Last_Name = "ghosh",
                User_ID = 123,
                Task_ID = 12345,
                Project_ID = 1234
            });
            context.Users = users;
            tasks.Add(new DAC.Task()
            {
                Project_ID = 12345,
                Parent_ID = 12345,
                Task_ID = 12345,
                Task1 = "TEST",
                Priority = 1,
                Status = 1,
                Start_Date = DateTime.Now,
                End_Date = DateTime.Now.AddDays(5)
            });
            tasks.Add(new DAC.Task()
            {
                Project_ID = 123,
                Parent_ID = 123,
                Task_ID = 123,
                Task1 = "TEST",
                Priority = 1,
                Status = 1,
                Start_Date = DateTime.Now,
                End_Date = DateTime.Now.AddDays(5)
            });
            context.Tasks = tasks;

            var controller = new TaskController(new MockTaskBC(context));
            var result = controller.RetrieveTaskByProjectId(12345) as JSendResponse;

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Data, typeof(List<PMServer.Models.Task>));
            Assert.AreEqual((result.Data as List<PMServer.Models.Task>).Count, 1);
            Assert.AreEqual((result.Data as List<PMServer.Models.Task>)[0].Task_Name, "TEST");
        }

        [TestMethod]
        [ExpectedException(typeof(ArithmeticException))]
        public void TestRetrieveTaskByProjectId_NegativeTaskId()
        {
            var context = new MockProjectManagerEntities();

            var controller = new TaskController(new MockTaskBC(context));
            var result = controller.RetrieveTaskByProjectId(-12345) as JSendResponse;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestInsertTask_NullTaskObject()
        {
            var context = new MockProjectManagerEntities();

            var controller = new TaskController(new MockTaskBC(context));
            var result = controller.InsertTaskDetails(null) as JSendResponse;
        }


        [TestMethod]
        [ExpectedException(typeof(ArithmeticException))]
        public void TestInsertTask_NegativeTaskParentId()
        {
            var context = new MockProjectManagerEntities();
            var task = new Models.Task();
            task.Parent_ID = -234;
            var controller = new TaskController(new MockTaskBC(context));
            var result = controller.InsertTaskDetails(task) as JSendResponse;
        }

        [TestMethod]
        [ExpectedException(typeof(ArithmeticException))]
        public void TestInsertTask_NegativeProjectId()
        {
            var context = new MockProjectManagerEntities();
            var task = new Models.Task();
            task.Project_ID = -234;
            var controller = new TaskController(new MockTaskBC(context));
            var result = controller.InsertTaskDetails(task) as JSendResponse;
        }

        [TestMethod]
        [ExpectedException(typeof(ArithmeticException))]
        public void TestInsertTask_NegativeTaskId()
        {
            var context = new MockProjectManagerEntities();
            var task = new Models.Task();
            task.TaskId = -234;
            var controller = new TaskController(new MockTaskBC(context));
            var result = controller.InsertTaskDetails(task) as JSendResponse;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestUpdateTask_NullTaskObject()
        {
            var context = new MockProjectManagerEntities();

            var controller = new TaskController(new MockTaskBC(context));
            var result = controller.UpdateTaskDetails(null) as JSendResponse;
        }

        [TestMethod]
        [ExpectedException(typeof(ArithmeticException))]
        public void TestUpdateTask_NegativeTaskParentId()
        {
            var context = new MockProjectManagerEntities();
            var task = new Models.Task();
            task.Parent_ID = -234;
            var controller = new TaskController(new MockTaskBC(context));
            var result = controller.UpdateTaskDetails(task) as JSendResponse;
        }

        [TestMethod]
        [ExpectedException(typeof(ArithmeticException))]
        public void TestUpdateTask_NegativeProjectId()
        {
            var context = new MockProjectManagerEntities();
            var task = new Models.Task();
            task.Project_ID = -234;
            var controller = new TaskController(new MockTaskBC(context));
            var result = controller.UpdateTaskDetails(task) as JSendResponse;
        }

        [TestMethod]
        [ExpectedException(typeof(ArithmeticException))]
        public void TestUpdateTask_NegativeTaskId()
        {
            var context = new MockProjectManagerEntities();
            var task = new Models.Task();
            task.TaskId = -234;
            var controller = new TaskController(new MockTaskBC(context));
            var result = controller.UpdateTaskDetails(task) as JSendResponse;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestDeleteTask_NullTaskObject()
        {
            var context = new MockProjectManagerEntities();

            var controller = new TaskController(new MockTaskBC(context));
            var result = controller.DeleteTaskDetails(null) as JSendResponse;
        }

        [TestMethod]
        [ExpectedException(typeof(ArithmeticException))]
        public void TestDeleteTask_NegativeTaskParentId()
        {
            var context = new MockProjectManagerEntities();
            var task = new Models.Task();
            task.Parent_ID = -234;
            var controller = new TaskController(new MockTaskBC(context));
            var result = controller.DeleteTaskDetails(task) as JSendResponse;
        }

        [TestMethod]
        [ExpectedException(typeof(ArithmeticException))]
        public void TestDeleteTask_NegativeProjectId()
        {
            var context = new MockProjectManagerEntities();
            var task = new Models.Task();
            task.Project_ID = -234;
            var controller = new TaskController(new MockTaskBC(context));
            var result = controller.DeleteTaskDetails(task) as JSendResponse;
        }

        [TestMethod]
        [ExpectedException(typeof(ArithmeticException))]
        public void TestDeleteTask_NegativeTaskId()
        {
            var context = new MockProjectManagerEntities();
            var task = new Models.Task();
            task.TaskId = -234;
            var controller = new TaskController(new MockTaskBC(context));
            var result = controller.DeleteTaskDetails(task) as JSendResponse;
        }
    }
}
