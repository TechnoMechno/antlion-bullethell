using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AntLion.UI
{
    // Draws a short number string like "65/100" with pixel-art digit sprites, centred in this RectTransform.
    // Display only. Used instead of a font because the project has no pixel font.
    [RequireComponent(typeof(RectTransform))]
    public class PixelNumberText : MonoBehaviour
    {
        [Tooltip("Sprites for 0-9, then '/'. All the same size.")]
        [SerializeField] private Sprite[] glyphs = new Sprite[11];
        [Tooltip("Screen units per art pixel. Keep it a whole number so the pixels stay even.")]
        [SerializeField] private float pixelSize = 2f;
        [Tooltip("Neighbouring glyphs overlap by this many art pixels so their outlines share an edge, like the menu lettering.")]
        [SerializeField] private int overlap = 1;

        private readonly List<Image> images = new List<Image>();
        private string shown;

        public void SetText(string text)
        {
            if (text == shown) return;
            shown = text;

            // Turn the text into glyph indexes; characters without a glyph are skipped
            var indexes = new List<int>();
            foreach (char c in text)
            {
                if (c >= '0' && c <= '9') indexes.Add(c - '0');
                else if (c == '/') indexes.Add(10);
            }

            Rect glyphRect = glyphs[0].rect;
            float width = glyphRect.width * pixelSize;
            float height = glyphRect.height * pixelSize;
            float advance = (glyphRect.width - overlap) * pixelSize;
            float total = indexes.Count * advance + overlap * pixelSize;
            float left = -total / 2f;

            for (int i = 0; i < indexes.Count; i++)
            {
                Image image = GetImage(i);
                image.sprite = glyphs[indexes[i]];
                image.rectTransform.sizeDelta = new Vector2(width, height);
                image.rectTransform.anchoredPosition = new Vector2(left + i * advance, 0f);
                image.gameObject.SetActive(true);
            }

            // Hide glyphs left over from a longer number
            for (int i = indexes.Count; i < images.Count; i++) images[i].gameObject.SetActive(false);
        }

        // Reuses glyph Images; only makes a new one when the text is longer than anything shown before.
        private Image GetImage(int i)
        {
            if (i < images.Count) return images[i];

            var go = new GameObject("Glyph", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.layer = gameObject.layer;
            var rt = (RectTransform)go.transform;
            rt.SetParent(transform, false);
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0f, 0.5f);
            var image = go.GetComponent<Image>();
            image.raycastTarget = false;
            images.Add(image);
            return image;
        }
    }
}
