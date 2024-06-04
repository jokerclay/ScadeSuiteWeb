namespace ScadeSuiteWeb.Shared.ViewModels.PorjectModel;

public class CreateFileVm
{
    public int ProjectId { get; set; }
    public List<string> FileNames { get; set; } 
    public SdyElement? SelectedElement { get; set; } 
}