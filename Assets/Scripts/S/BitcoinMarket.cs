using System;
using System.Collections.Generic;
using UnityEngine;

public class BitcoinMarket : MonoBehaviour
{
    [Header("Price Timeline (Demo/Controlled)")]
    [SerializeField] private float tickSeconds = 1.0f;

    // Ýstediðin array mantýðý burada
    [SerializeField]
    private List<float> priceTimeline = new List<float>
    {
        150000, 100000, 70000, 80000, 120000, 90000, 110000
    };

    public float Price { get; private set; }
    public float PrevPrice { get; private set; }
    public float NextPrice { get; private set; }

    public List<float> History { get; private set; } = new List<float>();

    public event Action OnTick;

    int idx;
    float t;

    void Start()
    {
        if (priceTimeline == null || priceTimeline.Count < 2)
            priceTimeline = new List<float> { 100000, 101000 };

        idx = 0;
        PrevPrice = priceTimeline[0];
        Price = priceTimeline[0];
        NextPrice = priceTimeline[1];

        History.Clear();
        History.Add(Price);
    }

    void Update()
    {
        t += Time.deltaTime;
        if (t < tickSeconds) return;
        t = 0f;

        Tick();
    }

    void Tick()
    {
        PrevPrice = Price;

        idx = (idx + 1) % priceTimeline.Count;
        Price = priceTimeline[idx];

        int nextIdx = (idx + 1) % priceTimeline.Count;
        NextPrice = priceTimeline[nextIdx];

        History.Add(Price);
        if (History.Count > 200) History.RemoveAt(0);

        OnTick?.Invoke();
    }

    public float NextPercent()
    {
        if (Price <= 0.0001f) return 0f;
        return (NextPrice - Price) / Price; // 0.12 => %12
    }
}

