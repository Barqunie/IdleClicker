using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms.Impl;
using Unity.VisualScripting;

public class BuyGPU : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Clicker clicker;
    private ulong cost = 100;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (clicker == null) clicker = FindFirstObjectByType<Clicker>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Buy()
    {

        if (clicker.Point >= cost)
            clicker.Point -= cost;
            cost *= 2;


    }
}
