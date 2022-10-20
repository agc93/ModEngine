using System.Collections.Generic;

namespace ModEngine.Templating
{
    public interface ITemplateModelProvider
    {
        IEnumerable<ITemplateModel> LoadModels();
    }
}