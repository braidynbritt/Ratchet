namespace Ratchet.Semantic
{
    internal class ErrorType
    {
        public string ErrorMsg { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }
        public ErrorType(string errorMsg, int line, int column )
        {
            ErrorMsg = errorMsg;
            Line = line;
            Column = column;
        }
    }
}
