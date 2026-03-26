using System;
using System.Text;

namespace FrameworkCiphers;

public class CaesarCipher : Cipher
{
    private readonly int _shift;

    public CaesarCipher(int shift)
    {
        _shift = NormalizeShift(shift);
    }

    public override string Encrypt(string plaintext)
    {
        return ShiftText(plaintext, _shift);
    }

    public override string Decrypt(string ciphertext)
    {
        return ShiftText(ciphertext, 26 - _shift);
    }

    public string Encrypt(string plaintext, int shift)
    {
        return ShiftText(plaintext, NormalizeShift(shift));
    }

    public string Decrypt(string ciphertext, int shift)
    {
        return ShiftText(ciphertext, 26 - NormalizeShift(shift));
    }

    private static string ShiftText(string text, int shift)
    {
        var sb = new StringBuilder(text.Length);

        foreach (var ch in text)
        {
            if (char.IsUpper(ch))
            {
                sb.Append((char)('A' + (ch - 'A' + shift) % 26));
            }
            else if (char.IsLower(ch))
            {
                sb.Append((char)('a' + (ch - 'a' + shift) % 26));
            }
            else
            {
                sb.Append(ch);
            }
        }

        return sb.ToString();
    }

    private static int NormalizeShift(int shift)
    {
        var s = shift % 26;
        return s < 0 ? s + 26 : s;
    }
}
