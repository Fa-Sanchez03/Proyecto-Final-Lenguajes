using System;
using System.Collections.Generic;
using System.Text;
namespace FrameworkCiphers;
public abstract class Cipher
{
    public abstract string Encrypt(string plaintext);
    public abstract string Decrypt(string ciphertext);
}
