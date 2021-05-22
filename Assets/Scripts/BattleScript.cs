using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class BattleScript : MonoBehaviourPun
{
    public Spinner spinnerScript;
    public GameObject uI_3DGameObject;
    public GameObject deathPanelUIPrefab;
    private GameObject _deathPanelUIGameObject;

    private Rigidbody rb;

    private float _startSpinSpeed;
    private float _currentSpinSpeed;

    public float commonDamageCoefficient = 0.04f;

    public bool isAttacker;
    public bool isDefender;
    private bool _isDead = false;

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
            if (photonView.IsMine)
            {
                Vector3 effectPosition = (gameObject.transform.position + collision.transform.position) / 2 + new Vector3(0, 0.05f, 0);

                //Instantiate Collision Effect ParticleSystem
                GameObject collisionEffectGameobject = GetPooledObject();
                if (collisionEffectGameobject != null)
                {
                    collisionEffectGameobject.transform.position = effectPosition;
                    collisionEffectGameobject.SetActive(true);
                    collisionEffectGameobject.GetComponentInChildren<ParticleSystem>().Play();

                    //De-activate Collision Effect Particle System after some seconds.
                    StartCoroutine(DeactivateAfterSeconds(collisionEffectGameobject, 0.5f));

                }
            }
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
        if (!_isDead)
        {
            if (isAttacker)
            {
                _damageAmount *= getDamageCoefficient_Attacker;
                if(_damageAmount > 1000)
                {
                    _damageAmount = 400f;
                }
            }
            else if (isDefender)
            {
                _damageAmount *= getDamageCoefficient_Defender;
            }
            spinnerScript.spinSpeed -= _damageAmount;
            _currentSpinSpeed = spinnerScript.spinSpeed;

            spinSpeedBar_Image.fillAmount = _currentSpinSpeed / _startSpinSpeed;
            SpinSpeedRatio_Text.text = _currentSpinSpeed.ToString("F0") + "/" + _startSpinSpeed;

            if (_currentSpinSpeed < 100)
            {
                //Die.
                Dead();
            }
        }
        
    }

    void Dead()
    {
        _isDead = true;
        GetComponent<MovementController>().enabled = false;
        rb.freezeRotation = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        spinnerScript.spinSpeed = 0f;
        uI_3DGameObject.SetActive(false);

        if (photonView.IsMine)
        {
            //countdown to respown!
            StartCoroutine(ReSpawn());
        }

    }

    IEnumerator ReSpawn()
    {
        GameObject canvasGameObject = GameObject.Find("Canvas");

        if(_deathPanelUIGameObject == null)
        {
            _deathPanelUIGameObject = Instantiate(deathPanelUIPrefab, canvasGameObject.transform);
        } else
        {
            _deathPanelUIGameObject.SetActive(true);
        }

        Text respawnTimeText = _deathPanelUIGameObject.transform.Find("RespawnTimeText").GetComponent<Text>();

        float respawnTime = 8.0f;

        respawnTimeText.text = respawnTime.ToString(".00");

        while (respawnTime > 0.0f)
        {
            yield return new WaitForSeconds(1.0f);
            respawnTime -= 1.0f;
            respawnTimeText.text = respawnTime.ToString(".00");

            GetComponent<MovementController>().enabled = false;
        }

        _deathPanelUIGameObject.SetActive(false);
        GetComponent<MovementController>().enabled = true;

        photonView.RPC("ReBorn", RpcTarget.AllBuffered);

    }
    [PunRPC]
    public void ReBorn()
    {
        spinnerScript.spinSpeed = _startSpinSpeed;
        _currentSpinSpeed = spinnerScript.spinSpeed;

        spinSpeedBar_Image.fillAmount = _currentSpinSpeed / _startSpinSpeed;
        SpinSpeedRatio_Text.text = _currentSpinSpeed + "/" + _startSpinSpeed;

        rb.freezeRotation = true;
        transform.rotation = Quaternion.Euler(Vector3.zero);

        uI_3DGameObject.SetActive(true);

        _isDead = false;

    }




    public List<GameObject> pooledObjects;
    public int amountToPool = 8;
    public GameObject CollisionEffectPrefab;
    // Start is called before the first frame update
    void Start()
    {
        CheckPlayerType();

        rb = GetComponent<Rigidbody>();


        if (photonView.IsMine)
        {
            pooledObjects = new List<GameObject>();
            for (int i = 0; i < amountToPool; i++)
            {
                GameObject obj = (GameObject)Instantiate(CollisionEffectPrefab, Vector3.zero, Quaternion.identity);
                obj.SetActive(false);
                pooledObjects.Add(obj);
            }
        }



    }

    public GameObject GetPooledObject()
    {

        for (int i = 0; i < pooledObjects.Count; i++)
        {

            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        return null;
    }

    IEnumerator DeactivateAfterSeconds(GameObject _gameObject, float _seconds)
    {
        yield return new WaitForSeconds(_seconds);
        _gameObject.SetActive(false);

    }
}
