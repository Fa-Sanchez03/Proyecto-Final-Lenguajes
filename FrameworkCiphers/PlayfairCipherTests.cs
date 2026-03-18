#if DEBUG
using System;
using FrameworkCiphers;

//Demo de ejemplos
Console.WriteLine("Playfair cipher demo\n");

const string key = "PLAYFAIREXAMPLE";
const string plaintext = "HIDETHEGOLDINTHETREESTUMP";
const string ciphertext = "BMODZBXDNABEKUDMUIXMMOUVIF";

var cipher = new PlayfairCipher(key);

//Encryption
var encrypted = cipher.Encrypt(plaintext);
Console.WriteLine($"Encrypting\nKey: {key}\nPlaintext: {plaintext}\nCiphertext: {encrypted}\n");

//Decryption
var decrypted = cipher.Decrypt(ciphertext);
Console.WriteLine($"Decrypting\nKey: {key}\nCiphertext: {ciphertext}\nPlaintext: {decrypted}\n");

Console.WriteLine("Demo finished\n");
#endif
