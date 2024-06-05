using ParseSuite;
namespace ScadeSuiteWeb.Client.Services;

    public interface IModelManager
    {
        public SSModel Project { get; set; }
        /// <summary>
        /// 新建文件夹
        /// </summary>
        /// <param name="newFolder"></param>
        /// <returns></returns>
        public SSModel InsertInput(string inputName);
        /// <summary>
        /// 新插入文件
        /// </summary>
        /// <returns></returns>
        public SSModel InsertOutput(string outputName);
        /// <summary>
        /// 新建项目
        /// </summary>
        /// <returns></returns>
        public SSModel NewProject();
    }
    public class ModelManager : IModelManager
    {
        public SSModel Project { get; set; } = new();
        public SSModel NewProject()
        {
            Project = new()
            {
                Project = "ABC_N",
                Nodes = new List<SSNode>
                {
                    new SSNode()
                    {
                        Name = "ABC_N",
                        Inputs = new List<SSInput>()
                        {
                            new SSInput()
                            {
                                Name = "Lock",
                                DataType = "bool",
                                Comment = "",
                                RequirementID = new()
                            },
                            new SSInput()
                            {
                                Name = "Buttons",
                                DataType = "bool ^n",
                                Comment = "",
                                RequirementID = new()
                            }
                        },
                        Outputs = new List<SSOutput>()
                        {
                            new SSOutput()
                            {
                                Name = "Bg",
                                DataType = "bk_color ^n",
                                Comment = "",
                                RequirementID = new()
                            },
                            new SSOutput()
                            {
                                Name = "Fg",
                                DataType = "fr_color ^n",
                                Comment = "",
                                RequirementID = new()
                            },
                            new SSOutput()
                            {
                                Name = "LockLight",
                                DataType = "bool",
                                Comment = "",
                                RequirementID = new()
                            }
                        }
                    }
                }
            };
            return Project;
        }
        public SSModel InsertInput(string inputName )
        {
            SSInput input = new()
            {
                Name = inputName,
                DataType = "bool",
                Comment = "",
                RequirementID = new()
            };
            foreach(var item in Project.Nodes)
            {
                item.Inputs.Add(input);
            }
            return Project;
        }

        public SSModel InsertOutput(string outputName)
        {
            SSOutput output = new()
            {
                Name = outputName,
                DataType = "bool",
                Comment = "",
                RequirementID = new()
            };
            foreach (var item in Project.Nodes)
            {
                item.Outputs.Add(output);
            }
            return Project;
        }
    }
