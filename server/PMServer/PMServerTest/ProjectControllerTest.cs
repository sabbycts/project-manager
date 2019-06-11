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
    class MockProjectBC : BC.ProjectBC
    {
        MockProjectManagerEntities _mockBd = null;

        public MockProjectBC(MockProjectManagerEntities mockBd)
        {
            _mockBd = mockBd;
        }
        public override List<Models.Project> RetrieveProjects()
        {
            return _mockBd.Projects.Select(x => new Models.Project()
            {
                ProjectId = x.Project_ID,
                ProjectName = x.Project1,
                ProjectEndDate = x.End_Date,
                ProjectStartDate = x.Start_Date,
                Priority = x.Priority,
                User = _mockBd.Users.Where(y => y.Project_ID == x.Project_ID).Select(z => new Models.User()
                {
                    UserId = z.User_ID
                }).FirstOrDefault(),
                NoOfTasks = _mockBd.Tasks.Where(y => y.Project_ID == x.Project_ID).Count(),
                NoOfCompletedTasks = _mockBd.Tasks.Where(y => y.Project_ID == x.Project_ID && y.Status == 1).Count(),
            }).ToList();
        }

        public override int InsertProjectDetails(Models.Project project)
        {
            DAC.Project proj = new DAC.Project()
            {
                Project1 = project.ProjectName,
                Start_Date = project.ProjectStartDate,
                End_Date = project.ProjectEndDate,
                Priority = project.Priority
            };
            _mockBd.Projects.Add(proj);
            var editDetails = (from editUser in _mockBd.Users
                               where editUser.User_ID.ToString().Contains(project.User.UserId.ToString())
                               select editUser).First();
            // Modify existing records
            if (editDetails != null)
            {
                editDetails.Project_ID = proj.Project_ID;
            }
            return 1;
        }

        public override int UpdateProjectDetails(Models.Project project)
        {
            var editProjDetails = (from editProject in _mockBd.Projects
                                   where editProject.Project_ID.ToString().Contains(project.ProjectId.ToString())
                                   select editProject).First();
            // Modify existing records
            if (editProjDetails != null)
            {
                editProjDetails.Project1 = project.ProjectName;
                editProjDetails.Start_Date = project.ProjectStartDate;
                editProjDetails.End_Date = project.ProjectEndDate;
                editProjDetails.Priority = project.Priority;
            }


            var editDetails = (from editUser in _mockBd.Users
                               where editUser.User_ID.ToString().Contains(project.User.UserId.ToString())
                               select editUser).First();
            // Modify existing records
            if (editDetails != null)
            {
                editDetails.Project_ID = project.ProjectId;
            }
            return 1;
        }
        public override int DeleteProjectDetails(Models.Project project)
        {
            var editDetails = (from proj in _mockBd.Projects
                               where proj.Project_ID == project.ProjectId
                               select proj).First();
            // Delete existing record
            if (editDetails != null)
            {
                _mockBd.Projects.Remove(editDetails);
            }
            return 1;
        }

    }
    [TestClass]
    public class ProjectControllerTest
    {
        [TestMethod]
        public void TestGetProjects_Success()
        {
            var context = new MockProjectManagerEntities();
            var projects = new TestDbSet<DAC.Project>();
            projects.Add(new DAC.Project()
            {
                Project_ID = 1234,
                Project1 = "MyProject",
                Start_Date = DateTime.Now,
                End_Date = DateTime.Now.AddDays(5),
                Priority = 3
            });
            projects.Add(new DAC.Project()
            {
                Project_ID = 12345,
                Project1 = "MyProject",
                Start_Date = DateTime.Now,
                End_Date = DateTime.Now.AddDays(5),
                Priority = 3
            });
            context.Projects = projects;
            context.Users = new TestDbSet<DAC.User>();
            context.Tasks = new TestDbSet<DAC.Task>();

            var controller = new ProjectController(new MockProjectBC(context));
            var result = controller.RetrieveProjects() as JSendResponse;

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Data, typeof(List<Project>));
            Assert.AreEqual((result.Data as List<Project>).Count, 2);
        }

        [TestMethod]
        public void TestInsertProjects_Success()
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
            context.Projects = new TestDbSet<DAC.Project>();

            var testProject = new Models.Project()
            {
                ProjectId = 12345,
                ProjectName = "MyProject",
                ProjectStartDate = DateTime.Now,
                ProjectEndDate = DateTime.Now.AddDays(5),
                Priority = 3,
                NoOfCompletedTasks = 3,
                NoOfTasks = 5,
                User = new User()
                {
                    FirstName = "ankita",
                    LastName = "ghosh",
                    EmployeeId = "123456",
                    UserId = 123
                }
            };
            var controller = new ProjectController(new MockProjectBC(context));
            var result = controller.InsertProjectDetails(testProject) as JSendResponse;

            Assert.IsNotNull(result);
            Assert.IsNotNull((context.Users.Local[0]).Project_ID);
        }

        [TestMethod]
        public void TestUpdateProjects_Success()
        {
            var context = new MockProjectManagerEntities();
            var projects = new TestDbSet<DAC.Project>();
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
            projects.Add(new DAC.Project()
            {
                Project1 = "MyTestProject",
                Start_Date = DateTime.Now,
                End_Date = DateTime.Now.AddDays(5),
                Priority = 2,
                Project_ID = 123
            });
            context.Projects = projects;
            context.Users = users;
            var testProject = new Models.Project()
            {
                ProjectId = 123,
                Priority = 3,
                NoOfCompletedTasks = 2,
                NoOfTasks = 5,
                ProjectName = "ProjectTest",
                ProjectStartDate = DateTime.Now,
                ProjectEndDate = DateTime.Now.AddDays(10),
                User = new User()
                {
                    EmployeeId = 418220.ToString(),
                    FirstName = "PRATEEK",
                    LastName = "GANGOPADHYAY",
                    ProjectId = 123,
                    UserId = 123
                }
            };

            var controller = new ProjectController(new MockProjectBC(context));
            var result = controller.UpdateProjectDetails(testProject) as JSendResponse;

            Assert.IsNotNull(result);
            Assert.AreEqual((context.Projects.Local[0]).Project1.ToUpper(), "PROJECTTEST");
        }

        [TestMethod]
        public void TestDeleteProjects_Success()
        {
            var context = new MockProjectManagerEntities();
            var projects = new TestDbSet<DAC.Project>();
            projects.Add(new DAC.Project()
            {
                Project_ID = 123,
                Project1 = "TEST",
                Priority = 1,
                Start_Date = DateTime.Now,
                End_Date = DateTime.Now.AddDays(5)
            });
            projects.Add(new DAC.Project()
            {
                Project_ID = 234,
                Project1 = "TEST2",
                Priority = 2,
                Start_Date = DateTime.Now,
                End_Date = DateTime.Now.AddDays(10)
            });
            context.Projects = projects;
            var controller = new ProjectController(new MockProjectBC(context));

            var testProject = new Models.Project()
            {
                ProjectId = 123,
                Priority = 3,
                NoOfCompletedTasks = 2,
                NoOfTasks = 5,
                ProjectName = "ProjectTest",
                ProjectStartDate = DateTime.Now,
                ProjectEndDate = DateTime.Now.AddDays(10),
                User = new User()
                {
                    EmployeeId = 418220.ToString(),
                    FirstName = "PRATEEK",
                    LastName = "GANGOPADHYAY",
                    ProjectId = 123,
                    UserId = 123
                }
            };

            var result = controller.DeleteProjectDetails(testProject) as JSendResponse;
            Assert.IsNotNull(result);
            Assert.AreEqual(context.Projects.Local.Count, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestInsertProject_NoProjectAsParameter()
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
            Models.Project testProject = null;
            var controller = new ProjectController(new MockProjectBC(context));
            var result = controller.InsertProjectDetails(testProject) as JSendResponse;
        }

        [TestMethod]
        [ExpectedException(typeof(ArithmeticException))]
        public void TestInsertProject_NegativeProjectId()
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
            Models.Project testProject = new Models.Project()
            {
                ProjectId = -234,
                NoOfCompletedTasks = 4,
                NoOfTasks = 5,
                Priority = 1,
                ProjectEndDate = DateTime.Now.AddDays(10),
                ProjectStartDate = DateTime.Now,
                ProjectName = "TEST",
                User = new User()
                {
                    EmployeeId = 123.ToString(),
                    FirstName = "Prateek",
                    LastName = "Gangopadhyay",
                    ProjectId = -234,
                    UserId = 123
                }
            };
            var controller = new ProjectController(new MockProjectBC(context));
            var result = controller.InsertProjectDetails(testProject) as JSendResponse;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestInsertProject_UserNullInProject()
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
            Models.Project testProject = new Models.Project()
            {
                ProjectId = 222,
                NoOfCompletedTasks = 4,
                NoOfTasks = 5,
                Priority = 1,
                ProjectEndDate = DateTime.Now.AddDays(10),
                ProjectStartDate = DateTime.Now,
                ProjectName = "TEST",
                User = null
            };
            var controller = new ProjectController(new MockProjectBC(context));
            var result = controller.InsertProjectDetails(testProject) as JSendResponse;
        }

        [TestMethod]
        [ExpectedException(typeof(ArithmeticException))]
        public void TestInsertProject_NegativeProjectIdInUser()
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
            Models.Project testProject = new Models.Project()
            {
                ProjectId = 234,
                NoOfCompletedTasks = 4,
                NoOfTasks = 5,
                Priority = 1,
                ProjectEndDate = DateTime.Now.AddDays(10),
                ProjectStartDate = DateTime.Now,
                ProjectName = "TEST",
                User = new User()
                {
                    EmployeeId = 123.ToString(),
                    FirstName = "Prateek",
                    LastName = "Gangopadhyay",
                    ProjectId = -234,
                    UserId = 123
                }
            };
            var controller = new ProjectController(new MockProjectBC(context));
            var result = controller.InsertProjectDetails(testProject) as JSendResponse;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInsertProject_CompletedTasksGreater()
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
            Models.Project testProject = new Models.Project()
            {
                ProjectId = 234,
                NoOfCompletedTasks = 10,
                NoOfTasks = 5,
                Priority = 1,
                ProjectEndDate = DateTime.Now.AddDays(10),
                ProjectStartDate = DateTime.Now,
                ProjectName = "TEST",
                User = new User()
                {
                    EmployeeId = 123.ToString(),
                    FirstName = "Prateek",
                    LastName = "Gangopadhyay",
                    ProjectId = 234,
                    UserId = 123
                }
            };
            var controller = new ProjectController(new MockProjectBC(context));
            var result = controller.InsertProjectDetails(testProject) as JSendResponse;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestUpdateProject_NoProjectAsParameter()
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
            Models.Project testProject = null;
            var controller = new ProjectController(new MockProjectBC(context));
            var result = controller.UpdateProjectDetails(testProject) as JSendResponse;
        }

        [TestMethod]
        [ExpectedException(typeof(ArithmeticException))]
        public void TestUpdateProject_NegativeProjectId()
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
            Models.Project testProject = new Models.Project()
            {
                ProjectId = -234,
                NoOfCompletedTasks = 4,
                NoOfTasks = 5,
                Priority = 1,
                ProjectEndDate = DateTime.Now.AddDays(10),
                ProjectStartDate = DateTime.Now,
                ProjectName = "TEST",
                User = new User()
                {
                    EmployeeId = 123.ToString(),
                    FirstName = "Prateek",
                    LastName = "Gangopadhyay",
                    ProjectId = -234,
                    UserId = 123
                }
            };
            var controller = new ProjectController(new MockProjectBC(context));
            var result = controller.UpdateProjectDetails(testProject) as JSendResponse;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestUpdateProject_UserNullInProject()
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
            Models.Project testProject = new Models.Project()
            {
                ProjectId = 222,
                NoOfCompletedTasks = 4,
                NoOfTasks = 5,
                Priority = 1,
                ProjectEndDate = DateTime.Now.AddDays(10),
                ProjectStartDate = DateTime.Now,
                ProjectName = "TEST",
                User = null
            };
            var controller = new ProjectController(new MockProjectBC(context));
            var result = controller.UpdateProjectDetails(testProject) as JSendResponse;
        }

        [TestMethod]
        [ExpectedException(typeof(ArithmeticException))]
        public void TestUpdateProject_NegativeProjectIdInUser()
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
            Models.Project testProject = new Models.Project()
            {
                ProjectId = 234,
                NoOfCompletedTasks = 4,
                NoOfTasks = 5,
                Priority = 1,
                ProjectEndDate = DateTime.Now.AddDays(10),
                ProjectStartDate = DateTime.Now,
                ProjectName = "TEST",
                User = new User()
                {
                    EmployeeId = 123.ToString(),
                    FirstName = "Prateek",
                    LastName = "Gangopadhyay",
                    ProjectId = -234,
                    UserId = 123
                }
            };
            var controller = new ProjectController(new MockProjectBC(context));
            var result = controller.UpdateProjectDetails(testProject) as JSendResponse;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestUpdateProject_CompletedTasksGreater()
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
            Models.Project testProject = new Models.Project()
            {
                ProjectId = 234,
                NoOfCompletedTasks = 10,
                NoOfTasks = 5,
                Priority = 1,
                ProjectEndDate = DateTime.Now.AddDays(10),
                ProjectStartDate = DateTime.Now,
                ProjectName = "TEST",
                User = new User()
                {
                    EmployeeId = 123.ToString(),
                    FirstName = "Prateek",
                    LastName = "Gangopadhyay",
                    ProjectId = 234,
                    UserId = 123
                }
            };
            var controller = new ProjectController(new MockProjectBC(context));
            var result = controller.UpdateProjectDetails(testProject) as JSendResponse;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestDeleteProject_NoProjectAsParameter()
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
            Models.Project testProject = null;
            var controller = new ProjectController(new MockProjectBC(context));
            var result = controller.DeleteProjectDetails(testProject) as JSendResponse;
        }

        [TestMethod]
        [ExpectedException(typeof(ArithmeticException))]
        public void TestDeleteProject_NegativeProjectId()
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
            Models.Project testProject = new Models.Project()
            {
                ProjectId = -234,
                NoOfCompletedTasks = 4,
                NoOfTasks = 5,
                Priority = 1,
                ProjectEndDate = DateTime.Now.AddDays(10),
                ProjectStartDate = DateTime.Now,
                ProjectName = "TEST",
                User = new User()
                {
                    EmployeeId = 123.ToString(),
                    FirstName = "Prateek",
                    LastName = "Gangopadhyay",
                    ProjectId = -234,
                    UserId = 123
                }
            };
            var controller = new ProjectController(new MockProjectBC(context));
            var result = controller.DeleteProjectDetails(testProject) as JSendResponse;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestDeleteProject_UserNullInProject()
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
            Models.Project testProject = new Models.Project()
            {
                ProjectId = 222,
                NoOfCompletedTasks = 4,
                NoOfTasks = 5,
                Priority = 1,
                ProjectEndDate = DateTime.Now.AddDays(10),
                ProjectStartDate = DateTime.Now,
                ProjectName = "TEST",
                User = null
            };
            var controller = new ProjectController(new MockProjectBC(context));
            var result = controller.DeleteProjectDetails(testProject) as JSendResponse;
        }

        [TestMethod]
        [ExpectedException(typeof(ArithmeticException))]
        public void TestDeleteProject_NegativeProjectIdInUser()
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
            Models.Project testProject = new Models.Project()
            {
                ProjectId = 234,
                NoOfCompletedTasks = 4,
                NoOfTasks = 5,
                Priority = 1,
                ProjectEndDate = DateTime.Now.AddDays(10),
                ProjectStartDate = DateTime.Now,
                ProjectName = "TEST",
                User = new User()
                {
                    EmployeeId = 123.ToString(),
                    FirstName = "Prateek",
                    LastName = "Gangopadhyay",
                    ProjectId = -234,
                    UserId = 123
                }
            };
            var controller = new ProjectController(new MockProjectBC(context));
            var result = controller.DeleteProjectDetails(testProject) as JSendResponse;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestDeleteProject_CompletedTasksGreater()
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
            Models.Project testProject = new Models.Project()
            {
                ProjectId = 234,
                NoOfCompletedTasks = 10,
                NoOfTasks = 5,
                Priority = 1,
                ProjectEndDate = DateTime.Now.AddDays(10),
                ProjectStartDate = DateTime.Now,
                ProjectName = "TEST",
                User = new User()
                {
                    EmployeeId = 123.ToString(),
                    FirstName = "Prateek",
                    LastName = "Gangopadhyay",
                    ProjectId = 234,
                    UserId = 123
                }
            };
            var controller = new ProjectController(new MockProjectBC(context));
            var result = controller.DeleteProjectDetails(testProject) as JSendResponse;
        }
    }
}
