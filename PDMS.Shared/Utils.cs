using System.Text;

namespace PDMS.Shared;

public class Utils {
    private const string Base36Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    public const string UppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public const string LowercaseChars = "abcdefghijklmnopqrstuvwxyz";
    public const string Numbers = "0123456789";

    public static string LongToBase32(long number) {
        var buffer = new char[13];
        var index = 12;

        do {
            buffer[index--] = Base36Chars[(int)(number % 36)];
            number /= 36;
        } while (number > 0);

        return new string(buffer, index + 1, 12 - index);
    }

    public static string RandomString(int length, params string[] characterPools) {
        var builder = new StringBuilder();
        var totalChars = string.Join("", characterPools);
        var random = new Random();
        for (int i = 0; i < length; i++) {
            builder.Append(totalChars[random.Next(0, totalChars.Length)]);
        }

        return builder.ToString();
    }
}