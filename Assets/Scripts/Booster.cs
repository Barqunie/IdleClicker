using System.Collections;
using TMPro;
using UnityEngine;

public class Booster : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Clicker clicker;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI boosterStatusText; 
    [SerializeField] private TextMeshProUGUI multiplierText;    

    [Header("Boost Settings")]
    [SerializeField] private float multiplierBonus = 9;   
    [SerializeField] private float boostDuration = 10f;

    [Header("Random Timing (seconds)")]
    [SerializeField] private float minWait = 15f;
    [SerializeField] private float maxWait = 45f;

    private bool isActive;

    private void Start()
    {
        if (clicker == null) clicker = FindFirstObjectByType<Clicker>();

        if (boosterStatusText != null) boosterStatusText.gameObject.SetActive(false);
        UpdateMultiplierUI();

        StartCoroutine(BoosterLoop());
    }

    private IEnumerator BoosterLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minWait, maxWait));
            yield return ActivateBoost();
        }
    }

    private IEnumerator ActivateBoost()
    {
        if (clicker == null || isActive) yield break;
        isActive = true;

        // Boost ON
        clicker.Multiplier += multiplierBonus;
        UpdateMultiplierUI();

        float t = boostDuration;
        if (boosterStatusText != null) boosterStatusText.gameObject.SetActive(true);

        while (t > 0f)
        {
            if (boosterStatusText != null)
                boosterStatusText.text = $"Booster activated for {Mathf.CeilToInt(t)}s";

            t -= Time.deltaTime;
            yield return null;
        }

        if (boosterStatusText != null) boosterStatusText.gameObject.SetActive(false);

        // Boost OFF
        if (clicker.Multiplier >= multiplierBonus) clicker.Multiplier -= multiplierBonus;
        else clicker.Multiplier = 1;

        UpdateMultiplierUI();
        isActive = false;
    }

    private void UpdateMultiplierUI()
    {
        if (multiplierText == null) return;

        ulong number = (clicker != null) ? clicker.Multiplier : 1;
        multiplierText.text = $"x{number}";
    }
}
