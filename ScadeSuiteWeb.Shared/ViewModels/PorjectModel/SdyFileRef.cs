using System.Xml;
using Newtonsoft.Json.Linq;

public class SdyFileRef : SdyElement
{
    public string PersistAs { get; set; }
    public SdyFileRef()
    {
        Class = "FileRef";
        Id = -1;
        PersistAs = string.Empty;
    }
    public SdyFileRef(int id, string persistAs)
    {
        Class = "FileRef";
        Id = id;
        PersistAs = persistAs;
    }
    /// <summary>
    /// 解析FileRef的XML信息并转化为C#
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public override string FillDate(XmlNode node)
    {
        XmlAttributeCollection attrs;
        if (node == null)
        {
            Console.WriteLine(node!.Name + " 无数据");
            return node!.Name + " 无数据";
        }
        try
        {
            attrs = node.Attributes!;
            if (int.TryParse(attrs["id"]!.Value, out Id) == false)
            {
                Console.WriteLine("Folder 的 id 属性无法解析为 int 类型的数值");
                return "Folder 的 id 属性无法解析为 int 类型的数值";
            }

            PersistAs = attrs["persistAs"]!.Value.Trim();

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
    /// 解析FileRef信息
    /// </summary>
    /// <param name="jobject"></param>
    public SdyFileRef(JObject jobject)
    {
        Class = "FileRef";
        PersistAs = string.Empty;
        Id = 0;
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
                case "Props":
                    Props = SdyProp.LoadPropsFromJArray((JArray)job.Value!);
                    break;
                case "PersistAs":
                    PersistAs = (string)job.Value!;
                    break;
                default:
                    break;
            }
        }
    }
    /// <summary>
    /// 书写配置文件信息
    /// </summary>
    /// <param name="etpDoc"></param>
    /// <returns></returns>
    public override XmlElement ToXML(XmlDocument etpDoc)
    {
        XmlElement node = etpDoc.CreateElement("FileRef");
        try
        {
            node.SetAttribute("id", Id.ToString());

            PersistAs = Path.GetFileName(PersistAs);
            node.SetAttribute("persistAs", PersistAs);

            XmlElement temp;
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
        catch (Exception)
        {
            return null!;
        }

        return node;
    }
    /// <summary>
    /// 生成json
    /// </summary>
    /// <returns></returns>
    public override JObject ToJson()
    {
        var json = base.ToJson();

        json[nameof(Id)] = Id;
        json[nameof(PersistAs)] = PersistAs;
        json[nameof(Class)] = Class;
        return json;
    }
}