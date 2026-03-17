#if DEBUG
using FrameworkCiphers;

Console.WriteLine("Playfair testing\n");
RunTest("PLAYFAIREXAMPLE", "HIDETHEGOLDINTHETREESTUMP");
RunTest("MONARCHY", "BALLOON");
Console.WriteLine("Fin del testing\n");

static void RunTest(string key, string plaintext)
{
    var cipher = new PlayfairCipher(key);
    var ciphertext = cipher.Encrypt(plaintext);
    Console.WriteLine($"Key: {key} | Plain: {plaintext} | Cipher: {ciphertext}");
}
#endif
