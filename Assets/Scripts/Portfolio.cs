using UnityEngine;
using TMPro;

public class Portfolio : MonoBehaviour
{
    [Header("Refs")]
    public BitcoinMarket market;

    [Header("Balances")]
    public double cash = 1000;   // USD/USDT 
    public double btc = 0;

    [Header("Fee")]
    public double feeRate = 0.001; // %0.1

    [Header("UI (optional)")]
    public TMP_Text priceText;
    public TMP_Text cashText;
    public TMP_Text btcText;
    public TMP_Text netWorthText;

    public double Price => market ? market.price : 0;

    public bool BuyWithCash(double cashToSpend)
    {
        if (!market) return false;
        if (cashToSpend <= 0) return false;

        if (cashToSpend > cash) cashToSpend = cash;
        if (cashToSpend <= 0) return false;

        double fee = cashToSpend * feeRate;
        double netSpend = cashToSpend - fee;
        if (netSpend <= 0) return false;

        double amountBtc = netSpend / Price;

        cash -= cashToSpend;
        btc += amountBtc;

        UpdateUI();
        return true;
    }

    public bool SellBtc(double btcToSell)
    {
        if (!market) return false;
        if (btcToSell <= 0) return false;

        if (btcToSell > btc) btcToSell = btc;
        if (btcToSell <= 0) return false;

        double gross = btcToSell * Price;
        double fee = gross * feeRate;
        double net = gross - fee;

        btc -= btcToSell;
        cash += net;

        UpdateUI();
        return true;
    }

    public void UpdateUI()
    {
        if (!market) return;
        double p = Price;
        double netWorth = cash + btc * p;

        if (priceText) priceText.text = $"BTC: {p:0}";
        if (cashText) cashText.text = $"Cash: {cash:0.##}";
        if (btcText) btcText.text = $"BTC: {btc:0.####}";
        if (netWorthText) netWorthText.text = $"Net: {netWorth:0.##}";
    }
}
