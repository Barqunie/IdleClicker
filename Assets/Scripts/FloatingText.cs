using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float InitialyVelocity = 15f;
    public float InitialxVelocityRange = 5f;
    public float LifeTime = 1f;

    //public GameObject flaotingPoints;
    private Rigidbody2D _rigidbody;
    private TMP_Text _coinValue;

    [SerializeField] private Clicker clicker;

    private void SetMessage(ulong msg)
    {
        clicker.ClickPoints = msg;
        _coinValue.SetText(msg.ToString());
        
    }
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _coinValue = GetComponentInChildren<TMP_Text>();
    }

    private void Start()
    {
        _rigidbody.velocity = new Vector2(Random.Range(-InitialxVelocityRange, InitialxVelocityRange), InitialyVelocity);
        Destroy(gameObject, LifeTime);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
