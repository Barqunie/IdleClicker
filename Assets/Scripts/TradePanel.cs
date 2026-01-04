using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TradePanel : MonoBehaviour
{
    public enum TradeMode { Buy, Sell }
    public enum AmountMode { Fixed, Percent }

    [Header("Refs")]
    [SerializeField] private Portfolio portfolio;

    [Header("UI - Tabs")]
    [SerializeField] private Button buyTabButton;
    [SerializeField] private Button sellTabButton;

    [Header("UI - Execute")]
    [SerializeField] private Button executeButton;
    [SerializeField] private Text executeLabel;                 // ✅ LEGACY TEXT (buton yazısı)

    [Header("UI - Info (TMP)")]
    [SerializeField] private TMP_Text selectedAmountText;       // ✅ TMP
    [SerializeField] private TMP_Text feedbackText;             // ✅ TMP

    [Header("Amount Buttons (optional)")]
    [SerializeField] private Button[] amountButtons;            // A1 A2 A3
    [SerializeField] private Text[] amountButtonLabels;         // ✅ LEGACY TEXT (A1/A2/A3 yazıları)

    [Header("Config")]
    public TradeMode tradeMode = TradeMode.Buy;
    public AmountMode amountMode = AmountMode.Fixed;

    private double fixedAmount = 10;      // BUY için cash
    private double percentAmount = 1.0;   // 1.0 = %100

    void Start()
    {
        if (buyTabButton) buyTabButton.onClick.AddListener(() => SetMode(TradeMode.Buy));
        if (sellTabButton) sellTabButton.onClick.AddListener(() => SetMode(TradeMode.Sell));
        if (executeButton) executeButton.onClick.AddListener(Execute);

        SetMode(tradeMode);
        SetFixedAmount(10);
        RefreshUI();
    }

    public void SetMode(TradeMode mode)
    {
        tradeMode = mode;

        // Default UX: BUY fixed, SELL percent
        amountMode = (mode == TradeMode.Buy) ? AmountMode.Fixed : AmountMode.Percent;

        UpdateAmountButtons();
        RefreshUI();
    }

    public void SetFixedAmount(double amount)
    {
        fixedAmount = Mathf.Max(0, (float)amount);
        amountMode = AmountMode.Fixed;
        RefreshUI();
    }

    public void SetPercent(double p01)
    {
        percentAmount = Mathf.Clamp01((float)p01);
        amountMode = AmountMode.Percent;
        RefreshUI();
    }

    void Execute()
    {
        if (!portfolio)
        {
            SetFeedback("Portfolio not assigned!");
            return;
        }

        bool ok;

        if (tradeMode == TradeMode.Buy)
        {
            double cashToSpend = ResolveBuyCashAmount();
            ok = portfolio.BuyWithCash(cashToSpend);
            if (!ok) SetFeedback("Not enough cash");
            else SetFeedback("");
        }
        else
        {
            double btcToSell = ResolveSellBtcAmount();
            ok = portfolio.SellBtc(btcToSell);
            if (!ok) SetFeedback("Not enough BTC");
            else SetFeedback("");
        }

        RefreshUI();
    }

    double ResolveBuyCashAmount()
    {
        if (amountMode == AmountMode.Fixed)
            return System.Math.Min(fixedAmount, portfolio.cash);
        else
            return portfolio.cash * percentAmount;
    }

    double ResolveSellBtcAmount()
    {
        if (amountMode == AmountMode.Fixed)
            return System.Math.Min(fixedAmount, portfolio.btc);
        else
            return portfolio.btc * percentAmount;
    }

    void RefreshUI()
    {
        // ✅ Execute button yazısı (LEGACY Text)
        if (executeLabel)
            executeLabel.text = (tradeMode == TradeMode.Buy) ? "BUY" : "SELL";

        // ✅ Info yazıları (TMP)
        if (selectedAmountText)
        {
            if (tradeMode == TradeMode.Buy)
            {
                selectedAmountText.text =
                    (amountMode == AmountMode.Fixed)
                        ? $"Amount: {fixedAmount:0}"
                        : $"Amount: {(percentAmount * 100):0}%";
            }
            else
            {
                selectedAmountText.text =
                    (amountMode == AmountMode.Fixed)
                        ? $"Amount: {fixedAmount:0.####} BTC"
                        : $"Amount: {(percentAmount * 100):0}%";
            }
        }

        portfolio?.UpdateUI();
    }

    void SetFeedback(string msg)
    {
        if (feedbackText) feedbackText.text = msg;
    }

    void UpdateAmountButtons()
    {
        if (amountButtons == null || amountButtonLabels == null) return;

        if (tradeMode == TradeMode.Buy)
        {
            SetLabel(0, "10"); Bind(0, () => SetFixedAmount(10));
            SetLabel(1, "100"); Bind(1, () => SetFixedAmount(100));
            SetLabel(2, "MAX"); Bind(2, () => SetPercent(1.0));   // %100 cash
        }
        else
        {
            SetLabel(0, "10%"); Bind(0, () => SetPercent(0.10));
            SetLabel(1, "50%"); Bind(1, () => SetPercent(0.50));
            SetLabel(2, "100%"); Bind(2, () => SetPercent(1.00));
        }
    }

    void SetLabel(int idx, string txt)
    {
        if (amountButtonLabels == null) return;
        if (idx < 0 || idx >= amountButtonLabels.Length) return;
        if (amountButtonLabels[idx]) amountButtonLabels[idx].text = txt;
    }

    void Bind(int idx, UnityEngine.Events.UnityAction action)
    {
        if (amountButtons == null) return;
        if (idx < 0 || idx >= amountButtons.Length) return;
        if (!amountButtons[idx]) return;

        amountButtons[idx].onClick.RemoveAllListeners();
        amountButtons[idx].onClick.AddListener(action);
    }
}
