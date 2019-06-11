using System.Collections.Generic;
using System.Linq;
using MODEL = PMServer.Models;

namespace PMServer.BC
{
    public class ProjectBC
    {
        DAC.ProjectManagerEntities dbContext = null;
        public ProjectBC()
        {
            dbContext = new DAC.ProjectManagerEntities();
        }

        public ProjectBC(DAC.ProjectManagerEntities context)
        {
            dbContext = context;
        }
        public virtual List<MODEL.Project> RetrieveProjects()
        {
            using (dbContext)
            {
                return dbContext.Projects.Select(x => new MODEL.Project()
                {
                    ProjectId = x.Project_ID,
                    ProjectName = x.Project1,
                    ProjectEndDate = x.End_Date,
                    ProjectStartDate = x.Start_Date,
                    Priority = x.Priority,
                    User = dbContext.Users.Where(y => y.Project_ID == x.Project_ID).Select(z => new MODEL.User()
                    {
                        UserId = z.User_ID
                    }).FirstOrDefault(),
                    NoOfTasks = dbContext.Tasks.Where(y => y.Project_ID == x.Project_ID).Count(),
                    NoOfCompletedTasks = dbContext.Tasks.Where(y => y.Project_ID == x.Project_ID && y.Status == 1).Count(),
                }).ToList();
            }
        }

        public virtual int InsertProjectDetails(MODEL.Project project)
        {
            using (dbContext)
            {
                DAC.Project proj = new DAC.Project()
                {
                    Project1 = project.ProjectName,
                    Start_Date = project.ProjectStartDate,
                    End_Date = project.ProjectEndDate,
                    Priority = project.Priority
                };
                dbContext.Projects.Add(proj);
                dbContext.SaveChanges();
                var editDetails = (from editUser in dbContext.Users
                                   where editUser.User_ID.ToString().Contains(project.User.UserId.ToString())
                                   select editUser).First();
                // Modify existing records
                if (editDetails != null)
                {
                    editDetails.Project_ID = proj.Project_ID;
                }
                return dbContext.SaveChanges();
            }
        }

        public virtual int UpdateProjectDetails(MODEL.Project project)
        {
            using (dbContext)
            {
                var editProjDetails = (from editProject in dbContext.Projects
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
                

                var editDetails = (from editUser in dbContext.Users
                                   where editUser.User_ID.ToString().Contains(project.User.UserId.ToString())
                                   select editUser).First();
                // Modify existing records
                if (editDetails != null)
                {
                    editDetails.Project_ID = project.ProjectId;
                }
                return dbContext.SaveChanges();
            }

        }
        public virtual int DeleteProjectDetails(MODEL.Project project)
        {
            using (dbContext)
            {
               
                var editDetails = (from proj in dbContext.Projects
                                   where proj.Project_ID==project.ProjectId
                                   select proj).First();
                // Delete existing record
                if (editDetails != null)
                {
                    dbContext.Projects.Remove(editDetails);
                }
                return dbContext.SaveChanges();
            }

        }

    }
}