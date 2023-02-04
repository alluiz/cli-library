using System.Text.RegularExpressions;

namespace Clysh.Helper;

/// <summary>
/// The string indexable
/// </summary>
public abstract class ClyshIndexable: IClyshIndexable
{
    private string _id = default!;

    /// <summary>
    /// The pattern to validate the ID. Could be null if no pattern is required.
    /// </summary>
    public string? Pattern;
    
    /// <summary>
    /// The ID max length
    /// </summary>
    protected int MaxLength = 0;
    
    private Regex? _regex;

    /// <summary>
    /// The ID text
    /// </summary>
    public string Id
    {
        get => _id;
        set => _id = ValidatedId(value);
    }

    /// <summary>
    /// Validates the ID
    /// </summary>
    /// <param name="desiredId">The desired ID to be validated</param>
    /// <returns>The validated ID</returns>
    /// <exception cref="ArgumentException">The ID is invalid.</exception>
    private string ValidatedId(string desiredId)
    {
        if (desiredId == null)
            throw new ArgumentException(string.Format(ClyshMessages.ErrorOnValidateIdPattern, Pattern, desiredId), nameof(desiredId));

        if (MaxLength > 0 && desiredId.Length > MaxLength)
            throw new ArgumentException(string.Format(ClyshMessages.ErrorOnValidateIdLength, MaxLength, desiredId), nameof(desiredId));

        //No validation if no pattern was provided before.
        if (Pattern == null) 
            return desiredId;
        
        _regex ??= new Regex(Pattern);

        if (!_regex.IsMatch(desiredId))
            throw new ArgumentException(string.Format(ClyshMessages.ErrorOnValidateIdPattern, Pattern, desiredId), nameof(desiredId));

        return desiredId;
    }
}