using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PMServer.Controllers;
using System.Collections.Generic;
using System.Web;
using PMServer.Models;
using System.Data.Entity;
using System.Linq;

namespace PMServer.Test
{
    class MockUserBC : BC.UserBC
    {
        MockProjectManagerEntities _mockBd = null;

        public MockUserBC(MockProjectManagerEntities mockBd)
        {
            _mockBd = mockBd;
        }

        public override List<User> GetUser()
        {
            return _mockBd.Users.Select(x => new User()
            {
                FirstName = x.First_Name,
                LastName = x.Last_Name,
                EmployeeId = x.Employee_ID,
                UserId = x.User_ID
            }).ToList();
        }

        public override int InsertUserDetails(User user)
        {
            _mockBd.Users.Add(new DAC.User()
            {
                Last_Name = user.LastName,
                First_Name = user.FirstName,
                Employee_ID = user.EmployeeId
            });
            return 1;
        }

        public override int UpdateUserDetails(User user)
        {
            var editDetails = (from editUser in _mockBd.Users
                               where editUser.User_ID == user.UserId
                               select editUser).First();
            // Modify existing records
            if (editDetails != null)
            {
                editDetails.First_Name = user.FirstName;
                editDetails.Last_Name = user.LastName;
                editDetails.Employee_ID = user.EmployeeId;

            }
            return 1;
        }

        public override int DeleteUserDetails(User user)
        {
            var editDetails = (from editUser in _mockBd.Users
                               where editUser.User_ID == user.UserId
                               select editUser).First();
            // Delete existing record
            if (editDetails != null)
            {
                _mockBd.Users.Remove(editDetails);
            }
            return 1; 
        }
    }

    [TestClass]
    public class UserControllerTest
    {
        [TestMethod]
        public void TestGetUser_Success()
        {
            var context = new MockProjectManagerEntities();
            var users = new TestDbSet<DAC.User>();
            users.Add(new DAC.User()
            {
                Employee_ID = "418220",
                First_Name = "Prateek",
                Last_Name = "Gangopadhyay",
                Project_ID = 123,
                Task_ID = 123,
                User_ID = 418220
            });
            users.Add(new DAC.User()
            {
                Employee_ID = "503322",
                First_Name = "Khushboo",
                Last_Name = "Jain",
                Project_ID = 1234,
                Task_ID = 1234,
                User_ID = 503322
            });
            context.Users = users;

            var controller = new UserController(new MockUserBC(context));
            var result = controller.GetUser() as JSendResponse;

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Data, typeof(List<User>));
            Assert.AreEqual((result.Data as List<User>).Count, 2);
        }

