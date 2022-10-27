using System.Text.Encodings.Web;
using System.Text.Json;

namespace ModEngine.Core;

public class ModParser<TMod> where TMod : Mod
{
    public ModParser() {
        
    }

    public ModParser(JsonSerializerOptions jsonOpts) {
        _jsonOpts = jsonOpts;
    }
    private readonly JsonSerializerOptions _jsonOpts = new() {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        Converters =
        {
            new System.Text.Json.Serialization.JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };
    public virtual string ToJson(Mod mod)
    {
        return JsonSerializer.Serialize(mod, _jsonOpts);
    }

    public virtual TMod? ParseMod(string rawJson)
    {
        return JsonSerializer.Deserialize<TMod>(rawJson, _jsonOpts);
    }

    public virtual bool IsValid(TMod? mod) {
        return mod is {} jsonMod;
    }

    public JsonSerializerOptions Options => _jsonOpts;

    public JsonSerializerOptions RelaxedOptions => new(_jsonOpts) {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
}