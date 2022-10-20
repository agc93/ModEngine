using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using ModEngine.Core;

namespace ModEngine.Merge;

public class JsonReportWriter<TMod> : IMergeReportWriter<TMod> where TMod : Mod
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public JsonReportWriter(JsonSerializerOptions? jsonSerializerOptions = null) {
        _jsonSerializerOptions = jsonSerializerOptions ?? new();
    }

    public async Task<Uri?> WriteReport(IEnumerable<MergeComponent<TMod>> mergeComponents, Dictionary<string, string> inputParameters, string? reportPath = null) {
        var reportFile = await BuildReport(mergeComponents, inputParameters);
        if (!string.IsNullOrWhiteSpace(reportPath)) {
            reportFile.MoveTo(reportPath, true);
            return new Uri(new FileInfo(reportPath).FullName);
        }
        return new Uri(reportFile.FullName);
    }

    private async Task<FileInfo> BuildReport(IEnumerable<MergeComponent<TMod>> mergeComponents,
        Dictionary<string, string> inputParameters) {
        var opts = new JsonSerializerOptions(_jsonSerializerOptions) {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        var report = new Dictionary<string, object> {[nameof(inputParameters)] = inputParameters};
        foreach (var component in mergeComponents.Where(mc =>
                     mc.MergedResources != null && !string.IsNullOrWhiteSpace(mc.Name) && mc.MergedResources.Any())) {
            report.Add(component.Name!, component.MergedResources!);
        }
        var json = JsonSerializer.Serialize(report, opts);
        var file = new FileInfo(Path.GetTempFileName());
        await File.WriteAllTextAsync(file.FullName, json);
        return file;
    }
        
        
}