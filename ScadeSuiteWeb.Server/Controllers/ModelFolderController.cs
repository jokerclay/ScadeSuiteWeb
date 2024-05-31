using Microsoft.AspNetCore.Mvc;
using ScadeSuiteWeb.Server.Models.ProjectModel;
using ScadeSuiteWeb.Shared.Utils;
using ScadeSuiteWeb.Shared.ViewModels.PorjectModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ScadeSuiteWeb.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelFolderController : ControllerBase 
    {
        
        [HttpGet]
        [Route("GetModelFolderInfo")]
        public async    Task<ResponResult<IEnumerable<ProjectsSimpleInfoVM>>> GetModelFolderInfo()
        {
            List<ProjectsSimpleInfo> projectsSimpleInfos = new List<ProjectsSimpleInfo>();
            if (projectsSimpleInfos == null) throw new ArgumentNullException(nameof(projectsSimpleInfos));

            var resourcesFilePath = CommonHelper.SystemResourcesFilePath;
            var modelFilePath = Path.Combine(resourcesFilePath , "SuiteModels");
            
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
        
        
        
        
        
        
        /*
         
        [HttpGet(Name = "LoadModel")]
        public SdyProject LoadModel(string filePath)
        {
        string filePath = Path.Combine(currentDirectory, "ABC_N", "ABC_N.etp");
        SdyProject pj = new SdyProject();
            if (pj.LoadProjectFile(filePath))
        {
            //var options = new JsonSerializerOptions { WriteIndented = true };

            var projectJsonString = JsonConvert.SerializeObject(pj);
    
            // Print the JSON string
            // Console.WriteLine(projectJsonString);
   
            // Define the path to save the JSON file
            // string outputFilePath = Path.Combine(Directory.GetCurrentDirectory(), "project.json");

            // Save the JSON string to a file
            // File.WriteAllText(outputFilePath , projectJsonString );

            // Optionally, print a message to confirm saving
            // Console.WriteLine($"JSON file saved to: {outputFilePath }");
            return pj;
        }
            return pj;
        }
        */
        
    }
}
