using System.Text;

namespace Mao.Infrastructure.Util
{
    public static class StringUtil
    {
        public static string ReplaceLast(this string input, string oldValue, string newValue)
        {
            int index = input.LastIndexOf(oldValue, System.StringComparison.Ordinal);
            return index < 0 ? input : ReplaceFrom(input, index, oldValue, newValue);
        }

        public static string ReplaceFirst(this string input, string oldValue, string newValue)
        {
            int index = input.IndexOf(oldValue, System.StringComparison.Ordinal);
            return index < 0 ? input : ReplaceFrom(input, index, oldValue, newValue);
        }

        static string ReplaceFrom(string input, int pos, string oldValue, string newValue)
        {
            StringBuilder sb = new StringBuilder(input.Length - oldValue.Length + newValue.Length);
            sb.Append(input.Substring(0, pos));
            sb.Append(newValue);
            sb.Append(input.Substring(pos + oldValue.Length,
                input.Length - pos - oldValue.Length));

            return sb.ToString();
        }

        public static string Pad(int num, int len, char pad = '0')
        {
            var str = num.ToString();
            var l = str.Length;
            for (var i = l; i < len; i++)
            {
                str = pad + str;
            }
            return str;
        }
    }
}
