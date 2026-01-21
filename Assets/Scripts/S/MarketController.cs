using UnityEngine;
using TMPro;

public class TradingController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Clicker clicker;
    [SerializeField] private BitcoinMarket market;

    [Header("Mode")]
    [SerializeField] private bool inBTC = false;

    // Gerçek BTC miktarý burada durur (sabit kalacak olan bu)
    private double btcHoldings = 0.0;

    [Header("UI (TMP)")]
    [SerializeField] private TextMeshProUGUI priceText;       // BTC PRICE
    [SerializeField] private TextMeshProUGUI btcText;         // BTC: 0.0000
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
        // Fiyat deðiþince para/btc ÇEVÝRME YOK.
        // Sadece UI güncelle.
        RefreshUI();
    }

    public void Buy()
    {
        if (clicker == null || market == null) return;
        if (market.Price <= 0.0001f) return;

        // Tüm cash ile BTC al (istersen sonra kýsmi miktar ekleriz)
        double cash = clicker.Point;               // cash (ulong) -> double
        double bought = cash / market.Price;       // BTC miktarý

        btcHoldings += bought;
        clicker.Point = 0;                         // cash bitti
        inBTC = true;

        RefreshUI();
    }

    public void Sell()
    {
        if (clicker == null || market == null) return;
        if (market.Price <= 0.0001f) return;

        // Tüm BTC sat -> cash
        double cash = btcHoldings * market.Price;

        if (cash < 0) cash = 0;
        clicker.Point = (ulong)cash;   // küsurat kaybolur (istersen ayrý cashDouble tutarýz)
        btcHoldings = 0.0;

        inBTC = false;
        RefreshUI();
    }

    void RefreshUI()
    {
        if (clicker == null || market == null) return;

        if (priceText) priceText.text = $"BTC PRICE: {market.Price:0}";

        // BTC miktarý artýk "bölme ile türetilmiyor", direkt holding gösteriliyor
        if (btcText) btcText.text = $"BTC: {btcHoldings:0.0000}";

        float p = market.NextPercent() * 100f;
        if (nextMoveText) nextMoveText.text = $"NEXT: {(p >= 0 ? "+" : "")}{p:0.0}%";

        if (modeText) modeText.text = inBTC ? "MODE: BTC" : "MODE: CASH";
    }
}
