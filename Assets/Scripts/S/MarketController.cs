using UnityEngine;
using TMPro;
using System.Collections;

public class TradingController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Clicker clicker;
    [SerializeField] private BitcoinMarket market;

    private double btcHoldings = 0.0;

    // ---- NEW: Cost basis + realized P/L (CASH units) ----
    private double costBasisCash = 0.0;     // þu an elde tuttuðun BTC'nin toplam maliyeti (cash)
    private double realizedPnLCash = 0.0;   // toplam realize kâr/zarar (cash)

    [Header("Trade Amount (CASH units)")]
    [SerializeField] private ulong selectedAmount = 0; // 0 => ALL-IN

    [Header("Feedback")]
    [SerializeField] private float feedbackDuration = 6f;
    private Coroutine feedbackCo;

    [Header("UI (TMP)")]
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI btcText;
    [SerializeField] private TextMeshProUGUI nextMoveText;
    [SerializeField] private TextMeshProUGUI modeText;
    [SerializeField] private TextMeshProUGUI pointTxt;            // CASH
    [SerializeField] private TextMeshProUGUI selectedAmountText;  // AMOUNT
    [SerializeField] private TextMeshProUGUI feedbackText;        // FEEDBACK 

    void Start()
    {
        if (!clicker) clicker = FindFirstObjectByType<Clicker>();
        if (!market) market = FindFirstObjectByType<BitcoinMarket>();

        HideFeedback();
        RefreshUI();
    }

    void OnEnable()
    {
        if (market != null) market.OnTick += HandleTick;

        // Market her açýldýðýnda ALL IN
        selectedAmount = 0;

        HideFeedback();
        RefreshUI();
    }

    void OnDisable()
    {
        if (market != null) market.OnTick -= HandleTick;
    }

    void Update()
    {
        if (clicker != null && pointTxt != null)
            pointTxt.text = $" {clicker.Point:0}";

        UpdateAmountText();
    }

    void HandleTick() => RefreshUI();

    // ---------- Amount Buttons ----------
    public void AddAmount100() => AddAmount(100);
    public void AddAmount1000() => AddAmount(1000);

    public void ResetAmount()
    {
        selectedAmount = 0;
        HideFeedback();
        RefreshUI();
    }

    public void MaxAmount()
    {
        if (clicker == null) return;
        selectedAmount = clicker.Point;
        HideFeedback();
        RefreshUI();
    }

    void AddAmount(ulong add)
    {
      
        ulong before = selectedAmount;
        selectedAmount += add;
        if (selectedAmount < before) selectedAmount = ulong.MaxValue;

        HideFeedback();
        RefreshUI();
    }

    // ---------- Trading ----------
    public void Buy()
    {
        if (clicker == null || market == null) return;
        if (market.Price <= 0.0001f) return;

        ulong cash = clicker.Point;
        ulong spend = (selectedAmount == 0) ? cash : selectedAmount;

        if (spend == 0)
        {
            ShowFeedback("NO CASH");
            return;
        }

        if (spend > cash)
        {
            ShowFeedback("INSUFFICIENT FUNDS");
            return;
        }

        double boughtBtc = spend / market.Price;

        btcHoldings += boughtBtc;

      
        costBasisCash += spend;

        clicker.Point = cash - spend;

      
        double avgEntry = (btcHoldings > 0.0) ? (costBasisCash / btcHoldings) : 0.0;
        ShowFeedback($"+{boughtBtc:0.000000} BTC | AVG: {avgEntry:0}");

        RefreshUI();
    }

    public void Sell()
    {
        if (clicker == null || market == null) return;
        if (market.Price <= 0.0001f) return;

        if (btcHoldings <= 0.0)
        {
            ShowFeedback("NO BTC");
            return;
        }

        // ALL-IN => 
        if (selectedAmount == 0)
        {
            double cashOut = btcHoldings * market.Price;
            ulong cashOutU = ToUlongClamped(cashOut);

            // realize P/L = satýþ geliri - maliyet
            double pnl = (double)cashOutU - costBasisCash;
            realizedPnLCash += pnl;

            SafeAddCash(cashOutU);

            btcHoldings = 0.0;
            costBasisCash = 0.0;

            long pnlInt = (long)System.Math.Round(pnl);
            long totalInt = (long)System.Math.Round(realizedPnLCash);

            ShowFeedback($"SOLD ALL: {cashOutU:0} CASH | P/L {Signed(pnlInt)} | TOTAL {Signed(totalInt)}");
            RefreshUI();
            return;
        }

        // selectedAmount kadar CASH almak için gereken BTC
        double btcBefore = btcHoldings;
        double costBefore = costBasisCash;

        double needBtc = selectedAmount / market.Price;

        if (needBtc <= 0.0)
        {
            ShowFeedback("INVALID AMOUNT");
            return;
        }

        if (needBtc > btcHoldings)
        {
            ShowFeedback("INSUFFICIENT BTC");
            return;
        }

        // satýlan BTC'nin maliyeti
        double proportion = needBtc / btcBefore;
        double costSold = costBefore * proportion;

        // eldekini düþ
        btcHoldings -= needBtc;
        costBasisCash -= costSold;

        // kasa ekle 
        SafeAddCash(selectedAmount);

        // realize P/L
        double pnl2 = (double)selectedAmount - costSold;
        realizedPnLCash += pnl2;

        // minik floating taþmasý olursa
        if (btcHoldings < 0) btcHoldings = 0;
        if (costBasisCash < 0) costBasisCash = 0;

        long pnlInt2 = (long)System.Math.Round(pnl2);
        long totalInt2 = (long)System.Math.Round(realizedPnLCash);

        ShowFeedback($"SOLD: {selectedAmount:0} CASH | P/L {Signed(pnlInt2)} | TOTAL {Signed(totalInt2)}");
        RefreshUI();
    }

    // ---------- UI ----------
    void RefreshUI()
    {
        if (clicker == null || market == null) return;

        if (priceText) priceText.text = $"BTC PRICE: {market.Price:0}";
        if (btcText) btcText.text = $"BTC: {btcHoldings:0.0000}";

        float p = market.NextPercent() * 100f;
        if (nextMoveText) nextMoveText.text = $"NEXT: {(p >= 0 ? "+" : "")}{p:0.0}%";

        if (modeText)
            modeText.text = selectedAmount == 0 ? "MODE: ALL IN" : "MODE: MANUAL";

        if (pointTxt) pointTxt.text = $" {clicker.Point:0}";

        UpdateAmountText();
    }

    void UpdateAmountText()
    {
        if (selectedAmountText == null) return;

        selectedAmountText.text = selectedAmount == 0
            ? "AMOUNT: ALL IN"
            : $"AMOUNT: {selectedAmount:0}";
    }

    // ---------- Feedback ----------
    void HideFeedback()
    {
        if (feedbackCo != null)
        {
            StopCoroutine(feedbackCo);
            feedbackCo = null;
        }

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);
    }

    void ShowFeedback(string msg)
    {
        if (feedbackText == null) return;

        if (feedbackCo != null)
            StopCoroutine(feedbackCo);

        feedbackCo = StartCoroutine(FeedbackRoutine(msg));
    }

    IEnumerator FeedbackRoutine(string msg)
    {
        feedbackText.gameObject.SetActive(true);
        feedbackText.text = msg;

        yield return new WaitForSeconds(feedbackDuration);

        feedbackText.gameObject.SetActive(false);
        feedbackCo = null;
    }

    // ---------- Helpers ----------
    ulong ToUlongClamped(double v)
    {
        if (v <= 0.0) return 0;
        if (v >= ulong.MaxValue) return ulong.MaxValue;
        return (ulong)v;
    }

    void SafeAddCash(ulong amount)
    {
        if (clicker == null) return;
        ulong before = clicker.Point;
        clicker.Point += amount;
        if (clicker.Point < before) clicker.Point = ulong.MaxValue; // overflow guard
    }

    string Signed(long v) => (v >= 0) ? $"+{v}" : v.ToString();
}
