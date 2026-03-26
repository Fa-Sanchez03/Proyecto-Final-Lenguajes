using System;
using System.Text;

namespace FrameworkCiphers;

public class VigenereCipher : Cipher
{
    private readonly string _key;

    public VigenereCipher(string key)
    {
        _key = NormalizeKey(key);
    }

    public override string Encrypt(string text)
    {
        return Transform(text, _key, encrypt: true);
    }

    public override string Decrypt(string text)
    {
        return Transform(text, _key, encrypt: false);
    }

    public string Encrypt(string text, string key)
    {
        return Transform(text, NormalizeKey(key), true);
    }

    public string Decrypt(string text, string key)
    {
        return Transform(text, NormalizeKey(key), false);
    }

    // Applies per-character Vigenere shifts while skipping non-letter symbols.
    private static string Transform(string text, string key, bool encrypt)
    {
        if (text is null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        var sb = new StringBuilder(text.Length);
        var keyIndex = 0;

        foreach (var ch in text)
        {
            if (char.IsLetter(ch))
            {
                var shift = key[keyIndex % key.Length] - 'A';
                if (!encrypt)
                {
                    shift = (26 - shift) % 26;
                }

                var baseChar = char.IsUpper(ch) ? 'A' : 'a';
                sb.Append((char)(baseChar + (ch - baseChar + shift) % 26));
                keyIndex++;
            }
            else
            {
                sb.Append(ch);
            }
        }

        return sb.ToString();
    }

    private static string NormalizeKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Key must contain alphabetic characters.", nameof(key));
        }

        var sb = new StringBuilder(key.Length);
        foreach (var ch in key)
        {
            if (char.IsLetter(ch))
            {
                sb.Append(char.ToUpperInvariant(ch));
            }
        }

        if (sb.Length == 0)
        {
            throw new ArgumentException("Key must contain at least one alphabetic character.", nameof(key));
        }

        return sb.ToString();
    }
}