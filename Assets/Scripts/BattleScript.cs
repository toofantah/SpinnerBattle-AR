using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class BattleScript : MonoBehaviour
{
    public Spinner spinnerScript;

    private float _startSpinSpeed;
    private float _currentSpinSpeed;

    public Image spinSpeedBar_Image;
    public TextMeshProUGUI SpinSpeedRatio_Text;

    private void Awake()
    {
        _startSpinSpeed = spinnerScript.spinSpeed;
        _currentSpinSpeed = spinnerScript.spinSpeed;

        spinSpeedBar_Image.fillAmount = _currentSpinSpeed / _startSpinSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //comparing the speeds of Spinner Tops
            float mySpeed = gameObject.GetComponent<Rigidbody>().velocity.magnitude;
            float otherPlayerSpeed = collision.collider.gameObject.GetComponent<Rigidbody>().velocity.magnitude;

            if(mySpeed > otherPlayerSpeed)
            {
                //Apply DAmege to Slow player!
                if (collision.collider.gameObject.GetComponent<PhotonView>().IsMine)
                {
                    collision.collider.gameObject.GetComponent<PhotonView>().RPC("DoDamage", RpcTarget.AllBuffered, 400f);
                }
               
            }
           
        }
    }

    [PunRPC]
    public void DoDamage( float _damageAmount)
    {
        spinnerScript.spinSpeed -= _damageAmount;
        _currentSpinSpeed = spinnerScript.spinSpeed;

        spinSpeedBar_Image.fillAmount = _currentSpinSpeed / _startSpinSpeed;
        SpinSpeedRatio_Text.text = _currentSpinSpeed + "/" + _startSpinSpeed; 
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
