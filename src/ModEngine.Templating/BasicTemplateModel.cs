using System.Collections.Generic;

namespace ModEngine.Templating
{
    public class BasicTemplateModel : ITemplateModel
    {
        public string Name { get; init; }
        public Dictionary<string, string> Values { get; init; }
        public Dictionary<string, string> GetModel()
        {
            return Values;
        }
    }
}