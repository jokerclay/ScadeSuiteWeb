using Microsoft.AspNetCore.Mvc;
using ScadeSuiteWeb.Server.Models.ProjectModel;
using ScadeSuiteWeb.Shared.Utils;
using ScadeSuiteWeb.Shared.ViewModels.PorjectModel;
using Newtonsoft.Json;

namespace ScadeSuiteWeb.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelFolderController : ControllerBase
    {
        [HttpGet]
        [Route("GetModelFolderInfo")]
        public async Task<ResponResult<IEnumerable<ProjectsSimpleInfoVM>>> GetModelFolderInfo()
        {
            List<ProjectsSimpleInfo> projectsSimpleInfos = new List<ProjectsSimpleInfo>();
            if (projectsSimpleInfos == null) throw new ArgumentNullException(nameof(projectsSimpleInfos));

            var resourcesFilePath = CommonHelper.SystemResourcesFilePath;
            var modelFilePath = Path.Combine(resourcesFilePath, "SuiteModels");

            string[] subfolders = Directory.GetDirectories(modelFilePath);

            for (int i = 0; i < subfolders.Length; ++i)
            {
                DirectoryInfo di = new DirectoryInfo(subfolders[i]);
                projectsSimpleInfos.Add(new ProjectsSimpleInfo()
                {
                    Id = i,
                    Name = di.Name,
                    CreatedTime = Directory.GetCreationTime(subfolders[i]),
                    ProjectFilePath = subfolders[i]
                });
            }

            var projectsSimpleInfosVm = new List<ProjectsSimpleInfoVM>();
            foreach (var project in projectsSimpleInfos)
            {
                projectsSimpleInfosVm.Add(new ProjectsSimpleInfoVM()
                {
                    Id = project.Id,
                    Name = project.Name,
                    CreatedTime = project.CreatedTime,
                    ProjectFilePath = project.ProjectFilePath
                });
            }

            return new ResponResult<IEnumerable<ProjectsSimpleInfoVM>>
            {
                Success = true,
                Data = projectsSimpleInfosVm
            };
        }


        [HttpGet]
        [Route("LoadModel")]
        public async Task<ResponResult<string>> LoadModel(int id)
        {
            var resourcesFilePath = CommonHelper.SystemResourcesFilePath;
            var modelFilePath = Path.Combine(resourcesFilePath, "SuiteModels");
            string[] subfolders = Directory.GetDirectories(modelFilePath);

            SdyProject pj = new SdyProject();

            for (int i = 0; i < subfolders.Length; ++i)
            {
                if (i == id)
                {
                    var filePath = subfolders[i] + "\\ABC_N.etp";
                    if (pj.LoadProjectFile(filePath))
                    {
                        var projectJsonString = JsonConvert.SerializeObject(pj);

                        return new ResponResult<string>
                        {
                            Success = true,
                            Data = projectJsonString
                        };
                    }
                }
            }
            return new ResponResult<string>
            {
                Success = false,
            };
        }

        [HttpPost]
        [Route("CreateNewFolder")]
        public ResponResult<bool> CreateNewFolder(string newFolderName, string newExtensions, int projectId)
        {
            string filePath = "";
            SdyFolder newFolder = new SdyFolder()
            {
                Id = 69,
                Name = newFolderName,
                Extensions = newExtensions,
            };

            // find the project that are currently editing
            var resourcesFilePath = CommonHelper.SystemResourcesFilePath;
            var modelFilePath = Path.Combine(resourcesFilePath, "SuiteModels");
            string[] subfolders = Directory.GetDirectories(modelFilePath);

            SdyProject pj = new SdyProject();

            for (int i = 0; i < subfolders.Length; ++i)
            {
                if (i == projectId)
                {
                    filePath = subfolders[i] + "\\ABC_N.etp";
                    if (pj.LoadProjectFile(filePath))
                    {
                        // add a new folder to the root of the project
                        var selectedFolder = pj.Project.Roots;
                        selectedFolder.Add(newFolder);
                        
                        // save the project back to xml file
                        var doc = pj.ToXML();
                        doc.Save(filePath);
                        return new ResponResult<bool>
                        {
                            Success = true,
                            Message = "New Folder Created."
                        };
                    }
                    else
                    {
                        return new ResponResult<bool>
                        {
                            Success = false,
                            Message = "ERROR: Could not Parse the Project File."
                        };
                    }
                }
            }
            
            return new ResponResult<bool>
            {
                Success = false,
            };
        }


        [HttpPost]
        [Route("CreateNewFile")]
        public async Task<ResponResult<bool>> CreateNewFile( SdyFolder selectedFolder, int projectId)
        {
            
            
            selectedFolder.Elements.Add(new SdyFileRef()
            {
                Id = 69,
                PersistAs = "hahagetyou"
            });
            
            return new ResponResult<bool>
            {
                Success = false,
            };
        }

    }
}
