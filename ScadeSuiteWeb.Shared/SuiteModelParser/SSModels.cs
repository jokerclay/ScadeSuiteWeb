namespace ParseSuite
{
    public class SSModel
    {
        private string title = "";
        private string subTitle = "";
        private string company = "";
        private string projectDescription = "";
        private string project = "";
        private string diagramDir = "";

        private List<SSPackageType> types = new();
        private List<SSPackageSignal> signals = new();
        private List<SSPackageConst> consts = new();
        private List<SSNode> nodes = new();
        private List<SSRequirement> requirements = new();
        private List<SSTreeNode> nodeTree = new();

        public string Title { get => title; set => title = value; }
        public string Subtitle { get => subTitle; set => subTitle = value; }
        public string Company { get => company; set => company = value; }
        public string ProjectDescript { get => projectDescription; set => projectDescription = value; }
        public string Project { get => project; set => project = value; }
        public string DiagramDir { get => diagramDir; set => diagramDir = value; }
        public List<SSNode> Nodes { get => nodes; set => nodes = value; }
        public List<SSPackageType> Types { get => types; set => types = value; }
        public List<SSPackageSignal> Signals { get => signals; set => signals = value; }
        public List<SSPackageConst> Consts { get => consts; set => consts = value; }
        public List<SSTreeNode> NodeTree { get => nodeTree; set => nodeTree = value; }
        public List<SSRequirement> Requirements { get => requirements; set => requirements = value; }
    }

    #region 数据类型
    public class SSType
    {
        private string name = "";
        private string detail = "";
        private string comment = "";
        private List<string> requirementID = new();
        private List<SSStruct> structMember = new();

        public string Name { get => name; set => name = value; }
        public string Detail { get => detail; set => detail = value; }
        public string Comment { get => comment; set => comment = value; }
        public List<string> RequirementID { get => requirementID; set => requirementID = value; }
        public List<SSStruct> StructMember { get => structMember; set => structMember = value; }
    }

    public class SSStruct
    {
        private string name = "";
        private string type = "";
        private string comment = "";

        public string Name { get => name; set => name = value; }
        public string Type { get => type; set => type = value; }
        public string Comment { get => comment; set => comment = value; }
    }
    #endregion 数据类型

    #region PackageType
    public class SSPackageType
    {
        private string package = "";
        private List<SSType> types = new();

        public string Package { get => package; set => package = value; }
        public List<SSType> Types { get => types; set => types = value; }
    }
    #endregion PackageType

    #region Signal
    public class SSSignal
    {
        private string name = "";
        private string type = "";
        private string comment = "";
        private List<string> requirementID = new();

        public string Name { get => name; set => name = value; }
        public string Type { get => type; set => type = value; }
        public string Comment { get => comment; set => comment = value; }

        public List<string> RequirementID { get => requirementID; set => requirementID = value; }
    }
    #endregion Signal

    #region PackageSignal
    public class SSPackageSignal
    {
        private string package = "";
        private List<SSSignal> signals = new();
        public string Package { get => package; set => package = value; }
        public List<SSSignal> Signals { get => signals; set => signals = value; }
    }
    #endregion PackageSignal

    #region Const
    public class SSConst
    {
        private string name = "";
        private string type = "";
        private string value = "";
        private string comment = "";
        private List<string> requirementID = new();

        public string Name { get => name; set => name = value; }
        public string Type { get => type; set => type = value; }
        public string Value { get => value; set => this.value = value; }
        public string Comment { get => comment; set => comment = value; }

        public List<string> RequirementID { get => requirementID; set => requirementID = value; }
    }
    #endregion Const

    #region PackageConst
    public class SSPackageConst
    {
        private string package = "";
        private List<SSConst> consts = new();
        public string Package { get => package; set => package = value; }
        public List<SSConst> Consts { get => consts; set => consts = value; }
    }
    #endregion PackageConst

    #region Requrements
    public class SSRequirement
    {
        private string requirementID = "";
        private string description = "";

        public string RequirementID { get => requirementID; set => requirementID = value; }
        public string Description { get => description; set => description = value; }

    }
    #endregion Requrements

    #region Input
    public class SSInput
    {
        private string name = "";
        private string dataType = "";
        private string comment = "";
        private List<string> requirementID = new();

        public string Name { get => name; set => name = value; }
        public string DataType { get => dataType; set => dataType = value; }
        public string Comment { get => comment; set => comment = value; }
        public List<string> RequirementID { get => requirementID; set => requirementID = value; }

        public bool Actived { get; set; } = false;
    }
    #endregion Input

    #region Output
    public class SSOutput
    {
        private string name = "";
        private string dataType = "";
        private string comment = "";
        private List<string> requirementID = new();
        public string Name { get => name; set => name = value; }
        public string DataType { get => dataType; set => dataType = value; }
        public string Comment { get => comment; set => comment = value; }
        public List<string> RequirementID { get => requirementID; set => requirementID = value; }
        
        public bool Actived { get; set; } = false;
    }
    #endregion Output

    #region Param
    public class SSParam
    {
        private string name = "";
        private string dataType = "";
        private string comment = "";
        private List<string> requirementID = new();
        public string Name { get => name; set => name = value; }
        public string DataType { get => dataType; set => dataType = value; }
        public string Comment { get => comment; set => comment = value; }
        public List<string> RequirementID { get => requirementID; set => requirementID = value; }
    }
    #endregion Param

    #region Local
    public class SSLocal
    {
        private string name = "";
        private string dataType = "";
        private string comment = "";
        private List<string> requirementID = new();
        public string Name { get => name; set => name = value; }
        public string DataType { get => dataType; set => dataType = value; }
        public string Comment { get => comment; set => comment = value; }
        public List<string> RequirementID { get => requirementID; set => requirementID = value; }
    }
    #endregion Local

    #region SSTextDiagram
    public class SSTextDiagram
    {
        private string name = "";
        private string text = "";
        public string Name { get => name; set => name = value; }
        public string Text { get => text; set => text = value; }
    }
    #endregion SSTextDiagram

    #region SubNode
    public class SSSubNode
    {
        private string name = "";
        private string package = "";

        public string Name { get => name; set => name = value; }
        public string Package { get => package; set => package = value; }
    }
    #endregion SubNode

    #region TreeNode
    public class SSTreeNode
    {
        private string name = "";
        private string package = "";
        private List<SSTreeNode> children = new();

        public string Name { get => name; set => name = value; }
        public string Package { get => package; set => package = value; }
        public List<SSTreeNode> Children { get => children; set => children = value; }
    }
    #endregion TreeNode

    #region Node
    public class SSNode
    {
        private string name = "";
        private string package = "";
        private string nodeKind = "";    //Imported or NetDiagram or TextDiagram
        private string importFile = "";  //导入节点的源文件全路径,导入节点无Diagram
        private string description = "";

        private List<SSInput> inputs = new();
        private List<SSOutput> outputs = new();
        private List<SSParam> parameters = new();
        private List<SSLocal> locals = new();
        private List<string> diagrams = new();  //图表
        private List<SSTextDiagram> textDiagram = new(); //当节点设计内容以文本形式表示时（即TextDiagram），无Diagram图片，此属性表示设计的文本内容
        private List<SSSubNode> callNodes = new();
        private List<SSSubNode> calledNodes = new();
        private List<string> requirementID = new();

        private string csuId = ""; //软件单元标识号
        private string csuVer = ""; //软件单元版本号
        private string entryFunction = ""; //软件单元入口函数
        private string rulesAndAlgorithms = "";  //规则与算法
        private string exceptionHandling = "";  //错误或异常处理
        private string interruptionProcessing = "";  //中断与处理
        private string dataFile = "";  //局部数据文件或数据库


        public string Name { get => name; set => name = value; }
        public string Package { get => package; set => package = value; }
        public string NodeKind { get => nodeKind; set => nodeKind = value; }
        public string ImportFile { get => importFile; set => importFile = value; }
        public List<SSTextDiagram> TextDiagram { get => textDiagram; set => textDiagram = value; }
        public string Description { get => description; set => description = value; }
        public List<SSInput> Inputs { get => inputs; set => inputs = value; }
        public List<SSOutput> Outputs { get => outputs; set => outputs = value; }
        public List<SSLocal> Locals { get => locals; set => locals = value; }
        public List<SSSubNode> CallNodes { get => callNodes; set => callNodes = value; }
        public List<SSSubNode> CalledNodes { get => calledNodes; set => calledNodes = value; }
        public List<string> Diagram { get => diagrams; set => diagrams = value; }
        public List<SSParam> Params { get => parameters; set => parameters = value; }
        public List<string> RequirementID { get => requirementID; set => requirementID = value; }
        public string CsuId { get => csuId; set => csuId = value; }
        public string CsuVer { get => csuVer; set => csuVer = value; }
        public string EntryFunction { get => entryFunction; set => entryFunction = value; }
        public string RulesAndAlgorithms { get => rulesAndAlgorithms; set => rulesAndAlgorithms = value; }
        public string ExceptionHandling { get => exceptionHandling; set => exceptionHandling = value; }
        public string InterruptionProcessing { get => interruptionProcessing; set => interruptionProcessing = value; }
        public string DataFile { get => dataFile; set => dataFile = value; }
    }
    #endregion Node


    public class SSReqTrace
    {
        private string reqID = "";
        private string reqDesc = "";
        private string keyReq = "";
        private string keyModel = "";
        private string modelName = "";
        private string modelDesc = "";
        private string modelLink = "";
        private string modelTips = "";

        public SSReqTrace(SSRequirement req, string modelName, string modelDesc, string modelLink, string modelTips)
        {
            reqID = req.RequirementID;
            reqDesc = req.Description;
            this.modelName = modelName;
            this.modelDesc = modelDesc;
            KeyReq = reqID + "___" + this.modelName;
            keyModel = this.modelName + "___" + reqID;
            this.modelLink = modelLink;
            ModelTips = modelTips;
        }
        public string RequirementID { get => reqID; set => reqID = value; }
        public string Description { get => reqDesc; set => reqDesc = value; }
        public string KeyReq { get => keyReq; set => keyReq = value; }
        public string KeyModel { get => keyModel; set => keyModel = value; }
        public string ModelName { get => modelName; set => modelName = value; }
        public string ModelDescription { get => modelDesc; set => modelDesc = value; }
        public string ModelLink { get => modelLink; set => modelLink = value; }
        public string ModelTips { get => modelTips; set => modelTips = value; }
    }
}
