using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MarketUI : MonoBehaviour
{
    [SerializeField] private GameObject marketPanel;
    [SerializeField] private GameObject CoinMesh;

    private void Start()
    {
        if (marketPanel != null) marketPanel.SetActive(false);
    }

    public void ToggleMarket()
    {
        if (marketPanel == null) return;
        marketPanel.SetActive(!marketPanel.activeSelf);
        CoinMesh.SetActive(false);
    }

    public void OpenMarket()
    {
        if (marketPanel == null) return;
        marketPanel.SetActive(true);
    }

    public void CloseMarket()
    {
        if (marketPanel == null) return;
        marketPanel.SetActive(false); CoinMesh.SetActive(true);
    }
}
