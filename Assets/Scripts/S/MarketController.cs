using UnityEngine;
using TMPro;

public class TradingController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Clicker clicker;
    [SerializeField] private BitcoinMarket market;

    private double btcHoldings = 0.0;

    [Header("Trade Amount (CASH units)")]
    [SerializeField] private ulong selectedAmount = 0; // 0 => ALL-IN

    // amount text'i geçici olarak uyarýya çevirmek için
    private bool amountInsufficientFlag = false;

    [Header("UI (TMP)")]
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI btcText;
    [SerializeField] private TextMeshProUGUI nextMoveText;
    [SerializeField] private TextMeshProUGUI modeText;
    [SerializeField] private TextMeshProUGUI pointTxt;            // CASH
    [SerializeField] private TextMeshProUGUI selectedAmountText;  // AMOUNT
    [SerializeField] private TextMeshProUGUI feedbackText;        // optional

    void Start()
    {
        if (!clicker) clicker = FindFirstObjectByType<Clicker>();
        if (!market) market = FindFirstObjectByType<BitcoinMarket>();
        RefreshUI();
    }

    void OnEnable()
    {
        if (market != null) market.OnTick += HandleTick;

        selectedAmount = 0;
        amountInsufficientFlag = false;

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

    // RESET butonu buna baðlanacak
    public void ResetAmount()
    {
        selectedAmount = 0;
        amountInsufficientFlag = false;
        Say("");
        RefreshUI();
    }

    public void MaxAmount()
    {
        if (clicker == null) return;
        selectedAmount = clicker.Point;
        amountInsufficientFlag = false;
        RefreshUI();
    }

    void AddAmount(ulong add)
    {
        ulong before = selectedAmount;
        selectedAmount += add;
        if (selectedAmount < before) selectedAmount = ulong.MaxValue; // overflow korumasý

        amountInsufficientFlag = false;
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
            Say("No cash.");
            return;
        }

        if (spend > cash)
        {
            amountInsufficientFlag = true;
            Say("Insufficient balance.");
            RefreshUI();
            return;
        }

        double boughtBtc = spend / market.Price;

        btcHoldings += boughtBtc;
        clicker.Point = cash - spend;

        amountInsufficientFlag = false;
        Say($"+{boughtBtc:0.000000} BTC");
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
                Say("No BTC.");
                return;
            }

            double cashOut = btcHoldings * market.Price;
            if (cashOut < 0) cashOut = 0;
            if (cashOut > ulong.MaxValue) cashOut = ulong.MaxValue;

            clicker.Point += (ulong)cashOut;
            btcHoldings = 0.0;

            amountInsufficientFlag = false;
            Say($"SOLD ALL: {(ulong)cashOut:0} CASH");
            RefreshUI();
            return;
        }

        // selectedAmount kadar CASH almak için gereken BTC
        double needBtc = selectedAmount / market.Price;

        if (needBtc <= 0.0)
        {
            Say("Invalid amount.");
            return;
        }

        if (needBtc > btcHoldings)
        {
            // burada balance = BTC bakiyesi, texti yine uyarýya çeviriyoruz
            amountInsufficientFlag = true;
            Say("Insufficient BTC.");
            RefreshUI();
            return;
        }

        btcHoldings -= needBtc;
        clicker.Point += selectedAmount;

        amountInsufficientFlag = false;
        Say($"-{needBtc:0.000000} BTC");
        RefreshUI();
    }

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

        if (amountInsufficientFlag && selectedAmount != 0)
        {
            selectedAmountText.text = "AMOUNT: INSUFFICIENT BALANCE";
            return;
        }

        selectedAmountText.text = selectedAmount == 0
            ? "AMOUNT: ALL IN"
            : $"AMOUNT: {selectedAmount:0}";
    }

    void Say(string msg)
    {
        if (feedbackText) feedbackText.text = msg;
    }
}
