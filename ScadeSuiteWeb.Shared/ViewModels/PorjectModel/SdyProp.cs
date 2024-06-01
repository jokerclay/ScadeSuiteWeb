using System.Xml;
using Newtonsoft.Json.Linq;

/// <summary>
/// 定义项目中的 配置相关信息
/// </summary>
public class SdyProp : ModelObject
{
    public List<string> Values { get; set; }
    public int Configuration;
    /// <summary>
    /// 初始化
    /// </summary>
    public SdyProp()
    {
        Class = "Prop";
        Name = string.Empty;
        Id = 0;
        Configuration = 0;
        Values = new List<string>();
    }
    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="values"></param>
    /// <param name="configuration"></param>
    public SdyProp(int id, string name, List<string> values, int configuration = -1)
    {
        Class = "Prop";
        Id = id;
        Name = name;
        Values = values;
        Configuration = configuration;
    }

    /// <summary>
    /// 解析props的XML信息
    /// </summary>
    /// <param name="propsNode"></param>
    /// <returns></returns>
    public static List<SdyProp> LoadPropsXmlNode(XmlNode propsNode)
    {
        List<SdyProp> props = new();
        if (propsNode == null)
        {
            Console.WriteLine(propsNode!.Name + " 无数据");
            return props;
        }
        if (propsNode.Name != "props")
        {
            Console.WriteLine("未知的XmlNode 的 Name ,期望为 props,而实际为 " + propsNode.Name);
            return props;
        }
        foreach (XmlNode n in propsNode.ChildNodes)
        {
            SdyProp prop = new();
            var result = prop.FillDate(n);
            if (result == "")
            {
                props.Add(prop);
            }

        }
        return props;
    }
    /// <summary>
    /// 获取Props里的子信息Prop节点
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public string FillDate(XmlNode node)
    {
        XmlAttributeCollection attrs;

        if (node == null)
        {
            Console.WriteLine(node + " 无数据");
            return node + " 无数据";
        }
        try
        {
            if (node.Name != "Prop")
            {
                Console.WriteLine("期望XmlNode的名称为 Prop,但实际为 " + node.Name);
                return "期望XmlNode的名称为 Prop,但实际为 " + node.Name;
            }
            attrs = node.Attributes!;
            if (int.TryParse(attrs["id"]!.Value, out Id) == false)
            {
                Console.WriteLine("Prop 的 id 属性无法解析为 int 类型的数值");
                return "Prop 的 id 属性无法解析为 int 类型的数值";
            }
            Name = attrs["name"]!.Value.Trim();
            foreach (XmlNode n in node.ChildNodes)
            {
                if (n.Name == "value")
                {
                    Values.Add(n.InnerText.Trim());
                }
                else if (n.Name == "configuration")
                {
                    if (int.TryParse(n.InnerText, out Configuration) == false)
                    {
                        Console.WriteLine("Prop 的 configuration 标签值无法解析为 int 类型的数值");
                        return "Prop 的 configuration 标签值无法解析为 int 类型的数值";
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("加载 Prop 数据失败：\n" + ex.Message);
            return "加载 Prop 数据失败：\n" + ex.Message;
        }
        return "";
    }

    /// <summary>
    /// 将json串转化为C#
    /// </summary>
    /// <param name="jArray"></param>
    /// <returns></returns>
    public static List<SdyProp> LoadPropsFromJArray(JArray jArray)
    {
        List<SdyProp> props = new();
        foreach (JObject job in jArray)
        {
            SdyProp prop = new(job);
            if (prop.Id == -1)
            {
                continue;
            }
            props.Add(prop);
        }
        return props;
    }
    /// <summary>
    /// 解析Json文件
    /// </summary>
    /// <param name="jobject"></param>
    public SdyProp(JObject jobject)
    {
        Class = "Prop";
        Name = string.Empty;
        Id = -1;
        Configuration = -1;
        Values = new List<string>();
        foreach (var job in jobject)
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
                case "Configuration":
                    Configuration = (int)job.Value!;
                    break;
                case "Values":
                    Values = job.Value!.ToObject<List<string>>()!;
                    break;
                default:
                    break;
            }
        }
    }
    /// <summary>
    /// 将Props的信息写入etp的XML文件中
    /// </summary>
    /// <param name="etpDoc"></param>
    /// <returns></returns>
    public XmlElement ToXML(XmlDocument etpDoc)
    {
        XmlElement node = etpDoc.CreateElement("Prop");
        try
        {
            node.SetAttribute("id", Id.ToString());
            node.SetAttribute("name", Name);
            //元素里包含下一级元素
            XmlElement temp;
            //Values在SdyProp的重载方法中已经获得
            foreach (string value in Values)
            {
                temp = etpDoc.CreateElement("value");
                temp.InnerText = value;
                //加入Prop元素的下一层集中
                node.AppendChild(temp);
            }
            if (Configuration > 0)
            {
                temp = etpDoc.CreateElement("configuration");
                temp.InnerText = Configuration.ToString();
                node.AppendChild(temp);
            }
        }
        catch
        {
            return node;
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
        json[nameof(Id)] = Id;
        json[nameof(Name)] = Name;
        json[nameof(Class)] = Class;
        json[nameof(Configuration)] = Configuration;
        if (Values.Count > 0)
        {
            var jarray = new JArray();
            foreach (var item in Values)
            {
                jarray.Add(item);
            }
            json[nameof(Values)] = jarray;
        }

        return json;
    }
}