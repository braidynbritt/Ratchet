namespace Ratchet.Semantic
{
    internal class FunctionSymbol : Symbol
    {
        public List<string?> ParamTypes { get; }
        public string? ReturnType { get; set; }

        public FunctionSymbol(string name, List<string?> paramTypes, string? returnType = null) : base(name)
        {
            ParamTypes = paramTypes ?? new List<string?>();
            ReturnType = returnType;
        }
    }
}