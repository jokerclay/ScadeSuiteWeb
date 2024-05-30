using System.Security.Cryptography;
using System.Text;

namespace ScadeSuiteWeb.Shared.Utils;

public static class Encrypt
{
    public static string Sha256EncryptString(string data)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(data);
        byte[] hash = SHA256.Create().ComputeHash(bytes);

       StringBuilder builder = new();
        foreach (byte t in hash)
        {
            builder.Append(t.ToString("x2"));
        }
        return builder.ToString();
    }
}