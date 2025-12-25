using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using UnityEditor.iOS.Xcode;
using JetBrains.Annotations;
using Unity.VisualScripting;

public class Booster : MonoBehaviour
{

    public Clicker clicker;
    void Awake()
    {
        clicker = GetComponent<Clicker>();
    }

    // Start is called before the first frame update

    float currentTime;
    public float startingTime = 10f;

    [SerializeField] Text countdownText;

    public ulong MultiplierValue = 9;
    void Start()
    {
        currentTime = startingTime;


        clicker.Multiplier += MultiplierValue;
    }
    void Update()
    {
        currentTime -= 1 * Time.deltaTime;
        // countdownText.text = currentTime.ToString("0");
        print(currentTime);
        if (currentTime <= 0)
        {


            clicker.Multiplier -= MultiplierValue;

        }
    }

}
