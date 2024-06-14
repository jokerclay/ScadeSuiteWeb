using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Models.Base;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.PathGenerators;
using Blazor.Diagrams.Core.Routers;
using Blazor.Diagrams.Options;
using Blazor.Diagrams;
using ScadeSuiteWeb.Client.Editor.NodeModel;
using Blazor.Diagrams.Core;
using Blazor.Diagrams.Core.Geometry;
using Microsoft.AspNetCore.Components;
using ParseSuite;
using Microsoft.FluentUI.AspNetCore.Components;

namespace ScadeSuiteWeb.Client.Editor.Draw;

public partial class Drawer
{
    /// <summary>
    /// 外部数据源
    /// </summary>
    [Parameter]
    public SSNode? NodeModel { get; set; }
    
    private BlazorDiagram Diagram { get; set; } = null!;
    
    [Inject]
    public DiagramService DiagramService { get; set; }

    protected override void OnInitialized()
    {
        DiagramService.OnInputAdded += HandleInputAdded;
        DiagramService.OnOutputAdded += HandleOutputAdded;
        DiagramService.OnSymbolAdded += HandleSymbolAdded;
        
        /*---------------------------------------------------    
        对 Diagram 的配置
        ---------------------------------------------------*/
        var options = new BlazorDiagramOptions
        {
            AllowMultiSelection = true,
            Zoom =
            {
                Enabled = true,
            },
            Links =
            {
                DefaultRouter = new NormalRouter(),
                DefaultPathGenerator = new SmoothPathGenerator(),
                Factory = ModelLinkCheck
            },
        };
        Diagram = new BlazorDiagram(options);

        /*---------------------------------------------------    
        注册 Node 
        ---------------------------------------------------*/
        Diagram.RegisterComponent<InputNodeModel, InputNode>();
        Diagram.RegisterComponent<OutputNodeModel, OutputNode>();
        Diagram.RegisterComponent<AddFuncNodeModel, AddFuncNode>();

        
        /*---------------------------------------------------    
        input model
        ---------------------------------------------------*/
        var node = new InputNodeModel(new Point(80, 80))
        {
            Title = "node",
        };
        Diagram.Nodes.Add(node );
         
        node.AddPort(new XPortModel(node, true, PortAlignment.Right));
        
        var node1 = new InputNodeModel(new Point(180, 180))
        {
            Title = "node1"
        };
            
        Diagram.Nodes.Add(node1);
        node1.AddPort(new XPortModel(node1, true, PortAlignment.Right));

        /*---------------------------------------------------
        output model
        ---------------------------------------------------*/
        
        var outputNode =new OutputNodeModel(new Point(380, 180)) { Title = "output" };
        Diagram.Nodes.Add(outputNode);
        outputNode.AddPort(new XPortModel(outputNode, false, PortAlignment.Left));

        
        /*---------------------------------------------------    
        add func model
        ---------------------------------------------------*/
        var addFuncNode = new AddFuncNodeModel(new Point(380, 180)) { Title = "Add" };
        Diagram.Nodes.Add(addFuncNode);
        
        addFuncNode.AddPort(new XPortModel(addFuncNode, true, PortAlignment.Right));
        addFuncNode.AddPort(new XPortModel(addFuncNode, false, PortAlignment.BottomLeft));
        addFuncNode.AddPort(new XPortModel(addFuncNode, false, PortAlignment.Left));
    }
    

    private LinkModel? ModelLinkCheck(Diagram diagram, ILinkable source, Anchor targetAnchor)
    {

        if (source is not XPortModel port)
        {
            return null;
        }

        if (port.In == false)
        {
            return null;
        }

        var a = new SinglePortAnchor(port);

        return new LinkModel(a, targetAnchor);
        
    }
    
    
    private void HandleInputAdded(SSInput inputNode)
    {
        var node = new InputNodeModel(new Point(80, 80))
        {
            Title = inputNode.Name,
        };
        Diagram.Nodes.Add(node);
        node.AddPort(new XPortModel(node, true, PortAlignment.Right));
        StateHasChanged();
    }

    private void HandleOutputAdded(SSOutput outputNode)
    {
        var node = new OutputNodeModel(new Point(80, 80))
        {
            Title = outputNode.Name,
        };
        Diagram.Nodes.Add(node);
        node.AddPort(new XPortModel(node, false, PortAlignment.Left));
        StateHasChanged();
    }
    
    private void HandleSymbolAdded()
    {
        var addFuncNode = new AddFuncNodeModel(new Point(380, 180)) { Title = "Add"};
        addFuncNode.AddPort(new XPortModel(addFuncNode, true, PortAlignment.Right));
        addFuncNode.AddPort(new XPortModel(addFuncNode, false, PortAlignment.BottomLeft));
        addFuncNode.AddPort(new XPortModel(addFuncNode, false, PortAlignment.Left));
        Diagram.Nodes.Add(addFuncNode);
    }


    public void Dispose()
    {
        DiagramService.OnInputAdded -= HandleInputAdded;
        DiagramService.OnOutputAdded -= HandleOutputAdded;
        DiagramService.OnSymbolAdded -= HandleSymbolAdded;
    }
    
    
}
