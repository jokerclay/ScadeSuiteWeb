using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models.Base;
using Blazor.Diagrams.Models;

public class XPortModel : PortModel
{
    public bool In { get; set; }
    public XPortModel(SvgNodeModel  parent, bool @in, PortAlignment alignment = PortAlignment.Bottom, Point? position = null, Size? size = null) : base(parent, alignment, position, size)
    {
        In = @in;
    }

    public XPortModel(string id, SvgNodeModel parent, bool @in, PortAlignment alignment = PortAlignment.Bottom, Point? position = null, Size? size = null) : base(id, parent, alignment, position, size)
    {
        In = @in;

    }

    public override bool CanAttachTo(ILinkable other)
    {
        if (other.Links.Count >= 1)
        {
            return false;
        }
        // default constraints
        if (!base.CanAttachTo(other))
        {
            return false;
        }

        if (other is not XPortModel otherPort)
        {
            return false;
        }

        if (In == false)
        {
            return false;
        }

        if (otherPort.In)
        {
            return false;
        }

        return true;
    }

}
