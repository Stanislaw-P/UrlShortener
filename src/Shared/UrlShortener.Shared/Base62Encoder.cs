using System.Text;

namespace UrlShortener.Shared
{
    public class Base62Encoder
    {
        const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        public static string Encode(long number)
        {
            if (number == 0) 
                return Alphabet[0].ToString();

            var sb = new StringBuilder();
            while (number > 0)
            {
                sb.Insert(0, Alphabet[(int)number % 62]);
                number /= 62;
            }

            return sb.ToString();
        }

        public static long Decode(string base62Text)
        {
            long number = 0;

            foreach (var ch in base62Text)
            {
                int index = Alphabet.IndexOf(ch);
                if (index == -1)
                    throw new ArgumentException("Недопустимый символ в строке Base62.");

                number = number * 62 + index;
            }

            return number;
        }
    }
}
