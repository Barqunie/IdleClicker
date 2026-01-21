using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MarketUI : MonoBehaviour
{
    [SerializeField] private GameObject storePanel;
    [SerializeField] private GameObject marketPanel;
    [SerializeField] private GameObject CoinMesh;

    private void Start()
    {
        if (marketPanel != null) marketPanel.SetActive(false);
        if(storePanel != null) storePanel.SetActive(false);
    }

    public void ToggleMarket()
    {
        if (marketPanel == null) return;
        marketPanel.SetActive(!marketPanel.activeSelf);
        CoinMesh.SetActive(false);
    }
    public void ToggleStore()
    {
        if (storePanel == null) return;
        storePanel.SetActive(!storePanel.activeSelf);
        CoinMesh.SetActive(false);
    }

    public void OpenMarket()
    {
        if (marketPanel == null) return;
        marketPanel.SetActive(true);
    }
    public void OpenStore()
    {
        if (storePanel == null) return;
        storePanel.SetActive(true);
    }


    public void CloseMarket()
    {
        if (marketPanel == null) return;
        marketPanel.SetActive(false); CoinMesh.SetActive(true);
    }
    public void CloseStore()
    {
        if (storePanel == null) return;
        storePanel.SetActive(false); CoinMesh.SetActive(true);
    }
}
