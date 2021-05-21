using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SynchronizationScript : MonoBehaviour, IPunObservable
{
    Rigidbody rb;
    PhotonView photonView;

    Vector3 networkedPosision;
    Quaternion networkedRotation;

    public bool synchronizeVelocity = true;
    public bool synchronizeAngularVelocity = true;
    public bool isTeleportEnabled = true;

    public float teleportIfDistanceGreaterThan = 1.0f;
    private float distance;
    private float angle;

    private GameObject battleArenaGameObject;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
        battleArenaGameObject = GameObject.Find("BattleArena");

        networkedPosision = new Vector3();
        networkedRotation = new Quaternion();
    }

    void Start()
    {
      
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            rb.position = Vector3.MoveTowards(rb.position, networkedPosision, distance * (1.0f/PhotonNetwork.SerializationRate));
            rb.rotation = Quaternion.RotateTowards(rb.rotation, networkedRotation, angle*(1.0f/ PhotonNetwork.SerializationRate));
        }
       
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //then, PhotonView ismine and i am the one who controls the player.
            // should send position , velocity etc. data to the other player.
            stream.SendNext(rb.position-battleArenaGameObject.transform.position);
            stream.SendNext(rb.rotation);

            if (synchronizeVelocity)
            {
                stream.SendNext(rb.velocity);
            }

            if (synchronizeAngularVelocity)
            {
                stream.SendNext(rb.angularVelocity);
            }
        }
        else
        {
            // Called on my player game object that exist in remote playeres game.
            networkedPosision = (Vector3)stream.ReceiveNext()+battleArenaGameObject.transform.position;
            networkedRotation = (Quaternion)stream.ReceiveNext();

            if (isTeleportEnabled)
            {
                if(Vector3.Distance(rb.position,networkedPosision) > teleportIfDistanceGreaterThan)
                {
                    rb.position = networkedPosision;
                }
            }

            if (synchronizeVelocity || synchronizeAngularVelocity)
            {
                float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));

                if (synchronizeVelocity)
                {
                    rb.velocity = (Vector3)stream.ReceiveNext();

                    networkedPosision += rb.velocity * lag;

                    distance = Vector3.Distance(rb.position, networkedPosision);
                }

                if (synchronizeAngularVelocity)
                {
                    rb.angularVelocity = (Vector3)stream.ReceiveNext();

                    networkedRotation = Quaternion.Euler(rb.angularVelocity * lag)*networkedRotation;

                    angle = Quaternion.Angle(rb.rotation, networkedRotation);
                }
            }

            
        }
    }
   
}
