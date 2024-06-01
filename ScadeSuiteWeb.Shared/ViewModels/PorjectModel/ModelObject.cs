using Newtonsoft.Json.Linq;

public class ModelObject
{
    public int Id;
    public string Name { get; set; } = string.Empty;
    public string Class { get; set; } = string.Empty;
    public virtual JObject ToJson()
    {
        return new JObject();
    }
}