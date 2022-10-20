using System.Collections.Generic;

namespace ModEngine.Templating
{
    public interface ITemplateModel
    {
        string Name { get; }
        Dictionary<string, string> GetModel();
    }
}