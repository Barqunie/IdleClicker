using TMPro;
using UnityEngine;

public class Portfolio : MonoBehaviour
{
    [Header("Refs")]
    public CoinMarket market;
    public Clicker clicker; // cash yerine score kaynaðý

    [Header("Balances")]
    public float btc = 0f;

    [Header("Fee")]
    [Range(0f, 0.05f)]
    public float feeRate = 0.001f; // %0.1

    [Header("UI (TMP)")]
    public TMP_Text priceText;
    public TMP_Text cashText;
    public TMP_Text btcText;
    public TMP_Text netText;

    ulong Cash => clicker ? clicker.Point : 0UL;

    void Start()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        float price = market ? market.Price : 0f;
        ulong cash = Cash;

        if (priceText) priceText.SetText($"BTC: ${price:0,0}");
        if (cashText) cashText.SetText($"Cash: {cash:0,0}");
        if (btcText) btcText.SetText($"BTC: {btc:0.####}");
        if (netText) netText.SetText($"Net: {(cash + (double)btc * price):0,0.##}");
    }

    
    public bool Buy(float usdAmount)
    {
        if (!market || !clicker) return false;

        usdAmount = Mathf.Max(0f, usdAmount);
        if (usdAmount <= 0f) return false;

        float price = market.Price;

        float fee = usdAmount * feeRate;
        float total = usdAmount + fee;



        ulong totalUL = (ulong)Mathf.CeilToInt(total);

        if (clicker.Point < totalUL) return false;

        float btcBought = usdAmount / price;

        clicker.Point -= totalUL; //  para düþ
        btc += btcBought;


        if (clicker.Score) clicker.Score.text = clicker.Point.ToString();

        RefreshUI();
        return true;
    }


    public bool Sell(float usdAmount)
    {
        if (!market || !clicker) return false;

        usdAmount = Mathf.Max(0f, usdAmount);
        if (usdAmount <= 0f) return false;

        float price = market.Price;

        float btcNeeded = usdAmount / price;
        if (btc < btcNeeded) return false;

        float fee = usdAmount * feeRate;
        float received = usdAmount - fee;

        ulong receivedUL = (ulong)Mathf.FloorToInt(received);

        btc -= btcNeeded;
        clicker.Point += receivedUL; //  para ekle

        if (clicker.Score) clicker.Score.text = clicker.Point.ToString();

        RefreshUI();
        return true;
    }
}
