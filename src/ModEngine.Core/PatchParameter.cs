using System.Text.Json.Serialization;

namespace ModEngine.Core;

/// <summary>
/// A simple convenience type for projects that want to use input parameters with custom mod types. 
/// You may need to replace this with your own implementation if your project has more complex requirements. 
/// </summary>
public class PatchParameter
{
    [JsonConstructor]
    public PatchParameter(string id, string message) {
        Id = id;
        Message = message;
    }

    public string Id { get; set; }
    public ParameterType Type { get; set; } = ParameterType.String;
    public string Message { get; set; }
    public string? Default { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Range { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Pattern { get; set; }
}

/// <summary>
/// A simple convenience type for input types when using patch parameters.
/// You may need to replace this with your own implementation if your project has more complex requirements. 
/// </summary>
public enum ParameterType
{
    String,
    Number,
    Boolean
}