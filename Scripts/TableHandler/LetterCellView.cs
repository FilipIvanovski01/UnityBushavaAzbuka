using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LetterCellView : MonoBehaviour
{
    private const string GlyphChildName = "LetterGlyph";

    private Image ringImage;
    private Image glyphImage;
    [SerializeField] private TMP_Text letterText;

    public int Row { get; private set; }
    public int Col { get; private set; }

    private char letterValue;

    private Color defaultRingColor;
    private Color defaultTextColor;

    private static Sprite sharedCircleSprite;

    private static readonly Color IdleRingColor = new Color(0.28f, 0.42f, 0.72f, 1f);
    private static readonly Color SelectingColor = new Color(0.75f, 0.75f, 0.75f, 1f);
    private static readonly Color SuccessColor = new Color(0.4f, 0.85f, 0.45f, 1f);
    private static readonly Color FailColor = new Color(0.95f, 0.35f, 0.35f, 1f);

    private void Awake()
    {
        EnsureSharedCircleSprite();

        ringImage = GetComponent<Image>();
        if (ringImage == null)
            ringImage = gameObject.AddComponent<Image>();

        ringImage.sprite = sharedCircleSprite;
        ringImage.type = Image.Type.Simple;
        ringImage.preserveAspect = true;
        ringImage.raycastTarget = false;
        ringImage.color = IdleRingColor;
        defaultRingColor = IdleRingColor;

        EnsureGlyphImage();

        if (letterText == null)
            letterText = GetComponentInChildren<TMP_Text>();

        if (letterText != null)
            letterText.rectTransform.SetAsLastSibling();

        if (glyphImage != null)
        {
            glyphImage.preserveAspect = true;
            glyphImage.raycastTarget = true;
            glyphImage.color = Color.white;
        }

        if (letterText != null)
            defaultTextColor = letterText.color;

        var button = GetComponent<Button>();
        if (button != null && glyphImage != null)
            button.targetGraphic = glyphImage;
    }

    private void EnsureGlyphImage()
    {
        Transform existing = transform.Find(GlyphChildName);
        if (existing != null)
        {
            glyphImage = existing.GetComponent<Image>();
            if (glyphImage != null)
                return;
        }

        var go = new GameObject(GlyphChildName, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(transform, false);

        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;

        glyphImage = go.GetComponent<Image>();
        go.transform.SetAsLastSibling();
    }

    private static void EnsureSharedCircleSprite()
    {
        if (sharedCircleSprite != null)
            return;

        const int size = 128;
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;

        float radius = size * 0.48f;
        var center = new Vector2(size * 0.5f, size * 0.5f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), center);
                float alpha = d <= radius ? 1f : Mathf.Clamp01(1f - (d - radius) / 2.5f);
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        tex.Apply();
        sharedCircleSprite = Sprite.Create(
            tex,
            new Rect(0, 0, size, size),
            new Vector2(0.5f, 0.5f),
            100f,
            0,
            SpriteMeshType.FullRect);
    }

    public void Configure(int row, int col, char letter)
    {
        Row = row;
        Col = col;
        letterValue = letter;

        Sprite glyph = AlphabetAtlasCache.TryGetGlyph(letter);

        if (glyphImage != null)
            glyphImage.sprite = glyph;

        if (letterText != null)
            letterText.text = glyph != null ? string.Empty : letter.ToString();

        if (ringImage != null)
            ringImage.color = IdleRingColor;
    }

    public char GetLetter()
    {
        return letterValue == '\0' ? '\0' : letterValue;
    }

    public void SetHighlightSelecting()
    {
        if (ringImage != null)
            ringImage.color = SelectingColor;
    }

    public void SetResultSuccess()
    {
        if (ringImage != null)
            ringImage.color = SuccessColor;
    }

    public void SetResultFail()
    {
        if (ringImage != null)
            ringImage.color = FailColor;
    }

    public void ResetDefault()
    {
        if (ringImage != null)
            ringImage.color = defaultRingColor;

        if (letterText != null)
            letterText.color = defaultTextColor;
    }
}
