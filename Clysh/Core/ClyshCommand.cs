using Clysh.Helper;

namespace Clysh.Core;

public class ClyshCommand : ClyshSimpleIndexable, IClyshCommand
{
    public Action<ClyshMap<ClyshOption>, IClyshView>? Action { get; set; }
    public ClyshMap<ClyshCommand> Children { get; }
    public ClyshMap<ClyshGroup> Groups { get; set; }
    public ClyshMap<ClyshOption> Options { get; }
    public IClyshCommand? Parent { get; set; }
    public int Order { get; set; }
    public string? Description { get; set; }

    private readonly Dictionary<string, string> shortcutToOptionId;
        
    public ClyshCommand()
    {
        Groups = new ClyshMap<ClyshGroup>();
        Options = new ClyshMap<ClyshOption>();
        Children = new ClyshMap<ClyshCommand>();
        shortcutToOptionId = new Dictionary<string, string>();
        AddHelpOption();
    }
        
    public void AddOption(ClyshOption option)
    {
        Options.Add(option);

        if (option.Shortcut != null)
            shortcutToOptionId.Add(option.Shortcut, option.Id);
    }

    private void AddHelpOption()
    {
        var builder = new ClyshOptionBuilder();
        var helpOption = builder
            .Id("help")
            .Description("Show help on screen")
            .Shortcut("h")
            .Build();
            
        AddOption(helpOption);
    }
        
    public void AddChild(ClyshCommand child)
    {
        child.Parent = this;
        Children.Add(child);
    }

    public ClyshOption? GetOptionFromGroup(ClyshGroup group)
    {
        return Options
            .Values
            .SingleOrDefault(x =>
                x.Group == group && x.Selected);
    }

    public ClyshOption GetOption(string arg)
    {
        try
        {
            return Options[arg];
        }
        catch (Exception)
        {
            return Options[shortcutToOptionId[arg]];
        }
    }

    public bool HasOption(string key)
    {
        return Options.Has(key) || shortcutToOptionId.ContainsKey(key);
    }

    public bool HasAnyChildren()
    {
        return Children.Any();
    }

    public bool HasChild(string name)
    {
        return Children.Has(name);
    }
}