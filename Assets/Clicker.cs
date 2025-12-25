using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;


public class Clicker : MonoBehaviour
{

    public ulong ClickPoints = 1;
    public TextMeshProUGUI Score;
    bool Flip = true;
    public ulong Multiplier = 1;
    public ulong Point = 0;
    public void IncreaseScore()
    {


        Point += Points(Multiplier, ClickPoints);
        Score.text = Point.ToString();

        print(Multiplier);
        if (Flip)
        {

            Flip = false;
            return;

        }
        if (!Flip)
        {
            Flip = true;
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Flip)
        {
            transform.Rotate(Vector3.forward * 0.2f);
            transform.Rotate(Vector3.up * 0.2f);
            transform.Rotate(Vector3.right * 0.2f);

        }
        if (!Flip)
        {
            transform.Rotate(Vector3.back * 0.2f);
            transform.Rotate(Vector3.down * 0.2f);
            transform.Rotate(Vector3.left * 0.2f);


        }
    }
    ulong Points(ulong a, ulong b)
    {
        ulong c;
        c = a * b;
        return c;

    }

}
