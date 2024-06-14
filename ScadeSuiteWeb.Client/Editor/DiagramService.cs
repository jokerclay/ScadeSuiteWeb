using ParseSuite;

namespace ScadeSuiteWeb.Client.Editor;

public class DiagramService
{
    
    public event Action<SSInput> OnInputAdded;
    public event Action<SSOutput> OnOutputAdded;

    public event Action OnSymbolAdded;
    public void AddInputNode(SSInput inputNode)
    {
        OnInputAdded?.Invoke(inputNode);
    }

    public void AddOutputNode(SSOutput outputNode)
    {
        OnOutputAdded?.Invoke(outputNode);
    }
    public void AddSymbol()
    {
        OnSymbolAdded?.Invoke();
    }
}