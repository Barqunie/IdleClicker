using System.Collections.Generic;
using Unity.AppUI.Redux;
using UnityEngine;

public class BitcoinMarket : MonoBehaviour
{
    [Header("Price")]
    public float price = 50000f;
    public float fairValue = 50000f;
    public float updateInterval = 0.25f;

    [Header("Behavior")]
    public float driftPerSec = 0.00f;
    public float volatility = 0.015f;
    public float meanReversion = 0.02f;

    [Header("Pump/Dump")]
    public float eventChancePerTick = 0.01f;
    public float eventMagnitude = 0.08f;

    [Header("History")]
    public int historySize = 120;
    public List<float> history = new();

    float _timer;

    void Start()
    {
        history.Clear();
        for (int i = 0; i < historySize; i++) history.Add(price);
    }

    void Update()
    {
        _timer += Time.deltaTime;
        while (_timer >= updateInterval)
        {
            _timer -= updateInterval;
            Tick(updateInterval);
        }
    }

    void Tick(float dt)
    {
        float reversion = (fairValue - price) / Mathf.Max(1f, fairValue);
        float baseMove = (driftPerSec + meanReversion * reversion) * dt;

        float noise = (Random.value - 0.5f) + (Random.value - 0.5f) + (Random.value - 0.5f);
        float volMove = noise * volatility * dt;

        float pctMove = baseMove + volMove;

        if (Random.value < eventChancePerTick)
        {
            float dir = Random.value < 0.5f ? -1f : 1f;
            pctMove += dir * eventMagnitude;
        }

        price *= (1f + pctMove);
        price = Mathf.Max(1f, price);

        history.Add(price);
        if (history.Count > historySize) history.RemoveAt(0);
    }
    void AddPriceToHistory(float p)
    {
        history.Add(p);
        if (history.Count > historySize)
            history.RemoveAt(0);
    }
}
