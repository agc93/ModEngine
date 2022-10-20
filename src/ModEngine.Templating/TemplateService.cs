using System.Collections.Generic;
using System.Linq;
using Fluid;
using ModEngine.Core;

namespace ModEngine.Templating
{
    public class TemplateService
    {
        private readonly FluidParser _parser;
        public FluidParser Parser => _parser;
        public TemplateService() {
            _parser = new FluidParser().AddTags();
        }
        
        public TemplateService(IEnumerable<ITemplateFilterProvider> templates, IEnumerable<ITemplateModelProvider> modelProviders) : this()
        {
            var templateFilterProviders = templates.ToList();
            if (templateFilterProviders.Any())
            {
                Filters = templateFilterProviders.SelectMany(provider => provider.LoadFilters());
            }

            var templateModelProviders = modelProviders.ToList();
            if (templateModelProviders.Any())
            {
                Models = templateModelProviders.SelectMany(provider => provider.LoadModels());
            }
        }
        
        public IEnumerable<ITemplateFilter> Filters { get; } = new List<ITemplateFilter>();
        public IEnumerable<ITemplateModel> Models { get; } = new List<ITemplateModel>();
        
        public string? Render(Dictionary<string, string> templateInputs, string? inputKey, Dictionary<string, string>? modelVars = null)
        {
            modelVars ??= new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(inputKey) && _parser.TryParse(inputKey, out var template))
            {
                return template.Render(GetInputContext(templateInputs, modelVars));
            }
            else
            {
                return inputKey;
            }
        }

        private TemplateContext GetContext(object templateInputs, Dictionary<string, string>? additionalVars = null) {
            var templOpts = TemplateOptions.Default;
            templOpts.WithFilters(Filters);
            var templCtx = new TemplateContext(templateInputs,templOpts);
            
            foreach (var templateModel in Models)
            {
                templCtx.SetValue(templateModel.Name, templateModel.GetModel());
            }
            if (additionalVars != null)
            {
                templCtx.SetValue("vars", additionalVars);
            }
            return templCtx;
        }

        public Dictionary<string, string> RenderVariables(Dictionary<string, string> requestInputs, Dictionary<string, string>? modVariables)
        {
            var validVars = new Dictionary<string, string>();
            if (modVariables != null)
            {
                foreach (var (varName, varTemplate) in modVariables)
                {
                    if (_parser.TryParse(varTemplate, out var subTemplate))
                    {
                        validVars.Add(varName, subTemplate.Render(GetInputContext(requestInputs, validVars)));
                    }
                }
            }
            return validVars;
        }

        private TemplateContext GetInputContext(Dictionary<string, string> templateInputs, Dictionary<string, string>? additionalVars = null)
        {
            return GetContext(new {inputs = templateInputs}, additionalVars);
        }

        public bool TryRender(string rawInput, Dictionary<string, string> templateInputs, Dictionary<string, string>? additionalVars, out string rendered) {
            if (_parser.TryParse(rawInput, out var templateResult)) {
                rendered = templateResult.Render(GetInputContext(templateInputs, additionalVars));
                return true;
            }
            rendered = string.Empty;
            return false;
        }

        public bool ValidateTemplate(string rawInput) {
            return _parser.TryParse(rawInput, out var _);
        }

        public PatchSet<TPatch> RenderPatch<TPatch>(PatchSet<TPatch> patch, Dictionary<string, string> templateInputs,
            Dictionary<string, string> modelVariables) where TPatch : Patch {
            var psList = patch;
            psList.Patches = psList.Patches.Select(p =>
            {
                if (TryRender(p.Value, templateInputs, modelVariables, out var rValue)) {
                    p.Value = rValue;
                }

                if (TryRender(p.Template, templateInputs, modelVariables, out var rTemplate)) {
                    p.Template = rTemplate;
                }

                if (p.Window != null) {
                    p.Window.After = Render(templateInputs, p.Window.After, modelVariables);
                    p.Window.Before = Render(templateInputs, p.Window.Before, modelVariables);
                }

                return p;
            }).ToList();
            return psList;
        }

        public List<PatchSet<Patch>> RenderPatches(List<PatchSet<Patch>> patches, Dictionary<string, string> templateInputs, Dictionary<string, string> modelVars) {
            var finalPatches = patches.Where(psList =>
            {
                    
                // REMEMBER: this is to keep the step, so have to return false to skip it
                // if (mod.ModInfo.StepsEnabled.ContainsKey(psList.Name ?? string.Empty) &&
                //     TryRender(mod.ModInfo.StepsEnabled[psList.Name], templateInputs, modelVars, out var rendered)) {
                //     var result = !bool.TryParse(rendered, out var skip) || skip;
                //     // var result = bool.TryParse(rendered, out var skip) || skip;
                //     // do NOT invert result: result *is* inverted
                //     return result;
                // }

                return true;
            }).Select(psList =>
            {
                psList.Patches = psList.Patches.Select(p =>
                {
                    if (TryRender(p.Value, templateInputs, modelVars, out var rValue)) {
                        p.Value = rValue;
                    }

                    if (TryRender(p.Template, templateInputs, modelVars, out var rTemplate)) {
                        p.Template = rTemplate;
                    }

                    if (p.Window != null) {
                        p.Window.After = Render(templateInputs, p.Window.After, modelVars);
                        p.Window.Before = Render(templateInputs, p.Window.Before, modelVars);
                    }

                    return p;
                }).ToList();
                return psList;
            }).ToList();
            return finalPatches;
        }
    }
}