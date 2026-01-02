namespace Ratchet.Symbols
{
    internal class Symbol
    {
        public string Name { get; set; }

        public string? TypeName { get; set; }

        public bool IsInferred { get; set; }

        public bool IsInitialized { get; set; }

        public Symbol(string name, string? typeName = null, bool isInferred = false, bool isInitialized = false)
        {
            Name = name;
            TypeName = typeName;
            IsInferred = isInferred;
            IsInitialized = isInitialized;
        }
    }
}
