using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChartTexture : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private CoinMarket market;
    [SerializeField] private RawImage img;
    [SerializeField] private TMP_Text valueText; // optional (crosshair value)

    [Header("Y Axis Labels (TMP)")]
    [SerializeField] private TMP_Text yTopText;
    [SerializeField] private TMP_Text yMidText;
    [SerializeField] private TMP_Text yBottomText;

    [Header("Texture")]
    [SerializeField] private int texWidth = 640;
    [SerializeField] private int texHeight = 360;
    [SerializeField] private int padding = 16;

    [Header("Style")]
    [SerializeField] private int lineThickness = 3;
    [SerializeField] private int fillAlpha = 60;     // 0-255
    [SerializeField] private int gridAlpha = 30;     // 0-255
    [SerializeField] private bool drawGrid = true;
    [SerializeField] private bool drawCrosshair = true;

    Texture2D tex;
    Color32[] clearPixels;

    void Awake()
    {
        if (!img) img = GetComponent<RawImage>();

        tex = new Texture2D(texWidth, texHeight, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;
        img.texture = tex;

        clearPixels = new Color32[texWidth * texHeight];
        for (int i = 0; i < clearPixels.Length; i++) clearPixels[i] = new Color32(0, 0, 0, 0);

        Clear();
        tex.Apply();
    }

    void Update()
    {
        Redraw();

        if (drawCrosshair && valueText != null)
            UpdateCrosshairText();
    }

    void Clear() => tex.SetPixels32(clearPixels);

    void Redraw()
    {
        if (market == null || market.History == null || market.History.Count < 2)
        {
            Clear(); tex.Apply(); return;
        }

        Clear();

        int w = texWidth;
        int h = texHeight;
        int plotW = w - padding * 2;
        int plotH = h - padding * 2;

        List<float> data = market.History;

        float min = float.MaxValue, max = float.MinValue;
        for (int i = 0; i < data.Count; i++)
        {
            float v = data[i];
            if (v < min) min = v;
            if (v > max) max = v;
        }
        if (Mathf.Approximately(min, max)) max = min + 1f;

        //  Y-axis label’larý yaz
        float mid = (min + max) * 0.5f;
        if (yTopText) yTopText.SetText($"{max:0,0}");
        if (yMidText) yMidText.SetText($"{mid:0,0}");
        if (yBottomText) yBottomText.SetText($"{min:0,0}");

        // Trend rengi: yeþil / kýrmýzý (son deðer önceki deðerden büyükse yeþil daha mantýklý)
        bool up = data[data.Count - 1] >= data[data.Count - 2];
        Color32 lineColor = up ? new Color32(90, 255, 160, 255) : new Color32(255, 90, 120, 255);
        Color32 fillColor = new Color32(lineColor.r, lineColor.g, lineColor.b, (byte)fillAlpha);

        if (drawGrid)
            DrawGrid(padding, padding, plotW, plotH);

        // noktalarý hesapla
        Vector2Int[] pts = new Vector2Int[data.Count];
        for (int i = 0; i < data.Count; i++)
        {
            float t = (data[i] - min) / (max - min);
            int x = padding + Mathf.RoundToInt(plotW * (i / (float)(data.Count - 1)));
            int y = padding + Mathf.RoundToInt(plotH * t);
            pts[i] = new Vector2Int(x, y);
        }

        // fill
        for (int i = 0; i < pts.Length; i++)
        {
            int x = pts[i].x;
            int yTop = pts[i].y;
            for (int y = padding; y <= yTop; y++)
                SetPixelSafe(x, y, fillColor);
        }

        // line
        for (int i = 1; i < pts.Length; i++)
            DrawLine(pts[i - 1].x, pts[i - 1].y, pts[i].x, pts[i].y, lineThickness, lineColor);

        tex.Apply();
    }

    void DrawGrid(int x0, int y0, int w, int h)
    {
        Color32 grid = new Color32(255, 255, 255, (byte)gridAlpha);
        int vLines = 6;
        int hLines = 4;

        for (int i = 0; i <= vLines; i++)
        {
            int x = x0 + Mathf.RoundToInt(w * (i / (float)vLines));
            DrawLine(x, y0, x, y0 + h, 1, grid);
        }
        for (int i = 0; i <= hLines; i++)
        {
            int y = y0 + Mathf.RoundToInt(h * (i / (float)hLines));
            DrawLine(x0, y, x0 + w, y, 1, grid);
        }
    }

    void UpdateCrosshairText()
    {
        Vector2 screen = Input.mousePosition;

        RectTransform rt = img.rectTransform;
        if (!RectTransformUtility.RectangleContainsScreenPoint(rt, screen)) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, screen, null, out Vector2 local);
        Rect rect = rt.rect;

        float u = Mathf.InverseLerp(rect.xMin, rect.xMax, local.x);
        int idx = Mathf.Clamp(Mathf.RoundToInt(u * (market.History.Count - 1)), 0, market.History.Count - 1);
        float val = market.History[idx];

        valueText.SetText($"${val:0,0.##}");
    }

    void SetPixelSafe(int x, int y, Color32 c)
    {
        if (x < 0 || x >= texWidth || y < 0 || y >= texHeight) return;
        tex.SetPixel(x, y, c);
    }

    void DrawLine(int x0, int y0, int x1, int y1, int thick, Color32 col)
    {
        int dx = Mathf.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        int dy = -Mathf.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        int err = dx + dy;

        while (true)
        {
            DrawDot(x0, y0, thick, col);
            if (x0 == x1 && y0 == y1) break;
            int e2 = 2 * err;
            if (e2 >= dy) { err += dy; x0 += sx; }
            if (e2 <= dx) { err += dx; y0 += sy; }
        }
    }

    void DrawDot(int cx, int cy, int r, Color32 col)
    {
        for (int y = -r; y <= r; y++)
            for (int x = -r; x <= r; x++)
            {
                int px = cx + x;
                int py = cy + y;
                if (px < 0 || px >= texWidth || py < 0 || py >= texHeight) continue;
                tex.SetPixel(px, py, col);
            }
    }
}
