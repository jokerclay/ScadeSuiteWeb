using System.Xml;
using Newtonsoft.Json.Linq;

public class SdyFolder : SdyElement
{
    public string Extensions { get; set; }
    public List<SdyElement> Elements { get; set; }
    public SdyFolder()
    {
        Class = "Folder";
        Name = string.Empty;
        Id = -1;
        Elements = new List<SdyElement>();
        Extensions = string.Empty;
    }
    public SdyFolder(int id, string extensions, string name, List<SdyElement>? elements = null)
    {
        Class = "Folder";
        Id = id;
        Name = name;
        Extensions = extensions;
        if (elements == null)
        {
            Elements = new List<SdyElement>();
        }
        else
        {
            Elements = elements;
        }
    }
    /// <summary>
    /// 解析Folder的XML信息并转化为C#
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public override string FillDate(XmlNode node)
    {
        XmlAttributeCollection attrs;
        if (node == null)
        {
            Console.WriteLine(node!.Name + "无数据");
            return node!.Name + "无数据";
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
            if (attrs["extensions"] != null)
            {
                Extensions = attrs["extensions"]!.Value.Trim();
            }

            if (node.HasChildNodes)
            {
                foreach (XmlNode tempNode in node.ChildNodes)
                {
                    if (tempNode.Name == "elements")
                    {
                        Elements = SdyElement.LoadRootsXmlNode(tempNode);

                    }
                    else if (tempNode.Name == "props")
                    {
                        Props ??= new List<SdyProp>();
                        foreach (XmlNode childNode in tempNode.ChildNodes)
                        {
                            if (childNode.Name == "Prop")
                            {
                                SdyProp prop = new();
                                var result = prop.FillDate(childNode);
                                if (result == "")
                                {
                                    Props.Add(prop);
                                }

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

    /// <summary>
    /// 解析为数据结构
    /// </summary>
    /// <param name="jobject"></param>
    public SdyFolder(JObject jobject)
    {
        Class = "Folder";
        Name = string.Empty;
        Id = -1;
        Elements = new List<SdyElement>();
        Extensions = string.Empty;
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
                case "Extensions":
                    Extensions = (string)job.Value!;
                    break;
                case "Elements":
                    Elements = SdyElement.LoadElementsFromJArray((JArray)job.Value!);
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
    /// 书写XML信息
    /// </summary>
    /// <param name="etpDoc"></param>
    /// <returns></returns>
    public override XmlElement ToXML(XmlDocument etpDoc)
    {
        XmlElement node = etpDoc.CreateElement("Folder");
        try
        {
            node.SetAttribute("id", Id.ToString());
            node.SetAttribute("name", Name);
            if (Extensions != "")
            {
                node.SetAttribute("extensions", Extensions);
            }

            XmlElement temp;
            if (Elements != null && Elements.Count > 0)
            {
                XmlElement elements = etpDoc.CreateElement("elements");
                foreach (SdyElement elem in Elements)
                {
                    temp = elem.ToXML(etpDoc);
                    if (temp != null)
                    {
                        elements.AppendChild(temp);
                    }
                }
                node.AppendChild(elements);
            }
            if (Props != null && Props.Count > 0)
            {
                XmlElement props = etpDoc.CreateElement("props");
                foreach (SdyProp prop in Props)
                {
                    temp = prop.ToXML(etpDoc);
                    if (temp != null)
                    {
                        props.AppendChild(temp);
                    }
                }
                node.AppendChild(props);
            }
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
        json[nameof(Id)] = Id;
        json[nameof(Name)] = Name;
        json[nameof(Class)] = Class;
        if (Elements.Count > 0)
        {
            var jarry = new JArray();
            foreach (var item in Elements)
            {
                jarry.Add(item.ToJson());
            }
            json[nameof(Elements)] = jarry;
        }
        json[nameof(Extensions)] = Extensions;
        return json;
    }
}