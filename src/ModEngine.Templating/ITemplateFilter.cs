using System.Threading.Tasks;
using Fluid;
using Fluid.Values;

namespace ModEngine.Templating
{
    public interface ITemplateFilter
    {
        string Name { get; }
        ValueTask<FluidValue> RunFilter(FluidValue input, FilterArguments arguments, TemplateContext ctx);
    }
}