        [TestMethod]
        public void TestInsertUser_Success()
        {
            var context = new MockProjectManagerEntities();
            context.Users = new TestDbSet<DAC.User>();

            var user = new Models.User();
            user.FirstName = "ankita";
            user.LastName = "ghosh";
            user.EmployeeId = "123456";
            user.UserId = 123;
            var controller = new UserController(new MockUserBC(context));
            var result = controller.InsertUserDetails(user) as JSendResponse;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Data, 1);
        }

        [TestMethod]
        public void TestUpdateUser_Success()
        {
            var context = new MockProjectManagerEntities();
            var users = new TestDbSet<DAC.User>();
            
            users.Add(new DAC.User()
            {
                Employee_ID = "503322",
                First_Name = "Khushboo",
                Last_Name = "Jain",
                Project_ID = 1234,
                Task_ID = 1234,
                User_ID = 503322
            });
            context.Users = users;

            var user = new Models.User();
            user.FirstName = "Khush";
            user.LastName = "jain";
            user.EmployeeId = "123";
            user.UserId = 503322;

            var controller = new UserController(new MockUserBC(context));
            var result = controller.UpdateUserDetails(user) as JSendResponse;

            Assert.IsNotNull(result);
            Assert.AreEqual((context.Users.Local[0]).First_Name.ToUpper(), "KHUSH");
        }

        [TestMethod]
        public void TestDeleteUser_Success()
        {
            var context = new MockProjectManagerEntities();
            var users = new TestDbSet<DAC.User>();
            users.Add(new DAC.User()
            {
                Employee_ID = "418220",
                First_Name = "Prateek",
                Last_Name = "Gangopadhyay",
                Project_ID = 123,
                Task_ID = 123,
                User_ID = 418220
            });
            users.Add(new DAC.User()
            {
                Employee_ID = "503322",
                First_Name = "Khushboo",
                Last_Name = "Jain",
                Project_ID = 1234,
                Task_ID = 1234,
                User_ID = 503322
            });
            context.Users = users;

            var user = new Models.User();
            user.FirstName = "Khushboo";
            user.LastName = "Jain";
            user.EmployeeId = "503322";
            user.UserId = 503322;

            var controller = new UserController(new MockUserBC(context));
            var result = controller.DeleteUserDetails(user) as JSendResponse;

            Assert.IsNotNull(result);
            Assert.AreEqual(context.Users.Local.Count,1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestDeleteUser_UserNullException()
        {
            var context = new MockProjectManagerEntities();
            var users = new TestDbSet<DAC.User>();
            users.Add(new DAC.User()
            {
                Employee_ID = "418220",
                First_Name = "Prateek",
                Last_Name = "Gangopadhyay",
                Project_ID = 123,
                Task_ID = 123,
                User_ID = 418220
            });
            users.Add(new DAC.User()
            {
                Employee_ID = "503322",
                First_Name = "Khushboo",
                Last_Name = "Jain",
                Project_ID = 1234,
                Task_ID = 1234,
                User_ID = 503322
            });
            context.Users = users;

            var user = new Models.User();
            user = null;

            var controller = new UserController(new MockUserBC(context));
            var result = controller.DeleteUserDetails(user) as JSendResponse;
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void TestDeleteUser_InvalidEmployeeId()
        {
            var context = new MockProjectManagerEntities();
            var users = new TestDbSet<DAC.User>();
            users.Add(new DAC.User()
            {
                Employee_ID = "418220",
                First_Name = "Prateek",
                Last_Name = "Gangopadhyay",
                Project_ID = 123,
                Task_ID = 123,
                User_ID = 418220
            });
            users.Add(new DAC.User()
            {
                Employee_ID = "503322",
                First_Name = "Khushboo",
                Last_Name = "Jain",
                Project_ID = 1234,
                Task_ID = 1234,
                User_ID = 503322
            });
            context.Users = users;

            var user = new Models.User();
            user.EmployeeId = "TEST";

            var controller = new UserController(new MockUserBC(context));
            var result = controller.DeleteUserDetails(user) as JSendResponse;
        }

        [TestMethod]
        [ExpectedException(typeof(ArithmeticException))]
        public void TestDeleteUser_NegativeEmployeeId()
        {
            var context = new MockProjectManagerEntities();
            var users = new TestDbSet<DAC.User>();
            users.Add(new DAC.User()
            {
                Employee_ID = "418220",
                First_Name = "Prateek",
                Last_Name = "Gangopadhyay",
                Project_ID = 123,
                Task_ID = 123,
                User_ID = 418220
            });
            users.Add(new DAC.User()
            {
                Employee_ID = "503322",
                First_Name = "Khushboo",
                Last_Name = "Jain",
                Project_ID = 1234,
                Task_ID = 1234,
                User_ID = 503322
            });
            context.Users = users;

            var user = new Models.User();
            user.EmployeeId = "-233";

            var controller = new UserController(new MockUserBC(context));
            var result = controller.DeleteUserDetails(user) as JSendResponse;
        }

        [TestMethod]
        [ExpectedException(typeof(ArithmeticException))]
        public void TestDeleteUser_InvalidProjectIdFormat()
        {
            var context = new MockProjectManagerEntities();
            var users = new TestDbSet<DAC.User>();
            users.Add(new DAC.User()
            {
                Employee_ID = "418220",
                First_Name = "Prateek",
                Last_Name = "Gangopadhyay",
                Project_ID = 123,
                Task_ID = 123,
                User_ID = 418220
            });
            users.Add(new DAC.User()
            {
                Employee_ID = "503322",
                First_Name = "Khushboo",
                Last_Name = "Jain",
                Project_ID = 1234,
                Task_ID = 1234,
                User_ID = 503322
            });
            context.Users = users;

            var user = new Models.User();
            user.ProjectId = -1;

            var controller = new UserController(new MockUserBC(context));
            var result = controller.DeleteUserDetails(user) as JSendResponse;
        }

        [TestMethod]
        [ExpectedException(typeof(ArithmeticException))]
        public void TestDeleteUser_NegativeUserIdFormat()
        {
            var context = new MockProjectManagerEntities();
            var users = new TestDbSet<DAC.User>();
            users.Add(new DAC.User()
            {
                Employee_ID = "418220",
                First_Name = "Prateek",
                Last_Name = "Gangopadhyay",
                Project_ID = 123,
                Task_ID = 123,
                User_ID = 418220
            });
            users.Add(new DAC.User()
            {
                Employee_ID = "503322",
                First_Name = "Khushboo",
                Last_Name = "Jain",
                Project_ID = 1234,
                Task_ID = 1234,
                User_ID = 503322
            });
            context.Users = users;

            var user = new Models.User();
            user.UserId = -1;

            var controller = new UserController(new MockUserBC(context));
            var result = controller.DeleteUserDetails(user) as JSendResponse;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestUpdateUser_UserNullException()
        {
            var context = new MockProjectManagerEntities();
            var users = new TestDbSet<DAC.User>();
            users.Add(new DAC.User()
            {
                Employee_ID = "418220",
                First_Name = "Prateek",
                Last_Name = "Gangopadhyay",
                Project_ID = 123,
                Task_ID = 123,
                User_ID = 418220
            });
            users.Add(new DAC.User()
            {
                Employee_ID = "503322",
                First_Name = "Khushboo",
                Last_Name = "Jain",
                Project_ID = 1234,
                Task_ID = 1234,
                User_ID = 503322
            });
            context.Users = users;

            var user = new Models.User();
            user = null;

            var controller = new UserController(new MockUserBC(context));
            var result = controller.UpdateUserDetails(user) as JSendResponse;
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void TestUpdateUser_InvalidEmployeeId()
        {
            var context = new MockProjectManagerEntities();
            var users = new TestDbSet<DAC.User>();
            users.Add(new DAC.User()
            {
                Employee_ID = "418220",
                First_Name = "Prateek",
                Last_Name = "Gangopadhyay",
                Project_ID = 123,
                Task_ID = 123,
                User_ID = 418220
            });
            users.Add(new DAC.User()
            {
                Employee_ID = "503322",
                First_Name = "Khushboo",
                Last_Name = "Jain",
                Project_ID = 1234,
                Task_ID = 1234,
                User_ID = 503322
            });
            context.Users = users;

            var user = new Models.User();
            user.EmployeeId = "TEST";

            var controller = new UserController(new MockUserBC(context));
            var result = controller.UpdateUserDetails(user) as JSendResponse;
        }

        [TestMethod]
        [ExpectedException(typeof(ArithmeticException))]
        public void TestUpdateUser_NegativeEmployeeId()
        {
            var context = new MockProjectManagerEntities();
            var users = new TestDbSet<DAC.User>();
            users.Add(new DAC.User()
            {
                Employee_ID = "418220",
                First_Name = "Prateek",
                Last_Name = "Gangopadhyay",
                Project_ID = 123,
                Task_ID = 123,
                User_ID = 418220
            });
            users.Add(new DAC.User()
            {
                Employee_ID = "503322",
                First_Name = "Khushboo",
                Last_Name = "Jain",
                Project_ID = 1234,
                Task_ID = 1234,
                User_ID = 503322
            });
            context.Users = users;

            var user = new Models.User();
            user.EmployeeId = "-233";

            var controller = new UserController(new MockUserBC(context));
            var result = controller.UpdateUserDetails(user) as JSendResponse;
        }

        [TestMethod]
        [ExpectedException(typeof(ArithmeticException))]
        public void TestUpdateUser_InvalidProjectIdFormat()
        {
            var context = new MockProjectManagerEntities();
            var users = new TestDbSet<DAC.User>();
            users.Add(new DAC.User()
            {
                Employee_ID = "418220",
                First_Name = "Prateek",
                Last_Name = "Gangopadhyay",
                Project_ID = 123,
                Task_ID = 123,
                User_ID = 418220
            });
            users.Add(new DAC.User()
            {
                Employee_ID = "503322",
                First_Name = "Khushboo",
                Last_Name = "Jain",
                Project_ID = 1234,
                Task_ID = 1234,
                User_ID = 503322
            });
            context.Users = users;

            var user = new Models.User();
            user.ProjectId = -1;

            var controller = new UserController(new MockUserBC(context));
            var result = controller.UpdateUserDetails(user) as JSendResponse;
        }

        [TestMethod]
        [ExpectedException(typeof(ArithmeticException))]
        public void TestUpdateUser_NegativeUserIdFormat()
        {
            var context = new MockProjectManagerEntities();
            var users = new TestDbSet<DAC.User>();
            users.Add(new DAC.User()
            {
                Employee_ID = "418220",
                First_Name = "Prateek",
                Last_Name = "Gangopadhyay",
                Project_ID = 123,
                Task_ID = 123,
                User_ID = 418220
            });
            users.Add(new DAC.User()
            {
                Employee_ID = "503322",
                First_Name = "Khushboo",
                Last_Name = "Jain",
                Project_ID = 1234,
                Task_ID = 1234,
                User_ID = 503322
            });
            context.Users = users;

            var user = new Models.User();
            user.UserId = -1;

            var controller = new UserController(new MockUserBC(context));
            var result = controller.UpdateUserDetails(user) as JSendResponse;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestInsertUser_UserNullException()
        {
            var context = new MockProjectManagerEntities();
            var users = new TestDbSet<DAC.User>();
            users.Add(new DAC.User()
            {
                Employee_ID = "418220",
                First_Name = "Prateek",
                Last_Name = "Gangopadhyay",
                Project_ID = 123,
                Task_ID = 123,
                User_ID = 418220
            });
            users.Add(new DAC.User()
            {
                Employee_ID = "503322",
                First_Name = "Khushboo",
                Last_Name = "Jain",
                Project_ID = 1234,
                Task_ID = 1234,
                User_ID = 503322
            });
            context.Users = users;

            var user = new Models.User();
            user = null;

            var controller = new UserController(new MockUserBC(context));
            var result = controller.InsertUserDetails(user) as JSendResponse;
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void TestInsertUser_InvalidEmployeeId()
        {
            var context = new MockProjectManagerEntities();
            var users = new TestDbSet<DAC.User>();
            users.Add(new DAC.User()
            {
                Employee_ID = "418220",
                First_Name = "Prateek",
                Last_Name = "Gangopadhyay",
                Project_ID = 123,
                Task_ID = 123,
                User_ID = 418220
            });
            users.Add(new DAC.User()
            {
                Employee_ID = "503322",
                First_Name = "Khushboo",
                Last_Name = "Jain",
                Project_ID = 1234,
                Task_ID = 1234,
                User_ID = 503322
            });
            context.Users = users;

            var user = new Models.User();
            user.EmployeeId = "TEST";

            var controller = new UserController(new MockUserBC(context));
            var result = controller.InsertUserDetails(user) as JSendResponse;
        }

        [TestMethod]
        [ExpectedException(typeof(ArithmeticException))]
        public void TestInsertUser_NegativeEmployeeId()
        {
            var context = new MockProjectManagerEntities();
            var users = new TestDbSet<DAC.User>();
            users.Add(new DAC.User()
            {
                Employee_ID = "418220",
                First_Name = "Prateek",
                Last_Name = "Gangopadhyay",
                Project_ID = 123,
                Task_ID = 123,
                User_ID = 418220
            });
            users.Add(new DAC.User()
            {
                Employee_ID = "503322",
                First_Name = "Khushboo",
                Last_Name = "Jain",
                Project_ID = 1234,
                Task_ID = 1234,
                User_ID = 503322
            });
            context.Users = users;

            var user = new Models.User();
            user.EmployeeId = "-233";

            var controller = new UserController(new MockUserBC(context));
            var result = controller.InsertUserDetails(user) as JSendResponse;
        }

        [TestMethod]
        [ExpectedException(typeof(ArithmeticException))]
        public void TestInsertUser_InvalidProjectIdFormat()
        {
            var context = new MockProjectManagerEntities();
            var users = new TestDbSet<DAC.User>();
            users.Add(new DAC.User()
            {
                Employee_ID = "418220",
                First_Name = "Prateek",
                Last_Name = "Gangopadhyay",
                Project_ID = 123,
                Task_ID = 123,
                User_ID = 418220
            });
            users.Add(new DAC.User()
            {
                Employee_ID = "503322",
                First_Name = "Khushboo",
                Last_Name = "Jain",
                Project_ID = 1234,
                Task_ID = 1234,
                User_ID = 503322
            });
            context.Users = users;

            var user = new Models.User();
            user.ProjectId = -1;

            var controller = new UserController(new MockUserBC(context));
            var result = controller.InsertUserDetails(user) as JSendResponse;
        }
    }
}

