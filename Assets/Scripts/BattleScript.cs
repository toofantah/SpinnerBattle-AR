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

    public float commonDamageCoefficient = 0.04f;

    public bool isAttacker;
    public bool isDefender;

    [Header("Player Type Damage Coefficient")]
    public float doDamageCoefficient_Attacker = 10.0f;
    public float getDamageCoefficient_Attacker = 1.2f;

    public float doDamageCoefficient_Defender = 0.75f;
    public float getDamageCoefficient_Defender = 0.2f;

    public Image spinSpeedBar_Image;
    public TextMeshProUGUI SpinSpeedRatio_Text;

    private void Awake()
    {
        _startSpinSpeed = spinnerScript.spinSpeed;
        _currentSpinSpeed = spinnerScript.spinSpeed;

        spinSpeedBar_Image.fillAmount = _currentSpinSpeed / _startSpinSpeed;
    }

    private void CheckPlayerType()
    {
        if (gameObject.name.Contains("Attacker"))
        {
            isAttacker = true;
            isDefender = false;
        } 
        else if (gameObject.name.Contains("Defender"))
        {
            isDefender = true;
            isAttacker = false;

            spinnerScript.spinSpeed = 4400;
            _startSpinSpeed = spinnerScript.spinSpeed;
            _currentSpinSpeed = spinnerScript.spinSpeed;

            SpinSpeedRatio_Text.text = _currentSpinSpeed + "/" + _startSpinSpeed;
        }
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
                float defualtDamageAmount = gameObject.GetComponent<Rigidbody>().velocity.magnitude * 3600f * commonDamageCoefficient;

                if (isAttacker)
                {
                    defualtDamageAmount *= doDamageCoefficient_Attacker;

                }
                else if (isDefender)
                {
                    defualtDamageAmount *= doDamageCoefficient_Defender;
                }

                //Apply DAmege to Slow player!
                if (collision.collider.gameObject.GetComponent<PhotonView>().IsMine)
                {
                  
                    collision.collider.gameObject.GetComponent<PhotonView>().RPC("DoDamage", RpcTarget.AllBuffered, defualtDamageAmount);
                }
               
            }
           
        }
    }

    [PunRPC]
    public void DoDamage( float _damageAmount)
    {
        if (isAttacker)
        {
            _damageAmount *= getDamageCoefficient_Attacker;
        }
        else if (isDefender)
        {
            _damageAmount *= getDamageCoefficient_Defender;
        }
        spinnerScript.spinSpeed -= _damageAmount;
        _currentSpinSpeed = spinnerScript.spinSpeed;

        spinSpeedBar_Image.fillAmount = _currentSpinSpeed / _startSpinSpeed;
        SpinSpeedRatio_Text.text = _currentSpinSpeed.ToString("F0") + "/" + _startSpinSpeed; 
    }

    void Start()
    {
        CheckPlayerType();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
