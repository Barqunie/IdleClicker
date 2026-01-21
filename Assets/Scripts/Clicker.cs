
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
    public ulong GPU;

    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private Transform floatingSpawnPoint;

    public void IncreaseScore()
    {

        HapticsAdvanced.Medium();
        Point += Points(Multiplier, ClickPoints);
      

       GameObject gainedCoinValue = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity) as GameObject;
        gainedCoinValue.GetComponent<TextMesh>().text = "+ " + Points(Multiplier, ClickPoints).ToString();

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
        gameObject.AddComponent<FloatingText>();
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
        Score.text = Point.ToString();
    }
    ulong Points(ulong a, ulong b)
    {
        ulong c;
        c = a * b;
        return c;

    }

}
