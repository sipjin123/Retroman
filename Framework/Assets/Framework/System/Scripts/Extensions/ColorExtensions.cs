namespace Framework
{
    public enum Color
    {
        black,
        blue,
        cyan,
        gray,
        green,
        red,
        white,
        yellow,
    }

    public static class D
    {
        public static readonly string LOG = Color.green.LogHeader("[LOG]");
        public static readonly string WARNING = Color.yellow.LogHeader("[WARNING]");
        public static readonly string ERROR = Color.red.LogHeader("[ERROR]");
        public static readonly string CHECK = Color.white.LogHeader("[CHECK]");
        public static readonly string F = Color.green.LogHeader("[Framework]");
        public static readonly string A = Color.blue.LogHeader("[ARIES]");
        public static readonly string B = Color.blue.LogHeader("[BURLIN]");
        public static readonly string SOCKETS = Color.white.LogHeader("[SOCKETS]");
        public static readonly string LOBBY = Color.green.LogHeader("[LOBBY]");
        public static readonly string GAME = Color.blue.LogHeader("[GAME]");

        public static string L(string header)
        {
            return Color.green.LogHeader(header);
        }

        public static string W(string header)
        {
            return Color.yellow.LogHeader(header);
        }

        public static string E(string header)
        {
            return Color.red.LogHeader(header);
        }
    }

    public static class ColorExtensions
    {
        public static string LogHeader(this Color color, string header)
        {
            return string.Format("<color={0}>{1}</color> ", color.ToString(), header);
        }
    }
}