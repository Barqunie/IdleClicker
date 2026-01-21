using UnityEngine;
using TMPro;
using System.Collections;

public class TradingController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Clicker clicker;
    [SerializeField] private BitcoinMarket market;

    private double btcHoldings = 0.0;

    [Header("Trade Amount (CASH units)")]
    [SerializeField] private ulong selectedAmount = 0; // 0 => ALL-IN

    [Header("Feedback")]
    [SerializeField] private float feedbackDuration = 2f;
    private Coroutine feedbackCo;

    [Header("UI (TMP)")]
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI btcText;
    [SerializeField] private TextMeshProUGUI nextMoveText;
    [SerializeField] private TextMeshProUGUI modeText;
    [SerializeField] private TextMeshProUGUI pointTxt;            // CASH
    [SerializeField] private TextMeshProUGUI selectedAmountText;  // AMOUNT
    [SerializeField] private TextMeshProUGUI feedbackText;        // FEEDBACK (hidden by default)

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
            pointTxt.text = $"CASH: {clicker.Point:0}";

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
        // clamp YOK: sýnýrsýz biriksin (overflow korumasý var)
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
        clicker.Point = cash - spend;

        ShowFeedback($"+{boughtBtc:0.000000} BTC");
        RefreshUI();
    }

    public void Sell()
    {
        if (clicker == null || market == null) return;
        if (market.Price <= 0.0001f) return;

        // ALL-IN => tüm BTC sat
        if (selectedAmount == 0)
        {
            if (btcHoldings <= 0.0)
            {
                ShowFeedback("NO BTC");
                return;
            }

            double cashOut = btcHoldings * market.Price;
            if (cashOut < 0) cashOut = 0;
            if (cashOut > ulong.MaxValue) cashOut = ulong.MaxValue;

            clicker.Point += (ulong)cashOut;
            btcHoldings = 0.0;

            ShowFeedback($"SOLD ALL: {(ulong)cashOut:0} CASH");
            RefreshUI();
            return;
        }

        // selectedAmount kadar CASH almak için gereken BTC
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

        btcHoldings -= needBtc;
        clicker.Point += selectedAmount;

        ShowFeedback($"-{needBtc:0.000000} BTC");
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

        if (pointTxt) pointTxt.text = $"CASH: {clicker.Point:0}";

        UpdateAmountText();
    }

    void UpdateAmountText()
    {
        if (selectedAmountText == null) return;

        selectedAmountText.text = selectedAmount == 0
            ? "AMOUNT: ALL IN"
            : $"AMOUNT: {selectedAmount:0}";
    }

    // ---------- Feedback (show for 2s then hide) ----------
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
}
