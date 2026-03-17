#if DEBUG
using FrameworkCiphers;

Console.WriteLine("Playfair testing\n");

//Llave con lo que se va a cifrar el texto
const string keyUnderTest = "PLAYFAIREXAMPLE";        

//Texto que se va a cifrar
const string plaintextUnderTest = "HIDETHEGOLDINTHETREESTUMP"; 

RunTest(keyUnderTest, plaintextUnderTest);

Console.WriteLine("Fin del testing\n");

static void RunTest(string key, string plaintext)
{
    var cipher = new PlayfairCipher(key);
    var ciphertext = cipher.Encrypt(plaintext);
    Console.WriteLine($"Key: {key} | Plain: {plaintext} | Cipher: {ciphertext}");
}
#endif
