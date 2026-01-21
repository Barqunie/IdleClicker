using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Buy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Clicker clicker;

    [Header("Costs")]
    [SerializeField] private ulong costGPU = 5;
    [SerializeField] private ulong costClicker = 5;

    [Header("UI")]
    [SerializeField] private TMP_Text pointText;
    [SerializeField] private TMP_Text gpuCostText;
    [SerializeField] private TMP_Text clickerCostText;
    void Start()
    {
        if (clicker == null)
            clicker = FindFirstObjectByType<Clicker>();

        InvokeRepeating(nameof(AddPointsPerSec), 1f, 1f);
        UpdateUI();
    }

    public void BuyGPU()
    {
        if (clicker.Point >= costGPU)
        {
            clicker.Point -= costGPU;
            clicker.GPU++;
            costGPU *= 2;

            UpdateUI();
        }
    }

    public void BuyClicker()
    {
        if (clicker.Point >= costClicker)
        {
            clicker.Point -= costClicker;
            clicker.Multiplier++;
            costClicker *= 3;

            UpdateUI();
        }
    }

    void AddPointsPerSec()
    {
        clicker.Point += clicker.GPU * 2;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (pointText) pointText.text = $"POINTS: {clicker.Point}";
        if (gpuCostText) gpuCostText.text = $"COST: {costGPU}";
        if (clickerCostText) clickerCostText.text = $"COST: {costClicker}";

    }
}
