using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TradingController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Clicker clicker;
    [SerializeField] private BitcoinMarket market;

    [Header("Mode")]
    [SerializeField] private bool inBTC = false;

    [Header("UI (TMP)")]
    [SerializeField] private TextMeshProUGUI priceText;       // BTC PRICE
    [SerializeField] private TextMeshProUGUI btcText;         // BTC: 0.0000 (display)
    [SerializeField] private TextMeshProUGUI nextMoveText;    // NEXT: +12.0%
    [SerializeField] private TextMeshProUGUI modeText;        // MODE: BTC/CASH

    void Start()
    {
        if (!clicker) clicker = FindFirstObjectByType<Clicker>();
        RefreshUI();
    }

    void OnEnable()
    {
        if (market != null) market.OnTick += HandleTick;
    }

    void OnDisable()
    {
        if (market != null) market.OnTick -= HandleTick;
    }

    void HandleTick()
    {
        if (clicker == null || market == null) return;

        // BTC modundaysan: net worth (Point) market oranýyla güncellenir
        if (inBTC && market.PrevPrice > 0.0001f)
        {
            double ratio = market.Price / market.PrevPrice;
            double nw = clicker.Point;
            nw *= ratio;

            if (nw < 0) nw = 0;
            clicker.Point = (ulong)nw;
        }

        RefreshUI();
    }

    public void Buy()
    {
        inBTC = true;
        RefreshUI();
    }

    public void Sell()
    {
        inBTC = false;
        RefreshUI();
    }

    void RefreshUI()
    {
        if (clicker == null || market == null) return;

        if (priceText) priceText.text = $"BTC PRICE: {market.Price:0}";

        // BTC display = NetWorth / Price
        double btc = (market.Price > 0.0001f) ? (clicker.Point / (double)market.Price) : 0.0;
        if (btcText) btcText.text = $"BTC: {btc:0.0000}";

        float p = market.NextPercent() * 100f;
        if (nextMoveText) nextMoveText.text = $"NEXT: {(p >= 0 ? "+" : "")}{p:0.0}%";

        if (modeText) modeText.text = inBTC ? "MODE: BTC" : "MODE: CASH";
    }
}

