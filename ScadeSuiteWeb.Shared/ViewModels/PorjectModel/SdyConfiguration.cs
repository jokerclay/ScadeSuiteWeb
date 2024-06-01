using Newtonsoft.Json.Linq;

/// <summary>
/// 项目结构信息
/// </summary>
public class SdyConfiguration
{
    public int Id;
    public int Oid_count;
    public int DefaultConfiguration;
    public List<SdyElement> Roots { get; set; }
    public List<SdyProp> Props { get; set; }
    public List<Configurations> Configuration { get; set; }
    /// <summary>
    /// 初始化
    /// </summary>
    public SdyConfiguration()
    {
        Oid_count = 0;
        DefaultConfiguration = 0;
        Roots = new List<SdyElement>();
        Configuration = new List<Configurations>();
        Props = new List<SdyProp>();
    }
    /// <summary>
    /// 解析项目信息
    /// </summary>
    /// <param name="jobject"></param>
    public SdyConfiguration(JObject jobject)
    {
        Oid_count = 0;
        DefaultConfiguration = 0;
        Roots = new List<SdyElement>();
        Configuration = new List<Configurations>();
        Props = new List<SdyProp>();
        foreach (var job in jobject)
        {
            switch (job.Key)
            {
                case "Id":
                    Id = (int)job.Value!;
                    break;
                case "Oid_count":
                    Oid_count = (int)job.Value!;
                    break;
                case "DefaultConfiguration":
                    DefaultConfiguration = (int)job.Value!;
                    break;
                case "Roots":
                    Roots = SdyElement.LoadElementsFromJArray((JArray)job.Value!);
                    break;
                case "Configuration":
                    Configuration = Configurations.LoadConfigurationsFromJArray((JArray)job.Value!);
                    break;
                case "Props":
                    Props = SdyProp.LoadPropsFromJArray((JArray)job.Value!);
                    break;
                default:
                    break;
            }
        }
    }
    /// <summary>
    /// 解析项目节点信息
    /// </summary>
    /// <param name="jobject"></param>
    /// <returns></returns>
    public static SdyConfiguration LoadFromJarry(JObject jobject)
    {
        SdyConfiguration xuanConfiguration = new();
        xuanConfiguration = new SdyConfiguration(jobject);
        return xuanConfiguration;
    }
    /// <summary>
    /// 生成Json
    /// </summary>
    /// <returns></returns>
    public JObject ToJson()
    {
        JObject json = new()
        {
            { nameof(Id), new JValue(Id)},
            { nameof(Oid_count), new JValue(Oid_count)},
            { nameof(DefaultConfiguration), new JValue(DefaultConfiguration) }

        };
        JArray array = new();
        foreach (SdyElement item in Roots)
        {
            array.Add(item.ToJson());
        }
        JArray array1 = new();
        foreach (SdyProp item in Props)
        {
            array1.Add(item.ToJson());
        }
        JArray array2 = new();
        foreach (Configurations item in Configuration)
        {
            array2.Add(item.ToJson());
        }
        json.Add(nameof(Roots), array);
        json.Add(nameof(Props), array1);
        json.Add(nameof(Configuration), array2);
        return json;
    }
}