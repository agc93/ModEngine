using System.Collections.Generic;

namespace ModEngine.Templating
{
    public interface ITemplateFilterProvider
    {
        IEnumerable<ITemplateFilter> LoadFilters();
    }
}