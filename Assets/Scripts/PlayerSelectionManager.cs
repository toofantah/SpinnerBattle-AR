using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class PlayerSelectionManager : MonoBehaviour
{
    public Transform playerSwitcherTransform;
    

    public int playerSelectionNumber;

    [Header("UI")]
    public TextMeshProUGUI playerModelType_Text;

    public GameObject[] spinnerModels;
    public GameObject UI_Selection;
    public GameObject UI_AfterSelection;
    public Button nextButton;
    public Button previousButton;


    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        UI_Selection.SetActive(true);
        UI_AfterSelection.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    #region UI CallBack Methods
    public void NextPlayer()
    {
        playerSelectionNumber += 1;
        if(playerSelectionNumber>= spinnerModels.Length)
        {
            playerSelectionNumber = 0;
        }


        nextButton.enabled = false;
        previousButton.enabled = false;

        StartCoroutine(Rotate(Vector3.up, playerSwitcherTransform, 90, 1.0f));

        if(playerSelectionNumber == 0 || playerSelectionNumber == 1)
        {
            //Attack Type
            playerModelType_Text.text = "ATTACK";

        }else
        {
            //Defend Type
            playerModelType_Text.text = "DEFENCE";
        }
    }

    public void PreviousPlayer()
    {
        playerSelectionNumber -= 1;
        if (playerSelectionNumber <= 0)
        {
            playerSelectionNumber = spinnerModels.Length - 1;
        }
        nextButton.enabled = false;
        previousButton.enabled = false;

        StartCoroutine(Rotate(Vector3.up, playerSwitcherTransform, -90, 1.0f));

        if (playerSelectionNumber == 0 || playerSelectionNumber == 1)
        {
            //Attack Type
            playerModelType_Text.text = "ATTACK";

        }
        else
        {
            //Defend Type
            playerModelType_Text.text = "DEFENCE";
        }
    }

    public void OnSelectButtonClicked()
    {
        UI_Selection.SetActive(false);
        UI_AfterSelection.SetActive(true);
        ExitGames.Client.Photon.Hashtable playerSelectionProp = new ExitGames.Client.Photon.Hashtable { {MultiplayerARSpinnerBattle.PLAYER_SELECTION_NUMBER, playerSelectionNumber } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProp);
    }

    public void OnReselectButtonClicked()
    {
        UI_Selection.SetActive(true);
        UI_AfterSelection.SetActive(false);
    }

    public void OnbattleButtonClicked()
    {
        SceneLoader.Instance.LoadScene("Scene_Gameplay");
    }

    public void OnBackButtonClicked()
    {
        SceneLoader.Instance.LoadScene("Scene_Lobby");
    }
    #endregion

    #region Private Methods
    IEnumerator Rotate(Vector3 axis, Transform transformToRotate, float angle, float duration = 1.0f )
    {
        Quaternion originalRotation = transformToRotate.rotation;
        Quaternion finalRotation = transformToRotate.rotation * Quaternion.Euler(axis * angle);//rotate vector by another vector

        float elapsedTime = 0.0f;
        while (elapsedTime < duration)
        {
            transformToRotate.rotation = Quaternion.Slerp(originalRotation, finalRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transformToRotate.rotation = finalRotation;
        nextButton.enabled = true;
        previousButton.enabled = true;
    }
    #endregion
}
