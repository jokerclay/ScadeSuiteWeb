using Microsoft.AspNetCore.Mvc;
using ScadeSuiteWeb.Server.Models.ProjectModel;
using ScadeSuiteWeb.Shared.Utils;
using ScadeSuiteWeb.Shared.ViewModels.PorjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;


using static Microsoft.FluentUI.AspNetCore.Components.Emojis.FoodDrink.Color.Default;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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


        [HttpGet(Name = "LoadModel")]
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
    }
}
