using System.Text.RegularExpressions;

namespace Clysh.Core.Builder;

/// <summary>
/// A builder for <see cref="ClyshOption"/>
/// </summary>
/// <seealso cref="ClyshBuilder{T}"/>
public class ClyshOptionBuilder: ClyshBuilder<ClyshOption>
{
    private const int MaxDescription = 50;
    private const int MinDescription = 10;
        
    private const int MinShortcut = 1;
    private const int MaxShortcut = 1;
    
    private const string Pattern = "[a-zA-Z]";
    
    private readonly Regex regex;

    /// <summary>
    /// The builder constructor
    /// </summary>
    public ClyshOptionBuilder()
    {
        regex = new Regex(Pattern);
    }

    /// <summary>
    /// Build the option identifier
    /// </summary>
    /// <param name="id">The option identifier</param>
    /// <returns>An instance of <see cref="ClyshOptionBuilder"/></returns>
    public ClyshOptionBuilder Id(string id)
    {
        Result.Id = id;
        return this;
    }
    
    /// <summary>
    /// Build the option description
    /// </summary>
    /// <param name="description">The option description</param>
    /// <returns>An instance of <see cref="ClyshOptionBuilder"/></returns>
    public ClyshOptionBuilder Description(string description)
    {
        Validate(nameof(description), description, MinDescription, MaxDescription);
        Result.Description = description;
        return this;
    }
    
    /// <summary>
    /// Build the option shortcut
    /// </summary>
    /// <param name="shortcut">The option shortcut</param>
    /// <returns>An instance of <see cref="ClyshOptionBuilder"/></returns>
    public ClyshOptionBuilder Shortcut(string? shortcut)
    {
        if (shortcut != null && (shortcut.Length is < MinShortcut or > MaxShortcut || !regex.IsMatch(Pattern)))
            throw new ArgumentException($"Invalid shortcut. The shortcut must be null or follow the pattern {Pattern} and between {MinShortcut} and {MaxShortcut} chars.",
                nameof(shortcut));

        if (Result.Id is not "help" && shortcut is "h")
            throw new ArgumentException("Shortcut 'h' is reserved to help shortcut.", nameof(shortcut));
        
        Result.Shortcut = shortcut;
        return this;
    }
    
    /// <summary>
    /// Build the option parameters
    /// </summary>
    /// <param name="parameters">The option parameters</param>
    /// <returns>An instance of <see cref="ClyshOptionBuilder"/></returns>
    public ClyshOptionBuilder Parameters(ClyshParameters parameters)
    {
        Result.Parameters = parameters;
        return this;
    }

    /// <summary>
    /// Build the option group
    /// </summary>
    /// <param name="group">The option group</param>
    /// <returns>An instance of <see cref="ClyshOptionBuilder"/></returns>
    public ClyshOptionBuilder Group(ClyshGroup group)
    {
        Result.Group = group;
        return this;
    }

    /// <summary>
    /// Build the option selected
    /// </summary>
    /// <param name="selected">The option selected</param>
    /// <returns>An instance of <see cref="ClyshOptionBuilder"/></returns>
    public ClyshOptionBuilder Selected(bool selected)
    {
        Result.Selected = selected;
        return this;
    }
    
    private static void Validate(string? field, string? value, int min, int max)
    {
        if (value == null || value.Trim().Length < min || value.Trim().Length > max)
            throw new ArgumentException($"Option {field} must be not null or empty and between {min} and {max} chars.", field);
    }
}