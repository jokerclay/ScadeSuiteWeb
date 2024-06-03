using System.Xml;
using Newtonsoft.Json.Linq;

/// <summary>
/// 项目结构
/// </summary>
public class SdyProject : ModelObject
{
    /// <summary>
    /// Etp文件路径
    /// </summary>
    public string EtpFilePath;

    /// <summary>
    /// 存储Roots、Props和Configurations等配置文件
    /// </summary>
    public SdyConfiguration Project { get; set; } = new();
    /// <summary>
    /// 初始化
    /// </summary>
    public SdyProject()
    {
        Class = "Project";
        Name = string.Empty;
        Id = 0;
        Project = new();
        EtpFilePath = string.Empty;
    }
    /// <summary>
    /// 加载Suite的.etp文件
    /// </summary>
    /// <param name="etpFile"></param>
    /// <param name="number"></param>
    /// <returns></returns>
    public bool LoadProjectFile(string etpFile)
    {
        if (File.Exists(etpFile) == false)
        {
            Console.WriteLine("指定的文件“" + etpFile + "”不存在");
            return false;
        }
        //实例化XML类
        XmlDocument _etpData = new();
        try
        {
            //加载XML文件并获取内容
            _etpData.Load(etpFile);
        }
        catch (Exception ex)
        {
            Console.WriteLine("加载项目文件出错：" + ex.Message);
            return false;
        }

        //获取加载的XML文件路径
        EtpFilePath = etpFile;
        //获取etp的文件名
        Name = Path.GetFileNameWithoutExtension(etpFile);

        //获取XML文档的根节点，通过根节点获取其相应的元素信息
        var result = FillDate(_etpData.SelectSingleNode("Project")!);
        if (result != "")
        {
            Console.WriteLine(result);
            return false;
        }

        return true;
    }
    /// <summary>
    /// 解析Suite的etp XML文件
    /// </summary>
    /// <param name="node"></param>
    /// <param name="number"></param>
    /// <returns></returns>
    public string FillDate(XmlNode node)
    {
        if (node == null)
        {
            Console.WriteLine(node + " 无数据");
            return node + " 无数据";
        }
        if (node.Name != "Project")
        {
            Console.WriteLine($"未知的 XmlNode 节点名称：\" + {node.Name}");
            return $"未知的 XmlNode 节点名称：\" + {node.Name}";
        }
        //通过名称或索引访问XML的属性集合
        XmlAttributeCollection attrs;
        try
        {
            //获取根节点的属性
            attrs = node.Attributes!;

            /*xxx.TryParse(str1,out num1) 
            功能：将str1转化成xxx类型，若转化成功，将值赋给num1，并返回true; 
            若转化失败，返回false。*/
            if (int.TryParse(attrs["id"]!.Value, out Id) == false)
            {
                Console.WriteLine("Project 的 id 属性无法解析为 int 类型的数值");
                return "Project 的 id 属性无法解析为 int 类型的数值";
            }
            Project.Id = Id;
            if (int.TryParse(attrs["oid_count"]!.Value, out Project.Oid_count) == false)
            {
                Console.WriteLine("Project 的 oid_count 属性无法解析为 int 类型的数值");
                return "Project 的 oid_count 属性无法解析为 int 类型的数值";
            }
            if (attrs["defaultConfiguration"] != null)
            {
                if (int.TryParse(attrs["defaultConfiguration"]!.Value, out Project.DefaultConfiguration) == false)
                {
                    Console.WriteLine("Project 的 defaultConfiguration 属性无法解析为 int 类型的数值");
                    return "Project 的 defaultConfiguration 属性无法解析为 int 类型的数值";
                }
            }

            //遍历获取的根节点里的子节点
            foreach (XmlNode childNode in node.ChildNodes)
            {
                //这里子节点已经明确的，因此直接判断子节点是否存在
                if (childNode.Name == "props")
                {
                    //解析子节点props的XML信息
                    Project.Props = SdyProp.LoadPropsXmlNode(childNode);

                }
                else if (childNode.Name == "roots")
                {
                    //解析子节点roots的XML信息
                    Project.Roots = SdyElement.LoadRootsXmlNode(childNode);

                    foreach (SdyElement root in Project.Roots)
                    {
                        LoadFileRef(root);
                    }
                }
                else if (childNode.Name == "configurations")
                {
                    //项目中的 Configuration 类的信息
                    Project.Configuration = Configurations.LoadConfigurationsXmlNode(childNode);
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
    /// 解析节点roots里的子节点的XML信息
    /// </summary>
    /// <param name="element"></param>
    /// <param name="resourcesTable"></param>
    /// <param name="number"></param>
    public void LoadFileRef(SdyElement element)
    {
        SdyFolder tempFolder;
        if (element is SdyFolder)
        {
            tempFolder = (SdyFolder)element;
            foreach (SdyElement item in tempFolder.Elements)
            {
                LoadFileRef(item);
            }
        }
        else if (element is SdyFileRef)
        {
            //PraseFileRef((SdyFileRef)element);
        }
    }

    /// <summary>
    /// 解析SdyProject的JSON对象
    /// </summary>
    /// <param name="jobject"></param>
    public SdyProject(JObject jobject)
    {
        Class = "Project";
        Name = string.Empty;
        Id = 0;
        EtpFilePath = string.Empty;
        Project = new();
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
                case "EtpFilePath":
                    EtpFilePath = (string)job.Value!;
                    break;
                case "Project":
                    Project = SdyConfiguration.LoadFromJarry((JObject)job.Value!);
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// 将SdyProject的C#写到XML中
    /// </summary>
    /// <param name="folderPath"></param>
    /// <returns></returns>
    public XmlDocument ToXML()
    
    {
        /*
        string folderPath
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        
        */

        //创建一个文档etpdoc
        XmlDocument etpDoc = new();
        /* 添加首行 XML文档说明： <?xml version="1.0" encoding="UTF-8"?> */
        XmlNode child = etpDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
        etpDoc.AppendChild(child);

        XmlElement projectXML = etpDoc.CreateElement("Project");

        #region 添加属性
        XmlNode tempNode;
        projectXML.SetAttribute("id", Id.ToString());
        projectXML.SetAttribute("oid_count", Project.Oid_count.ToString());
        projectXML.SetAttribute("defaultConfiguration", Project.DefaultConfiguration.ToString());
        #endregion
        #region 添加子Node
        //XmlNode child;
        /* props */
        child = etpDoc.CreateElement("props");
        foreach (SdyProp item in Project.Props)
        {
            tempNode = item.ToXML(etpDoc);
            if (tempNode != null)
            {
                child.AppendChild(tempNode);
            }

        }
        projectXML.AppendChild(child);
        /* roots */
        child = etpDoc.CreateElement("roots");
        foreach (SdyElement item in Project.Roots)
        {
            tempNode = item.ToXML(etpDoc);
            if (tempNode != null)
            {
                child.AppendChild(tempNode);
            }
        }
        projectXML.AppendChild(child);
        /* configurations */
        child = etpDoc.CreateElement("configurations");
        foreach (Configurations item in Project.Configuration)
        {
            tempNode = item.ToXML(etpDoc);
            if (tempNode != null)
            {
                child.AppendChild(tempNode);
            }
        }
        projectXML.AppendChild(child);
        #endregion
        etpDoc.AppendChild(projectXML);

        // etpDoc.Save(Path.Combine(folderPath, Name + ".etp"));

        return etpDoc;
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
        json[nameof(EtpFilePath)] = EtpFilePath;
        json.Add(nameof(Project), Project.ToJson());

        return json;
    }

}
