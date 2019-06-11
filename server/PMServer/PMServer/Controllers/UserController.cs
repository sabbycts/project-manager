using PMServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using PMServer.BC;
using PMServer.Filters;
using DAC = PMServer.DAC;

namespace PMServer.Controllers
{
    public class UserController : ApiController
    {
        UserBC _userObjBC = null;

        public UserController()
        {
            _userObjBC = new UserBC();
        }

        public UserController(UserBC userObjBC)
        {
            _userObjBC = userObjBC;
        }

        [HttpGet]
        [ProjectManagerTransactionFilter]
        [ProjectManagerExceptionFilter]
        [Route("api/user")]
        public JSendResponse GetUser()
        {
            List<User> Users = _userObjBC.GetUser();

            return new JSendResponse()
            {
                Data = Users
            };
        }

        [HttpPost]
        [ProjectManagerTransactionFilter]
        [ProjectManagerExceptionFilter]
        [Route("api/user/add")]
        public JSendResponse InsertUserDetails(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("User id is null");
            }
            try
            {
                int employeeId = Convert.ToInt32(user.EmployeeId);
            }
            catch (FormatException ex)
            {
                throw new FormatException("Invalid format of employee Id", ex);
            }
            if (Convert.ToInt32(user.EmployeeId) < 0)
            {
                throw new ArithmeticException("Employee id cannot be negative");
            }
            if (Convert.ToInt32(user.ProjectId) < 0)
            {
                throw new ArithmeticException("Project id cannot be negative");
            }
            return new JSendResponse()
            {
                Data = _userObjBC.InsertUserDetails(user)
            };

        }

        [HttpPost]
        [Route("api/user/update")]
        [ProjectManagerTransactionFilter]
        [ProjectManagerExceptionFilter]
        public JSendResponse UpdateUserDetails(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("User id is null");
            }
            try
            {
                int employeeId = Convert.ToInt32(user.EmployeeId);
            }
            catch (FormatException ex)
            {
                throw new FormatException("Invalid format of employee Id", ex);
            }
            if (Convert.ToInt32(user.EmployeeId) < 0)
            {
                throw new ArithmeticException("Employee id cannot be negative");
            }
            if (Convert.ToInt32(user.ProjectId) < 0)
            {
                throw new ArithmeticException("Project id cannot be negative");
            }
            if (user.UserId <= 0)
            {
                throw new ArithmeticException("User id cannot be negative or 0");
            }
            return new JSendResponse()
            {
                Data = _userObjBC.UpdateUserDetails(user)
            };
        }

        [HttpPost]
        [ProjectManagerTransactionFilter]
        [ProjectManagerExceptionFilter]
        [Route("api/user/delete")]
        public JSendResponse DeleteUserDetails(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("User id is null");
            }
            try
            {
                int employeeId = Convert.ToInt32(user.EmployeeId);
            }
            catch (FormatException ex)
            {
                throw new FormatException("Invalid format of employee Id", ex);
            }
            if (Convert.ToInt32(user.EmployeeId) < 0)
            {
                throw new ArithmeticException("Employee id cannot be negative");
            }
            if (Convert.ToInt32(user.ProjectId) < 0)
            {
                throw new ArithmeticException("Project id cannot be negative");
            }
            if (user.UserId <= 0)
            {
                throw new ArithmeticException("User id cannot be negative or 0");
            }
            return new JSendResponse()
            {
                Data = _userObjBC.DeleteUserDetails(user)
            };
        }
    }
}
