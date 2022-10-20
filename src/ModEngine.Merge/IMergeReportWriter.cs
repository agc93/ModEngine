using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ModEngine.Core;

namespace ModEngine.Merge;

public interface IMergeReportWriter<TMod> where TMod : Mod
{
    Task<Uri?> WriteReport(IEnumerable<MergeComponent<TMod>> mergeComponents, Dictionary<string, string> inputParameters, string? reportPath = null);
}