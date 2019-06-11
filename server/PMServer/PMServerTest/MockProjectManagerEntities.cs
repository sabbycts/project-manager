using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMServer.Test
{
    public class MockProjectManagerEntities
    {
        private TestDbSet<DAC.User> _users = null;
        private TestDbSet<DAC.Project> _projects = null;
        private TestDbSet<DAC.Task> _tasks = null;
        private TestDbSet<DAC.ParentTask> _parentTasks = null;

        public TestDbSet<DAC.User> Users
        {
            get
            {
                return _users;
            }
            set
            {
                _users = value;
            }
        }

        public TestDbSet<DAC.Project> Projects
        {
            get
            {
                return _projects;
            }
            set
            {
                _projects = value;
            }
        }

        public TestDbSet<DAC.Task> Tasks
        {
            get
            {
                return _tasks;
            }
            set
            {
                _tasks = value;
            }
        }

        public TestDbSet<DAC.ParentTask> ParentTasks
        {
            get
            {
                return _parentTasks;
            }
            set
            {
                _parentTasks = value;
            }
        }
    }

}
