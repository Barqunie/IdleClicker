using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms.Impl;
using Unity.VisualScripting;

public class Buy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Clicker clicker;
    private ulong costGPU = 100;
    private ulong costClicker = 200;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (clicker == null) clicker = FindFirstObjectByType<Clicker>();
        InvokeRepeating(nameof(AddPointsPerSec), 1f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BuyGPU()
    {

        if (clicker.Point >= costGPU)
            clicker.Point -= costGPU;
            costGPU *= 2;
        clicker.GPU++;

    }
    void AddPointsPerSec()
    {
        clicker.Point += clicker.GPU * 2;
        
    }
    public void BuyClicker()
    {
        clicker.ClickPoints++;
        costClicker *= 3;
    }
    public void BuyCoin()
    {

    }
}
