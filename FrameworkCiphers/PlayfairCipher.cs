using System;
using System.Collections.Generic;
using System.Text;

namespace FrameworkCiphers;
public class PlayfairCipher : Cipher
{
    private readonly char[,] _matrix = new char[5, 5];
    private readonly Dictionary<char, (int Row, int Col)> _positions = new();
    private readonly char _filler;

    public PlayfairCipher(string key, char filler = 'X')
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Key cannot be null or empty.", nameof(key));
        }

        if (!char.IsLetter(filler))
        {
            throw new ArgumentException("Filler must be an alphabetic character.", nameof(filler));
        }

        var normalizedFiller = char.ToUpperInvariant(filler);
        _filler = normalizedFiller == 'J' ? 'X' : normalizedFiller;

        BuildMatrix(key);
    }

    public override string Encrypt(string plaintext)
    {
        if (plaintext is null)
        {
            throw new ArgumentNullException(nameof(plaintext));
        }

        var digraphs = BuildDigraphs(plaintext);
        if (digraphs.Count == 0)
        {
            return string.Empty;
        }

        return ProcessPairs(digraphs, 1);
    }

    public override string Decrypt(string text)
    {
        throw new NotImplementedException();
    }

    private void BuildMatrix(string key)
    {
        var seen = new HashSet<char>();
        var sequence = new List<char>(25);

        foreach (var raw in key.ToUpperInvariant())
        {
            if (!char.IsLetter(raw))
            {
                continue;
            }

            var normalized = raw == 'J' ? 'I' : raw;
            if (seen.Add(normalized))
            {
                sequence.Add(normalized);
            }
        }

        for (var letter = 'A'; letter <= 'Z'; letter++)
        {
            if (letter == 'J')
            {
                continue;
            }

            if (seen.Add(letter))
            {
                sequence.Add(letter);
            }
        }

        for (var i = 0; i < 25; i++)
        {
            var letter = sequence[i];
            var row = i / 5;
            var col = i % 5;
            _matrix[row, col] = letter;
            _positions[letter] = (row, col);
        }
    }

    private List<(char First, char Second)> BuildDigraphs(string input)
    {
        var cleaned = CleanInput(input);
        var pairs = new List<(char, char)>();

        var index = 0;
        while (index < cleaned.Length)
        {
            var first = cleaned[index];
            char second;

            if (index + 1 >= cleaned.Length)
            {
                second = FillerFor(first);
                index++;
            }
            else if (cleaned[index + 1] == first)
            {
                second = FillerFor(first);
                index++;
            }
            else
            {
                second = cleaned[index + 1];
                index += 2;
            }

            pairs.Add((first, second));
        }

        return pairs;
    }

    private string ProcessPairs(IEnumerable<(char First, char Second)> pairs, int direction)
    {
        var builder = new StringBuilder();

        foreach (var (first, second) in pairs)
        {
            var positionA = _positions[first];
            var positionB = _positions[second];

            if (positionA.Row == positionB.Row)
            {
                var colA = (positionA.Col + direction + 5) % 5;
                var colB = (positionB.Col + direction + 5) % 5;
                builder.Append(_matrix[positionA.Row, colA]);
                builder.Append(_matrix[positionB.Row, colB]);
            }
            else if (positionA.Col == positionB.Col)
            {
                var rowA = (positionA.Row + direction + 5) % 5;
                var rowB = (positionB.Row + direction + 5) % 5;
                builder.Append(_matrix[rowA, positionA.Col]);
                builder.Append(_matrix[rowB, positionB.Col]);
            }
            else
            {
                builder.Append(_matrix[positionA.Row, positionB.Col]);
                builder.Append(_matrix[positionB.Row, positionA.Col]);
            }
        }

        return builder.ToString();
    }

    private string CleanInput(string input)
    {
        var builder = new StringBuilder();

        foreach (var raw in input.ToUpperInvariant())
        {
            if (!char.IsLetter(raw))
            {
                continue;
            }

            builder.Append(raw == 'J' ? 'I' : raw);
        }

        return builder.ToString();
    }

    private char FillerFor(char reference)
    {
        if (reference != _filler)
        {
            return _filler;
        }

        return reference == 'X' ? 'Z' : 'X';
    }
}
