using PMServer.Models;
using PMServer.BC;
using System.Web.Http;
using PMServer.Filters;
using System.Collections.Generic;
using System;

namespace PMServer.Controllers
{
    public class TaskController : ApiController
    {
        TaskBC taskObj = null;

        public TaskController()
        {
            taskObj = new TaskBC();
        }

        public TaskController(TaskBC taskBc)
        {
            taskObj = taskBc;
        }

        [HttpGet]
        [Route("api/task")]
        [ProjectManagerTransactionFilter]
        [ProjectManagerExceptionFilter]
        public JSendResponse RetrieveTaskByProjectId(int projectId)
        {
            if (projectId < 0)
            {
                throw new ArithmeticException("ProjectId cannot be negative");
            }

            List<Task> Tasks = taskObj.RetrieveTaskByProjectId(projectId);

            return new JSendResponse()
            {
                Data = Tasks
            };

        }

        [HttpGet]
        [Route("api/task/parent")]
        [ProjectManagerTransactionFilter]
        [ProjectManagerExceptionFilter]
        public JSendResponse RetrieveParentTasks()
        {
            List<ParentTask> ParentTasks = taskObj.RetrieveParentTasks();

            return new JSendResponse()
            {
                Data = ParentTasks
            };

        }

        [HttpPost]
        [ProjectManagerTransactionFilter]
        [ProjectManagerExceptionFilter]
        [Route("api/task/add")]
        public JSendResponse InsertTaskDetails(Task task)
        {
            if (task == null)
            {
                throw new ArgumentNullException("Task object is null");
            }
            if (task.Parent_ID < 0)
            {
                throw new ArithmeticException("Parent Id of task cannot be negative");
            }
            if (task.Project_ID < 0)
            {
                throw new ArithmeticException("Project Id cannot be negative");
            }
            if (task.TaskId < 0)
            {
                throw new ArithmeticException("Task id cannot be negative");
            }
            return new JSendResponse()
            {
                Data = taskObj.InsertTaskDetails(task)
            };

        }

        [HttpPost]
        [ProjectManagerTransactionFilter]
        [ProjectManagerExceptionFilter]
        [Route("api/task/update")]
        public JSendResponse UpdateTaskDetails(Task task)
        {
            if (task == null)
            {
                throw new ArgumentNullException("Task object is null");
            }
            if (task.Parent_ID < 0)
            {
                throw new ArithmeticException("Parent Id of task cannot be negative");
            }
            if (task.Project_ID < 0)
            {
                throw new ArithmeticException("Project Id cannot be negative");
            }
            if (task.TaskId < 0)
            {
                throw new ArithmeticException("Task id cannot be negative");
            }
            return new JSendResponse()
            {
                Data = taskObj.UpdateTaskDetails(task)
            };

        }
        [HttpPost]
        [ProjectManagerTransactionFilter]
        [ProjectManagerExceptionFilter]
        [Route("api/task/delete")]
        public JSendResponse DeleteTaskDetails(Task task)
        {
            if (task == null)
            {
                throw new ArgumentNullException("Task object is null");
            }
            if (task.Parent_ID < 0)
            {
                throw new ArithmeticException("Parent Id of task cannot be negative");
            }
            if (task.Project_ID < 0)
            {
                throw new ArithmeticException("Project Id cannot be negative");
            }
            if (task.TaskId < 0)
            {
                throw new ArithmeticException("Task id cannot be negative");
            }
            return new JSendResponse()
            {
                Data = taskObj.DeleteTaskDetails(task)
            };
        }
    }
}
