using System.Xml;
using Newtonsoft.Json.Linq;

/// <summary>
/// 定义 项目中的 Folder 和 FileRef 类的信息
/// </summary>
public class SdyElement : ModelObject
{
    public List<SdyProp> Props { get; set; }
    
    public bool Actived { get; set; } = false;

    public SdyElement()
    {
        Class = string.Empty;
        Name = string.Empty;
        Id = -1;
        Props = new List<SdyProp>();
    }

    /// <summary>
    /// 解析roots里的XML信息
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static List<SdyElement> LoadRootsXmlNode(XmlNode node)
    {
        List<SdyElement> roots = new();
        if (node == null)
        {
            Console.WriteLine(node + " 无数据");
            return roots;
        }
        foreach (XmlNode childNode in node.ChildNodes)
        {
            if (childNode.Name == "Folder")
            {
                SdyFolder folder = new();
                var result = folder.FillDate(childNode);
                if (result == "")
                {
                    roots.Add(folder);
                }

            }
            else if (childNode.Name == "FileRef")
            {
                SdyFileRef fileRef = new();
                var result = fileRef.FillDate(childNode);
                if (result == "")
                {
                    roots.Add(fileRef);
                }

            }
        }
        return roots;
    }
    /// <summary>
    /// 解析roots里的节点信息
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public virtual string FillDate(XmlNode node)
    {
        XmlAttributeCollection attrs;
        if (node == null)
        {
            Console.WriteLine(node?.Name + " 无数据");
            return node + " 无数据";
        }
        if (node.Name != "Folder" || node.Name != "FileRef")
        {
            Console.WriteLine("无效数据（加载的XmlNode 的名称不是 Folder 或 FileRef）");
            return "无效数据（加载的XmlNode 的名称不是 Folder 或 FileRef）";
        }
        try
        {
            attrs = node.Attributes!;

            if (int.TryParse(attrs["id"]!.Value, out Id) == false)
            {
                Console.WriteLine("Folder 的 id 属性无法解析为 int 类型的数值");
                return "Folder 的 id 属性无法解析为 int 类型的数值";
            }
            Name = attrs["name"]!.Value.Trim();
            if (node.HasChildNodes)
            {
                foreach (XmlNode tempNode in node.ChildNodes)
                {
                    if (tempNode.Name == "props")
                    {
                        Props ??= new List<SdyProp>();
                        foreach (XmlNode childNode in tempNode.ChildNodes)
                        {
                            if (childNode.Name == "Prop")
                            {
                                SdyProp prop = new();
                                prop.FillDate(childNode);
                                Props.Add(prop);
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("加载 " + node.Name + " 数据失败：\n" + ex.Message);
            return "加载 " + node.Name + " 数据失败：\n" + ex.Message;
        }
        return "";
    }

    public SdyElement(JObject jobject)
    {
        Class = string.Empty;
        Name = string.Empty;
        Id = -1;
        Props = new List<SdyProp>();
        foreach (var job in jobject)
        {
            switch (job.Key)
            {
                case "Name":
                    Name = (string)job.Value!;
                    break;
                case "Id":
                    Id = (int)job.Value!;
                    break;
                case "Props":
                    Props = SdyProp.LoadPropsFromJArray((JArray)job.Value!);
                    break;
                default:
                    break;
            }
        }

    }

    public virtual XmlElement ToXML(XmlDocument etpDoc)
    {
        XmlElement element = etpDoc.CreateElement("Element");
        return element;
    }
    /// <summary>
    /// 将JSON对象转换为C#
    /// </summary>
    /// <param name="jArray"></param>
    /// <returns></returns>
    public static List<SdyElement> LoadElementsFromJArray(JArray jArray)
    {
        List<SdyElement> elements = new();
        foreach (JObject jObject in jArray)
        {
            if (jObject["Class"] != null)
            {
                if ((string)jObject["Class"]! == "Folder")
                {
                    SdyFolder folder = new(jObject);
                    if (folder.Id == -1)
                    {
                        continue;
                    }
                    elements.Add(folder);
                }
                else if ((string)jObject["Class"]! == "FileRef")
                {
                    SdyFileRef fileRef = new(jObject);
                    if (fileRef.Id == -1)
                    {
                        continue;
                    }
                    elements.Add(fileRef);
                }
            }
        }
        return elements;
    }
    /// <summary>
    /// 生成Json
    /// </summary>
    /// <returns></returns>
    public override JObject ToJson()
    {
        var json = base.ToJson();

        if (Props.Count > 0)
        {
            JArray jarray = new();
            foreach (var item in Props)
            {
                jarray.Add(item.ToJson());
            }
            json[nameof(Props)] = jarray;
        }

        return json;
    }
}