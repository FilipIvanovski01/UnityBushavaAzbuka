using System;
using System.Collections.Generic;
using UnityEngine;

public static class WordSelectionService
{
    private static readonly char[] randomLetters =
    {
        'А','Б','В','Г','Д','Ѓ','Е','Ж','З','Ѕ','И','Ј',
        'К','Л','Љ','М','Н','Њ','О','П','Р','С','Т','Ќ',
        'У','Ф','Х','Ц','Ч','Џ','Ш'
    };

    public static string[] SelectAllWordsForLetter(string letter)
    {
        if (string.IsNullOrWhiteSpace(letter))
            return Array.Empty<string>();
        string letterKey = letter.Trim().ToUpperInvariant();
        WordLoading.LoadByLetter(letterKey);
        IReadOnlyList<string> letterWords = WordLoading.GetChosenLetterWords();
        if (letterWords.Count == 0)
            return Array.Empty<string>();
        List<string> picked = GetRandomWords(letterWords, 6);
        return picked.ToArray();
    }
    // Entry point: call this from TableFactory.Start()
    public static string[] SelectWordsForPuzzle()
    {
        // Decide how many letters and how many words per letter
        int randomNum = UnityEngine.Random.Range(1, 7);
        (int numLetters, int wordsPerLetter) = GetWordDistribution(randomNum);

        // Shuffle letters so we get random letters each time
        List<char> shuffledLetters = new List<char>(randomLetters);
        Shuffle(shuffledLetters);

        List<string> allSelectedWords = new List<string>();

        for (int i = 0; i < numLetters && i < shuffledLetters.Count; i++)
        {
            string letterKey = shuffledLetters[i].ToString().ToUpperInvariant();

            // Ask WordLoading for words of this letter
            WordLoading.LoadByLetter(letterKey);
            IReadOnlyList<string> letterWords = WordLoading.GetChosenLetterWords();

            if (letterWords.Count == 0)
                continue;

            // For now: no difficulty, just random words
            List<string> picked = GetRandomWords(letterWords, wordsPerLetter);
            allSelectedWords.AddRange(picked);
        }

        return allSelectedWords.ToArray();
    }

    private static (int numLetters, int wordsPerLetter) GetWordDistribution(int randomNum)
    {
        switch (randomNum)
        {
            case 1: return (1, 6);
            case 2: return (2, 3);
            case 3:
            case 4:
            case 5: return (3, 2);
            case 6: return (6, 1);
            default: return (3, 2);
        }
    }

    private static void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    private static List<string> GetRandomWords(IReadOnlyList<string> source, int amount)
    {
        List<string> shuffled = new List<string>(source);
        Shuffle(shuffled);

        int wordsToTake = Mathf.Min(amount, shuffled.Count);
        return shuffled.GetRange(0, wordsToTake);
    }
}