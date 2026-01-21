using TMPro;
using UnityEngine;

public class Portfolio : MonoBehaviour
{
    [Header("Refs")]
    public CoinMarket market;

    [Header("Balances")]
    public float cash = 1000f;
    public float btc = 0f;

    [Header("Fee")]
    [Range(0f, 0.05f)]
    public float feeRate = 0.001f; // %0.1

    [Header("UI (TMP)")]
    public TMP_Text priceText;
    public TMP_Text cashText;
    public TMP_Text btcText;
    public TMP_Text netText;

    void Start()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        float price = market ? market.Price : 0f;

        if (priceText) priceText.SetText($"BTC: ${price:0,0}");
        if (cashText) cashText.SetText($"Cash: {cash:0,0.##}");
        if (btcText) btcText.SetText($"BTC: {btc:0.####}");
        if (netText) netText.SetText($"Net: {(cash + btc * price):0,0.##}");
    }

    // usdAmount: kaç dolarlýk BTC almak istiyoruz
    public bool Buy(float usdAmount)
    {
        if (!market) return false;
        usdAmount = Mathf.Max(0f, usdAmount);
        if (usdAmount <= 0f) return false;

        float price = market.Price;
        float fee = usdAmount * feeRate;
        float total = usdAmount + fee;

        if (cash < total) return false;

        float btcBought = usdAmount / price;
        cash -= total;
        btc += btcBought;

        RefreshUI();
        return true;
    }

    // usdAmount: kaç dolarlýk BTC satmak istiyoruz
    public bool Sell(float usdAmount)
    {
        if (!market) return false;
        usdAmount = Mathf.Max(0f, usdAmount);
        if (usdAmount <= 0f) return false;

        float price = market.Price;
        float btcNeeded = usdAmount / price;

        if (btc < btcNeeded) return false;

        float fee = usdAmount * feeRate;
        float received = usdAmount - fee;

        btc -= btcNeeded;
        cash += received;

        RefreshUI();
        return true;
    }
}
