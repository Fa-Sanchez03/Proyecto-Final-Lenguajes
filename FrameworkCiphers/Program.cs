using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FrameworkCiphers;

// Expected file format inside Inputs/Encrypt or Inputs/Decrypt folders:
// cipher: caesar|vigenere|playfair
// key: <value>
// filler: <single letter>   (optional, Playfair only)
// text:
// <plain text or cipher text spanning any number of lines>

const string EncryptDirectory = "Inputs/Encrypt";
const string DecryptDirectory = "Inputs/Decrypt";

Directory.CreateDirectory(EncryptDirectory);
Directory.CreateDirectory(DecryptDirectory);

Console.WriteLine("=== Cipher Playground ===");
Console.WriteLine($"Encryption inputs: {Path.GetFullPath(EncryptDirectory)}");
Console.WriteLine($"Decryption inputs: {Path.GetFullPath(DecryptDirectory)}");

var exitRequested = false;
while (!exitRequested)
{
	PrintMenu();
	var choice = ReadTrimmedLine();
	switch (choice)
	{
		case "1":
			InteractiveFolder(EncryptDirectory, encryptMode: true);
			break;
		case "2":
			InteractiveFolder(DecryptDirectory, encryptMode: false);
			break;
		case "3":
		case "q":
		case "Q":
			exitRequested = true;
			break;
		default:
			Console.WriteLine("Select 1, 2, 3 or Q to quit.");
			break;
	}
}

static void PrintMenu()
{
	Console.WriteLine();
	Console.WriteLine("Choose an option:");
	Console.WriteLine("  1) Encrypt files from Inputs/Encrypt");
	Console.WriteLine("  2) Decrypt files from Inputs/Decrypt");
	Console.WriteLine("  3) Quit");
	Console.Write("Selection: ");
}

static void InteractiveFolder(string directory, bool encryptMode)
{
	var files = Directory.GetFiles(directory, "*.txt");
	if (files.Length == 0)
	{
		Console.WriteLine($"No .txt files found in {Path.GetFullPath(directory)}");
		Console.WriteLine("Create a file using the documented header format and try again.");
		return;
	}

	while (true)
	{
		Console.WriteLine();
		Console.WriteLine($"{(encryptMode ? "Encryption" : "Decryption")} inputs available:");
		for (var i = 0; i < files.Length; i++)
		{
			Console.WriteLine($"  {i + 1}) {Path.GetFileName(files[i])}");
		}
		Console.WriteLine("  A) Run all files");
		Console.WriteLine("  B) Back to main menu");
		Console.Write("Selection: ");

		var selection = ReadTrimmedLine();
		if (string.IsNullOrEmpty(selection))
		{
			continue;
		}

		if (string.Equals(selection, "B", StringComparison.OrdinalIgnoreCase))
		{
			return;
		}

		if (string.Equals(selection, "A", StringComparison.OrdinalIgnoreCase))
		{
			foreach (var file in files)
			{
				RunScenarioFile(file, encryptMode);
			}
			continue;
		}

		if (int.TryParse(selection, NumberStyles.Integer, CultureInfo.InvariantCulture, out var index)
		    && index >= 1
		    && index <= files.Length)
		{
			RunScenarioFile(files[index - 1], encryptMode);
		}
		else
		{
			Console.WriteLine("Invalid selection. Try again.");
		}
	}
}

static void RunScenarioFile(string filePath, bool encryptMode)
{
	try
	{
		var scenario = ParseScenario(filePath);
		var cipher = CreateCipher(scenario);
		var result = encryptMode ? cipher.Encrypt(scenario.Text) : cipher.Decrypt(scenario.Text);

		Console.WriteLine();
		Console.WriteLine($"[{Path.GetFileName(filePath)}] {scenario.CipherName} {(encryptMode ? "encryption" : "decryption")}");
		Console.WriteLine($"Key: {scenario.Key}{(scenario.Filler is null ? string.Empty : $", filler: {scenario.Filler}")}");
		Console.WriteLine("Result:");
		Console.WriteLine(result);
	}
	catch (Exception ex)
	{
		Console.WriteLine($"Failed to process {Path.GetFileName(filePath)}: {ex.Message}");
	}
}

static Scenario ParseScenario(string filePath)
{
	var metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
	var textLines = new List<string>();
	var inTextBlock = false;

	foreach (var rawLine in File.ReadAllLines(filePath))
	{
		var trimmed = rawLine.Trim();
		if (!inTextBlock)
		{
			if (trimmed.StartsWith("#", StringComparison.Ordinal))
			{
				continue;
			}

			if (string.Equals(trimmed, "text:", StringComparison.OrdinalIgnoreCase))
			{
				inTextBlock = true;
				continue;
			}

			if (trimmed.Length == 0)
			{
				continue;
			}

			var parts = rawLine.Split(':', 2, StringSplitOptions.TrimEntries);
			if (parts.Length != 2)
			{
				throw new InvalidOperationException($"Invalid header line: '{rawLine}'");
			}

			if (parts[0].Equals("text", StringComparison.OrdinalIgnoreCase))
			{
				inTextBlock = true;
				if (parts[1].Length > 0)
				{
					textLines.Add(parts[1]);
				}
				continue;
			}

			metadata[parts[0]] = parts[1];
		}
		else
		{
			textLines.Add(rawLine);
		}
	}

	if (!metadata.TryGetValue("cipher", out var cipherName))
	{
		throw new InvalidOperationException("Missing 'cipher' header.");
	}

	if (!metadata.TryGetValue("key", out var keyValue) || string.IsNullOrWhiteSpace(keyValue))
	{
		throw new InvalidOperationException("Missing 'key' header.");
	}

	if (textLines.Count == 0)
	{
		throw new InvalidOperationException("Missing text block. Add 'text:' followed by the content to process.");
	}

	char? filler = null;
	if (metadata.TryGetValue("filler", out var fillerValue) && !string.IsNullOrWhiteSpace(fillerValue))
	{
		filler = char.ToUpperInvariant(fillerValue.Trim()[0]);
	}

	var text = string.Join(Environment.NewLine, textLines);
	return new Scenario(cipherName.Trim(), keyValue.Trim(), text, filler);
}

static Cipher CreateCipher(Scenario scenario)
{
	switch (scenario.CipherName.ToLowerInvariant())
	{
		case "caesar":
			if (!int.TryParse(scenario.Key, NumberStyles.Integer, CultureInfo.InvariantCulture, out var shift))
			{
				throw new InvalidOperationException("Caesar key must be an integer shift.");
			}
			return new CaesarCipher(shift);
		case "vigenere":
			return new VigenereCipher(scenario.Key);
		case "playfair":
			return new PlayfairCipher(scenario.Key, scenario.Filler ?? 'X');
		default:
			throw new InvalidOperationException($"Unsupported cipher '{scenario.CipherName}'.");
	}
}

static string? ReadTrimmedLine()
{
	return Console.ReadLine()?.Trim();
}

readonly record struct Scenario(string CipherName, string Key, string Text, char? Filler);
