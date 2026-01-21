using System;
using System.Collections.Generic;
using UnityEngine;

public class BitcoinMarket : MonoBehaviour
{
    public enum Mode { Timeline, Simulated }

    [Header("Mode")]
    [SerializeField] private Mode mode = Mode.Simulated;

    [Header("Tick")]
    [SerializeField] private float tickSeconds = 1.0f;

    [Header("Timeline (only if Mode=Timeline)")]
    [SerializeField]
    private List<float> priceTimeline = new List<float>
    {
        150000, 100000, 70000, 80000, 120000, 90000, 110000
    };

    [Header("Simulated Market (only if Mode=Simulated)")]
    [SerializeField] private float startPrice = 100000f;

    [Tooltip("Uzun dönem yön (%). 0.001 = tick başına +0.1%")]
    [SerializeField] private float driftPerTick = 0.0002f;

    [Tooltip("Rastgele oynaklık (%). 0.03 = yaklaşık ±3%")]
    [SerializeField] private float volatility = 0.03f;

    [Tooltip("FairValue’a geri çekme gücü (%). 0.02 = hafif mean reversion")]
    [SerializeField] private float meanReversion = 0.02f;

    [SerializeField] private float fairValue = 100000f;

    [Header("Pump / Dump")]
    [SerializeField] private float eventChancePerTick = 0.05f; // %5 olasılık
    [SerializeField] private float eventMagnitude = 0.10f;     // ±10%

    [Header("History")]
    [SerializeField] private int historyLimit = 200;

    public float Price { get; private set; }
    public float PrevPrice { get; private set; }
    public float NextPrice { get; private set; }

    public List<float> History { get; private set; } = new List<float>();
    public event Action OnTick;

    private int idx;
    private float t;

    void Start()
    {
        Init();
    }

    void Init()
    {
        History.Clear();
        idx = 0;

        if (mode == Mode.Timeline)
        {
            if (priceTimeline == null || priceTimeline.Count < 2)
                priceTimeline = new List<float> { 100000, 101000 };

            PrevPrice = priceTimeline[0];
            Price = priceTimeline[0];
            NextPrice = priceTimeline[1];
        }
        else
        {
            if (startPrice < 1f) startPrice = 1f;
            if (fairValue < 1f) fairValue = startPrice;

            PrevPrice = startPrice;
            Price = startPrice;

            // bir sonraki tick preview üret
            NextPrice = Price * (1f + SamplePctChange());
        }

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

        if (mode == Mode.Timeline)
        {
            idx = (idx + 1) % priceTimeline.Count;
            Price = priceTimeline[idx];

            int nextIdx = (idx + 1) % priceTimeline.Count;
            NextPrice = priceTimeline[nextIdx];
        }
        else
        {
            // preview olarak NextPrice için zaten bir pct üretiyoruz, onu uygula:
            // (istersen her tick yeni pct üretip uygula da olur)
            float pct = (NextPrice - Price) / Mathf.Max(Price, 1f);
            Price *= (1f + pct);
            Price = Mathf.Max(1f, Price);

            // sonraki tick preview:
            NextPrice = Price * (1f + SamplePctChange());
        }

        History.Add(Price);
        if (History.Count > historyLimit) History.RemoveAt(0);

        OnTick?.Invoke();
    }

    float SamplePctChange()
    {
        // drift
        float drift = driftPerTick;

        // noise: [-1..1] * volatility
        float noise = UnityEngine.Random.Range(-1f, 1f) * volatility;

        // mean reversion: price -> fairValue
        float mr = (fairValue - Price) / Mathf.Max(fairValue, 1f) * meanReversion;

        // event: pump/dump
        float evt = 0f;
        if (UnityEngine.Random.value < eventChancePerTick)
            evt = UnityEngine.Random.Range(-eventMagnitude, eventMagnitude);

        return drift + noise + mr + evt;
    }

    public float NextPercent()
    {
        if (Price <= 0.0001f) return 0f;
        return (NextPrice - Price) / Price;
    }

    // Editor’da mode değiştirince resetlemek istersen
    public void ResetMarket()
    {
        Init();
    }
}
