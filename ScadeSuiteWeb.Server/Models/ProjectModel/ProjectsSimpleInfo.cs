namespace ScadeSuiteWeb.Server.Models.ProjectModel;

public class ProjectsSimpleInfo
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    
    public DateTime CreatedTime { get; set; } 
    
    public string ProjectFilePath { get; set; } = string.Empty;
}