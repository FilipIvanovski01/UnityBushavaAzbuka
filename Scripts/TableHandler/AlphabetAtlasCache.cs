using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Loads <c>Resources.LoadAll&lt;Sprite&gt;("Alphabet/AlphabetAssets")</c> once and resolves glyphs by
/// slice name (single Cyrillic letter) or by numeric slice order matching <see cref="GridLetterOrder"/>.
/// </summary>
public static class AlphabetAtlasCache
{
    private const string AtlasResourcePath = "Alphabet/AlphabetAssets";

    /// <summary>Same order as <c>TableFactory.randomLetters</c> / atlas slice index when slices are named AlphabetAssets_N.</summary>
    private static readonly char[] GridLetterOrder =
    {
        'А', 'Б', 'В', 'Г', 'Д', 'Ѓ', 'Е', 'Ж', 'З', 'Ѕ', 'И', 'Ј',
        'К', 'Л', 'Љ', 'М', 'Н', 'Њ', 'О', 'П', 'Р', 'С', 'Т', 'Ќ',
        'У', 'Ф', 'Х', 'Ц', 'Ч', 'Џ', 'Ш'
    };

    private static bool loaded;
    private static bool warnedEmptyResources;
    private static Sprite[] rawSprites = Array.Empty<Sprite>();
    private static readonly Dictionary<char, Sprite> ByChar = new Dictionary<char, Sprite>();

    public static void EnsureLoaded()
    {
        if (loaded)
            return;

        loaded = true;
        rawSprites = Resources.LoadAll<Sprite>(AtlasResourcePath) ?? Array.Empty<Sprite>();
        if (rawSprites.Length == 0)
        {
            if (!warnedEmptyResources)
            {
                warnedEmptyResources = true;
                Debug.LogWarning(
                    $"AlphabetAtlasCache: no sprites at Resources/{AtlasResourcePath}. " +
                    "Add a sprite sheet or sprites under Assets/Resources/Alphabet/ (see Unity Resources rules). " +
                    "Letter cells will fall back to TMP text until sprites load.");
            }

            return;
        }

        foreach (Sprite s in rawSprites)
        {
            if (s == null || string.IsNullOrEmpty(s.name))
                continue;

            if (s.name.Length == 1)
            {
                char ch = char.ToUpperInvariant(s.name[0]);
                if (!ByChar.ContainsKey(ch))
                    ByChar[ch] = s;
            }
        }

        List<Sprite> sorted = rawSprites.Where(s => s != null).ToList();
        sorted.Sort((a, b) => ExtractTrailingIndex(a.name).CompareTo(ExtractTrailingIndex(b.name)));

        int n = Mathf.Min(sorted.Count, GridLetterOrder.Length);
        for (int i = 0; i < n; i++)
        {
            char ch = char.ToUpperInvariant(GridLetterOrder[i]);
            if (!ByChar.ContainsKey(ch))
                ByChar[ch] = sorted[i];
        }
    }

    public static Sprite TryGetGlyph(char letter)
    {
        EnsureLoaded();
        char key = char.ToUpperInvariant(letter);
        return ByChar.TryGetValue(key, out Sprite s) ? s : null;
    }

    private static int ExtractTrailingIndex(string spriteName)
    {
        int u = spriteName.LastIndexOf('_');
        if (u < 0 || u >= spriteName.Length - 1)
            return 0;

        string tail = spriteName.Substring(u + 1);
        return int.TryParse(tail, out int n) ? n : 0;
    }
}