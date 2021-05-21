using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerSetup : MonoBehaviourPun
{
    public TextMeshProUGUI playerNameText;
    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            //enable joystic for my player
            transform.GetComponent<MovementController>().enabled = true;
            transform.GetComponent<MovementController>().joyStick.gameObject.SetActive(true);
        }
        else
        {
            transform.GetComponent<MovementController>().enabled = false;
            transform.GetComponent<MovementController>().joyStick.gameObject.SetActive(false);
        }

        SetPlayerName();
    }

    void SetPlayerName()
    {
        if(playerNameText != null)
        {
            if (photonView.IsMine)
            {
                playerNameText.text = "YOU";
                playerNameText.color = Color.blue;
            }
            else
            {
                playerNameText.text = photonView.Owner.NickName;
                playerNameText.color = Color.red;
            }
            
        }
    }
}
