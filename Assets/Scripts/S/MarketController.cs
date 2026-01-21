using UnityEngine;
using TMPro;

public class TradingController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Clicker clicker;
    [SerializeField] private BitcoinMarket market;

    [Header("Mode")]
    [SerializeField] private bool inBTC = false;

    private double btcHoldings = 0.0;

    [Header("UI (TMP)")]
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI btcText;
    [SerializeField] private TextMeshProUGUI nextMoveText;
    [SerializeField] private TextMeshProUGUI modeText;
    [SerializeField] private TextMeshProUGUI pointTxt; // CASH text

    void Start()
    {
        if (!clicker) clicker = FindFirstObjectByType<Clicker>();
        if (!market) market = FindFirstObjectByType<BitcoinMarket>();

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

    void Update()
    {
        // Clicker.Point her frame deðiþiyor (týklayýnca), sadece CASH yazýsýný güncelle
        if (clicker != null && pointTxt != null)
            pointTxt.text = $"CASH: {clicker.Point:0}";
    }

    void HandleTick()
    {
        RefreshUI();
    }

    public void Buy()
    {
        if (clicker == null || market == null) return;
        if (market.Price <= 0.0001f) return;

        double cash = clicker.Point;
        double bought = cash / market.Price;

        btcHoldings += bought;
        clicker.Point = 0;
        inBTC = true;

        RefreshUI();
    }

    public void Sell()
    {
        if (clicker == null || market == null) return;
        if (market.Price <= 0.0001f) return;

        double cash = btcHoldings * market.Price;

        if (cash < 0) cash = 0;
        clicker.Point = (ulong)cash;
        btcHoldings = 0.0;

        inBTC = false;
        RefreshUI();
    }

    void RefreshUI()
    {
        if (clicker == null || market == null) return;

        if (priceText) priceText.text = $"BTC PRICE: {market.Price:0}";
        if (btcText) btcText.text = $"BTC: {btcHoldings:0.0000}";

        float p = market.NextPercent() * 100f;
        if (nextMoveText) nextMoveText.text = $"NEXT: {(p >= 0 ? "+" : "")}{p:0.0}%";

        if (modeText) modeText.text = inBTC ? "MODE: BTC" : "MODE: CASH";

        // Ýlk açýlýþta da CASH yazsýn
        if (pointTxt) pointTxt.text = $"CASH: {clicker.Point:0}";
    }
}
