using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineChart : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private BitcoinMarket market;     // senin market scriptin
    [SerializeField] private RectTransform chartArea;  // PriceChart'ýn RectTransform'u

    [Header("Chart Settings")]
    [SerializeField] private int maxPoints = 120;
    [SerializeField] private float padding = 10f;

    private LineRenderer line;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.useWorldSpace = false;

        if (!chartArea) chartArea = transform as RectTransform;
    }

    void OnEnable()
    {
        // market scriptinde event yoksa bile Update ile çizdireceðiz
        // Eðer OnPriceChanged event eklediysen þuraya baðlayabiliriz:
        // market.OnPriceChanged += _ => Redraw();
    }

    void Update()
    {
        // Þimdilik en kolayý: her frame deðil, market tick'inde çizmek daha iyi.
        // Ama hýzlý prototip için:
        Redraw();
    }

    public void Redraw()
    {
        if (market == null || market.history == null || market.history.Count < 2) return;

        // son maxPoints kadar al
        int count = Mathf.Min(maxPoints, market.history.Count);
        int startIndex = market.history.Count - count;

        float min = float.MaxValue;
        float max = float.MinValue;

        for (int i = 0; i < count; i++)
        {
            float v = market.history[startIndex + i];
            if (v < min) min = v;
            if (v > max) max = v;
        }

        // ayný olmasýn diye
        if (Mathf.Approximately(min, max))
        {
            max = min + 1f;
        }

        float width = chartArea.rect.width - padding * 2f;
        float height = chartArea.rect.height - padding * 2f;

        line.positionCount = count;

        for (int i = 0; i < count; i++)
        {
            float price = market.history[startIndex + i];
            float t = (price - min) / (max - min); // 0..1

            float x = padding + (width * (i / (float)(count - 1)));
            float y = padding + (height * t);

            line.SetPosition(i, new Vector3(x, y, 0f));
        }
    }
}
