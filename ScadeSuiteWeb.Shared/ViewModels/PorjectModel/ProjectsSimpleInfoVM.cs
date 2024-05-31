namespace ScadeSuiteWeb.Shared.ViewModels.PorjectModel;

public class ProjectsSimpleInfoVM
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    
    public DateTime CreatedTime { get; set; } 
    
    public string ProjectFilePath { get; set; } = string.Empty;
    
    public bool Selected { get; set; }
}