using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Resources;
using System.Security.Claims;
using System.Xml;
using System.Xml.Linq;



public class ModelObject
{
    public string Class { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Id;
    /// <summary>
    /// 生成Json
    /// </summary>
    /// <returns></returns>
    public virtual JObject ToJson()
    {
        return new JObject();
    }
}
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
    public XmlDocument ToXML(string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

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

        etpDoc.Save(Path.Combine(folderPath, Name + ".etp"));

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

#region Prop
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
#endregion

#region Configurations
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
#endregion

#region SdyElement
/// <summary>
/// 定义 项目中的 Folder 和 FileRef 类的信息
/// </summary>
public class SdyElement : ModelObject
{
    public List<SdyProp> Props { get; set; }

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
#endregion

#region SdyFolder
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
#endregion

#region SdyFileRef
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
#endregion
