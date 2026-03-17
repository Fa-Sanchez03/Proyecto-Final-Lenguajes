using FrameworkCiphers;

// Demo: Caesar cipher encrypt/decrypt
const string message = "Hello, World!";
const int shift = 3;

var cipher = new CaesarCipher(shift);
var encrypted = cipher.Encrypt(message);
var decrypted = cipher.Decrypt(encrypted);

Console.WriteLine($"Plain:    {message}");
Console.WriteLine($"Encrypted ({shift}): {encrypted}");
Console.WriteLine($"Decrypted: {decrypted}");
