using System.Collections.Generic;
using UnityEngine;

public class CoinMarket : MonoBehaviour
{
    [Header("Price")]
    public float Price = 50000f;
    public float FairValue = 50000f;
    public float UpdateInterval = 0.25f;

    [Header("Behavior")]
    public float DriftPerSec = 0.0f;        // uzun dönem yön
    public float Volatility = 0.015f;       // rastgele oynaklýk
    public float MeanReversion = 0.02f;     // fair value’a geri çekme

    [Header("Pump/Dump Events")]
    public float EventChancePerTick = 0.01f; // her tickte olma olasýlýðý
    public float EventMagnitude = 0.08f;     // % etki (0.08 = %8)

    [Header("History")]
    public int HistorySize = 120;
    public List<float> History = new List<float>();

    float _timer;

    void Start()
    {
        History.Clear();
        for (int i = 0; i < HistorySize; i++) History.Add(Price);
    }

    void Update()
    {
        _timer += Time.deltaTime;
        while (_timer >= UpdateInterval)
        {
            _timer -= UpdateInterval;
            Tick();
        }
    }

    void Tick()
    {
        // 1) drift
        float drift = DriftPerSec * UpdateInterval;

        // 2) noise (volatility)
        float noise = Random.Range(-1f, 1f) * Volatility;

        // 3) mean reversion (price -> fair)
        float mr = (FairValue - Price) / Mathf.Max(FairValue, 1f) * MeanReversion;

        // 4) pump/dump event
        float evt = 0f;
        if (Random.value < EventChancePerTick)
            evt = Random.Range(-EventMagnitude, EventMagnitude);

        // yüzde deðiþim
        float pctChange = drift + noise + mr + evt;

        Price *= (1f + pctChange);
        Price = Mathf.Max(1f, Price); // 0’a düþmesin

        // history push
        History.Add(Price);
        if (History.Count > HistorySize) History.RemoveAt(0);
    }
}
