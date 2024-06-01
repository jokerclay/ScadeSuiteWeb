using System.Xml;
using Newtonsoft.Json.Linq;

/// <summary>
/// 定义 项目中的 Configurations 类的信息
/// </summary>
public class Configurations : ModelObject
{
    public Configurations()
    {
        Class = "Configuration";
        Id = 0;
        Name = string.Empty;
    }
    public Configurations(int id, string name)
    {
        Class = "Configuration";
        Id = id;
        Name = name;
    }
    /// <summary>
    /// 解析Configurations的XML信息
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static List<Configurations> LoadConfigurationsXmlNode(XmlNode node)
    {
        List<Configurations> configs = new();
        if (node == null)
        {
            Console.WriteLine(node!.Name + " 无数据");
            return configs;
        }
        if (node.Name != "configurations")
        {
            Console.WriteLine("未知的XmlNode 的 Name ,期望为 configurations,而实际为 " + node.Name);
            return configs;
        }
        foreach (XmlNode childNode in node.ChildNodes)
        {
            Configurations config = new Configurations();
            var result = config.FillDate(childNode);
            if (result == "")
            {
                configs.Add(config);
            }

        }
        return configs;
    }
    /// <summary>
    /// 解析Configurations里的节点信息
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public string FillDate(XmlNode node)
    {
        if (node == null)
        {
            Console.WriteLine(node!.Name + " 无数据");
            return node!.Name + " 无数据";
        }
        if (node.Name != "Configuration")
        {
            Console.WriteLine("未知的 XmlNode 节点名称：" + node.Name);
            return "";
        }
        XmlAttributeCollection attrs;
        try
        {
            attrs = node.Attributes!;
            if (int.TryParse(attrs["id"]!.Value, out Id) == false)
            {
                Console.WriteLine("Configuration 的 id 属性无法解析为 int 类型的数值");
                return "Configuration 的 id 属性无法解析为 int 类型的数值";
            }
            Name = attrs["name"]!.Value.Trim();
        }
        catch (Exception ex)
        {
            Console.WriteLine("加载 " + node.Name + " 数据失败：\n" + ex.Message);
            return "加载 " + node.Name + " 数据失败：\n" + ex.Message;
        }
        return "";
    }
    /// <summary>
    /// 解析Json为数据结构
    /// </summary>
    /// <param name="jObject"></param>
    public Configurations(JObject jObject)
    {
        Class = "Configuration";
        Id = 0;
        Name = string.Empty;
        foreach (var job in jObject)
        {
            switch (job.Key)
            {
                case "Class":
                    Class = (string)job.Value!;
                    break;
                case "Name":
                    Name = (string)job.Value!;
                    break;
                case "Id":
                    Id = (int)job.Value!;
                    break;
                default:
                    break;
            }
        }
    }
    /// <summary>
    /// JSON对象转换为C#
    /// </summary>
    /// <param name="jArray"></param>
    /// <returns></returns>
    public static List<Configurations> LoadConfigurationsFromJArray(JArray jArray)
    {
        List<Configurations> configurations = new List<Configurations>();
        foreach (JObject jObject in jArray)
        {
            Configurations configuration = new Configurations(jObject);
            if (configuration.Id == -1)
            {
                continue;
            }
            configurations.Add(configuration);
        }
        return configurations;
    }
    /// <summary>
    /// 书写ConfigurationXML信息
    /// </summary>
    /// <param name="etpDoc"></param>
    /// <returns></returns>
    public XmlElement ToXML(XmlDocument etpDoc)
    {
        XmlElement node = etpDoc.CreateElement("Configuration");
        try
        {
            node.SetAttribute("id", Id.ToString());
            node.SetAttribute("name", Name);
        }
        catch
        {
            return null!;
        }
        return node;
    }
    /// <summary>
    /// 生成Json
    /// </summary>
    /// <returns></returns>
    public override JObject ToJson()
    {
        var json = base.ToJson();
        json[nameof(Name)] = Name;
        json[nameof(Id)] = Id;
        json[nameof(Class)] = Class;
        return json;
    }
}