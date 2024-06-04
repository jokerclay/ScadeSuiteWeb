namespace ScadeSuiteWeb.Shared.ViewModels.PorjectModel;

public class CreateFolderVm
{
    public int ProjectId { get; set; }
    public string FolderName { get; set; } =string.Empty;
    public string FolderExtension { get; set; } = string.Empty;
    public SdyElement? SelectedElement { get; set; } 
}
