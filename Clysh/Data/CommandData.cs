using System.ComponentModel.DataAnnotations;

// ReSharper disable All

namespace Clysh.Data;

// This class is used only to deserialize command data from JSON or YAML.
/// <summary>
/// Class used to deserialize command data from file
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public class CommandData
{
    /// <summary>
    /// The id of command
    /// </summary>
    [Required]
    public string Id { get; set; } = default!;

    /// <summary>
    /// The description
    /// </summary>
    [Required]
    public string Description { get; set; } = default!;

    /// <summary>
    /// Indicates if it is the root command
    /// </summary>
    public bool Root { get; set; }

    /// <summary>
    /// The command options data
    /// </summary>
    public List<OptionData>? Options { get; set; }

    /// <summary>
    /// Indicates if require subcommand
    /// </summary>
    public bool RequireSubcommand { get; set; }
}