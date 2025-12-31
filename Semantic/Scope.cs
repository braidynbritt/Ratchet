namespace Ratchet.Semantic
{
    internal class Scope
    {
        public Scope? ParentScope { get; set; }
        public Dictionary<string, Symbol> Symbols;


        public Scope()
        {
            Symbols = new Dictionary<string, Symbol>();
        }
        public bool Define(string name, string? typeName = null, bool isInferred = false)
        {
            if (!Symbols.ContainsKey(name))
            {
                Symbols.Add(name, new Symbol(name, typeName, isInferred));
                return true;
            }

            return false;
        }

        public bool DefineFunction(string name, List<string?> paramTypes, string? returnType = null)
        {
            if (!Symbols.ContainsKey(name))
            {
                Symbols.Add(name, new FunctionSymbol(name, paramTypes, returnType));
                return true;
            }
            return false;
        }

        public bool ExistsInCurrentScope(string name)
        {
            if (Symbols.ContainsKey(name))
            {
                return true;
            }
            return false;
        }

        public bool ExistsInAnyScope(string name)
        {
            if (Symbols.ContainsKey(name))
            {
                return true;
            }
            else if (ParentScope != null)
            {
                return ParentScope.ExistsInAnyScope(name);
            }
            else
            {
                return false;
            }
        }

        public Symbol? GetSymbol(string name)
        {
            if (Symbols.TryGetValue(name, out var s)) return s;
            return ParentScope?.GetSymbol(name);
        }

        public FunctionSymbol? GetFunction(string name)
        {
            var sym = GetSymbol(name);
            return sym as FunctionSymbol;
        }
    }
}
