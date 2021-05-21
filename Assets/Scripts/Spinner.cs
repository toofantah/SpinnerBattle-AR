using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    public float spinSpeed = 3600f;
    public bool isSpinning = false;
    public GameObject playerGraphic;

    private Rigidbody rb;
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        if (isSpinning)
        {
            playerGraphic.transform.Rotate(new Vector3(0, spinSpeed * Time.deltaTime, 0));
        }
    }
}
