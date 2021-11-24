using System.Text;
using System.Text.Json.Serialization;

namespace ModEngine.Core
{
    public abstract class Mod
    {
        [JsonPropertyName("_meta")] public SetMetadata? Metadata { get; set; } = new();

        public string GetLabel(string defaultValue = "unknown mod")
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(this?.Metadata?.DisplayName)) {
                sb.Append(this.Metadata.DisplayName);
            }
            if (!string.IsNullOrWhiteSpace(this?.Metadata?.Author)) {
                sb.Append($" (by {this.Metadata.Author})");
            }
            return sb.Length > 0 ? sb.ToString() : defaultValue;
        }
    }
}