using System;
using System.Collections.Generic;
using UnityEngine;

public static class WordLoading
{
    private const string DictionaryResourceFolder = "Dictionary";

    // All words across all letters
    private static readonly HashSet<string> allWords = new HashSet<string>();

    // Words for the last loaded letter
    private static readonly List<string> chosenLetterWords = new List<string>();

    public static void LoadAllWords()
    {
        allWords.Clear();

        TextAsset[] files = Resources.LoadAll<TextAsset>(DictionaryResourceFolder);
        if (files == null || files.Length == 0)
        {
            Debug.LogError($"No dictionary files found in Resources/{DictionaryResourceFolder}");
            return;
        }

        foreach (TextAsset file in files)
        {
            string[] lines = file.text.Split(
                new[] { '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries
            );

            foreach (string line in lines)
            {
                string normalized = line.Trim().ToUpperInvariant();
                if (!string.IsNullOrWhiteSpace(normalized))
                {
                    allWords.Add(normalized);
                }
            }
        }
    }

    public static void LoadByLetter(string letter)
    {
        if (string.IsNullOrWhiteSpace(letter))
            return;

        string letterKey = letter.Trim().ToUpperInvariant();

        chosenLetterWords.Clear();

        // Directly load the TextAsset with that letter name: e.g., Dictionary/A
        string resourcePath = $"{DictionaryResourceFolder}/{letterKey}";
        TextAsset textAsset = Resources.Load<TextAsset>(resourcePath);
        if (textAsset == null)
        {
            Debug.LogError($"No dictionary file found for letter '{letterKey}' at Resources/{resourcePath}");
            return;
        }

        string[] lines = textAsset.text.Split(
            new[] { '\r', '\n' },
            StringSplitOptions.RemoveEmptyEntries
        );

        foreach (string line in lines)
        {
            string normalized = line.Trim().ToUpperInvariant();
            if (!string.IsNullOrWhiteSpace(normalized))
            {
                chosenLetterWords.Add(normalized);
                allWords.Add(normalized); // optional: keep global set synced
            }
        }
    }

    // Returns a read-only view to avoid external modifications
    public static IReadOnlyList<string> GetChosenLetterWords()
    {
        return chosenLetterWords;
    }

    public static HashSet<string> GetAllWords()
    {
        return allWords;
    }
}