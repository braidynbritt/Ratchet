namespace Ratchet.Semantic
{
    internal class Symbol
    {
        public string Name { get; set; }

        public string? TypeName { get; set; }

        public bool IsInferred { get; set; }

        public Symbol(string name, string? typeName = null, bool isInferred = false)
        {
            Name = name;
            TypeName = typeName;
            IsInferred = isInferred;
        }
    }
}
