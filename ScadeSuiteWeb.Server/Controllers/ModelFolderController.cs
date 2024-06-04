using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FluentUI.AspNetCore.Components;
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
        public ResponResult<CreateFolderVm> CreateNewFolder( [FromBody] CreateFolderVm createFolderVm)
        {
            string filePath = "";
            var selectedElement = createFolderVm.SelectedElement;

            // find the project that are currently editing
            var resourcesFilePath = CommonHelper.SystemResourcesFilePath;
            var modelFilePath = Path.Combine(resourcesFilePath, "SuiteModels");
            string[] subfolders = Directory.GetDirectories(modelFilePath);

            var newfolder = new SdyFolder();
            SdyProject pj = new SdyProject();

            for (int i = 0; i < subfolders.Length; ++i)
            {
                if (i == createFolderVm.ProjectId)
                {
                    filePath = subfolders[i] + "\\ABC_N.etp";

                    if (pj.LoadProjectFile(filePath))
                    {
                        if (selectedElement?.Class=="Folder")
                        {
                            if (selectedElement.Name == "Root")
                            {
                                newfolder = new SdyFolder()
                                {
                                    Id = selectedElement.Id+1,
                                    Name = createFolderVm.FolderName,
                                    Extensions = createFolderVm.FolderExtension,
                                };
                                pj.Project.Roots.Add(newfolder);
                            }
                            else
                            {
                                var folder = GetFolder(selectedElement, pj);
                            
                                newfolder = new SdyFolder()
                                {
                                    Id = folder.Id+1,
                                    Name = createFolderVm.FolderName,
                                    Extensions = createFolderVm.FolderExtension,
                                };
                            
                                folder.Elements.Add(newfolder);
                                
                            }
                        }
                        
                        if (selectedElement?.Class == "FileRef")
                        {
                            var folder = GetFolder(selectedElement, pj);
                            Console.WriteLine(folder);
                        }
                        
                        var doc = pj.ToXML();
                        doc.Save(filePath);
                        var data = new CreateFolderVm();
                        
                        data.SelectedElement = newfolder;
                        return new ResponResult<CreateFolderVm>
                        {
                            Success = true,
                            Data = data
                        };
                    }
                    else
                    {
                        return new ResponResult<CreateFolderVm>
                        {
                            Success = false,
                            Message = "ERROR: Failed to load project file."
                        };
                    }
                    
                }
            }
            return new ResponResult<CreateFolderVm>
            {
                Success = false,
            };
        }

        
        private SdyFolder GetFolder(SdyElement selectedElement, SdyProject pj)
        {
            
            return FindFolderRecursive(pj.Project.Roots, selectedElement);
        }
        
        private SdyFolder FindFolderRecursive(List<SdyElement> folders, SdyElement selectedElement)
        {
            foreach (var folder in folders)
            {
                if (folder is SdyFolder sdyFolder)
                {
                    // Check if the current folder matches the selected element
                    if (sdyFolder.Name == selectedElement.Name)
                    {
                        return sdyFolder;
                    }

                    // Recursively search in the subfolders
                    var result = FindFolderRecursive(sdyFolder.Elements, selectedElement);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            // If no matching folder is found, return null
            return null;
        }
        
        


        [HttpPost]
        [Route("CreateNewFile")]
        public async Task<ResponResult<CreateFileVm>> CreateNewFile(CreateFileVm createFileVm )
        {
            string filePath = "";
            var selectedElement = createFileVm.SelectedElement;

            // find the project that are currently editing
            var resourcesFilePath = CommonHelper.SystemResourcesFilePath;
            var modelFilePath = Path.Combine(resourcesFilePath, "SuiteModels");
            string[] subfolders = Directory.GetDirectories(modelFilePath);

            SdyProject pj = new SdyProject();


            for (int i = 0; i < subfolders.Length; ++i)
            {
                if (i == createFileVm.ProjectId)
                {
                    filePath = subfolders[i] + "\\ABC_N.etp";

                    if (pj.LoadProjectFile(filePath))
                    {
                        if (selectedElement?.Class == "Folder")
                        {
                            var folder = GetFolder(selectedElement, pj);
                            
                            foreach (var f in createFileVm.FileNames)
                            {
                                folder.Elements.Add(new SdyFileRef()
                                {
                                    Id = 69,
                                    PersistAs = "../"+f,
                                });
                            }
                            var doc = pj.ToXML();
                            doc.Save(filePath);
                            return new ResponResult<CreateFileVm>
                            {
                                Success = true,
                                Message = "ERROR: Failed to load project file."
                            };
                        }
                    }
                    else
                    {
                        return new ResponResult<CreateFileVm>
                        {
                            Success = false,
                            Message = "ERROR: Failed to load project file."
                        };
                    }

                }
            }

            /*
            selectedFolder.Elements.Add(new SdyFileRef()
            {
                Id = 69,
                PersistAs = "hahagetyou"
            });

            */
            
            return new ResponResult<CreateFileVm>
            {
                Success = false,
            };
        }

    }
}
