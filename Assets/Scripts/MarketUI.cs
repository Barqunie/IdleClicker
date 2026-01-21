using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MarketUI : MonoBehaviour
{
    [SerializeField] private GameObject storePanel;
    [SerializeField] private GameObject marketPanel;
    [SerializeField] private GameObject CoinMesh;
    [SerializeField] private GameObject coinPanel;

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
        coinPanel.SetActive(false);


        HapticsAdvanced.Light();
    }
    public void ToggleStore()
    {
        if (storePanel == null) return;
        storePanel.SetActive(!storePanel.activeSelf);
        CoinMesh.SetActive(false);
        coinPanel.SetActive(false);


        HapticsAdvanced.Light();
    }

    public void OpenMarket()
    {
        if (marketPanel == null) return;
        marketPanel.SetActive(true);

        HapticsAdvanced.Light();
    }
    public void OpenStore()
    {
        if (storePanel == null) return;
        storePanel.SetActive(true);

        HapticsAdvanced.Light();
    }


    public void CloseMarket()
    {
        if (marketPanel == null) return;
        marketPanel.SetActive(false); CoinMesh.SetActive(true);
        coinPanel.SetActive(true );
        HapticsAdvanced.Light();
    }
    public void CloseStore()
    {
        if (storePanel == null) return;
        storePanel.SetActive(false); CoinMesh.SetActive(true);
        coinPanel.SetActive(true);

        HapticsAdvanced.Light();
    }
}
