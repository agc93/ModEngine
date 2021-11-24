namespace ModEngine.Core
{
    public class Patch
    {
        public int? Version { get; set; } = null;
        public string? Description {get;set;}
        public string Template {get;set;}
        public string Value {get;set;}
        public string Type {get;set;} = string.Empty;
        public MatchWindow? Window {get;set;} = null;
    }
}