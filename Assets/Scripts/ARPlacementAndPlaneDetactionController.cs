using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;


public class ARPlacementAndPlaneDetactionController : MonoBehaviour
{
    ARPlaneManager m_ARPlaneManager;
    ARPlacementManager m_ARPlacementManager;

    public GameObject placeButton;
    public GameObject adjustButton;
    public GameObject searchForGameButton;
    public TextMeshProUGUI informUIPanel_Text;
    public GameObject scaleSlider;


    private void Awake()
    {
        m_ARPlaneManager = GetComponent<ARPlaneManager>();
        m_ARPlacementManager = GetComponent<ARPlacementManager>();

        informUIPanel_Text.text = "Move your phone to detect planes and place the Arena!";
    }

    void Start()
    {
        placeButton.SetActive(true);
        scaleSlider.SetActive(true);

        searchForGameButton.SetActive(false);
        adjustButton.SetActive(false);
    }

   
    void Update()
    {
        
    }

    public void DisableARPlacementAndPlaneDetection()
    {
        m_ARPlaneManager.enabled = false;
        m_ARPlacementManager.enabled = false;

        SetAllPlainsActiveOrDeactive(false);

        placeButton.SetActive(false);
        adjustButton.SetActive(true);
        searchForGameButton.SetActive(true);
        scaleSlider.SetActive(false);

        informUIPanel_Text.text = "Great! you adjust your Arena, Waow... Now, search for games to BATTLE!";
    }

    public void EnableARPlacementAndPlaneDetection()
    {
        m_ARPlaneManager.enabled = true;
        m_ARPlacementManager.enabled = true;

        SetAllPlainsActiveOrDeactive(true);
        placeButton.SetActive(true);
        adjustButton.SetActive(false);
        searchForGameButton.SetActive(false);
        scaleSlider.SetActive(true);
        informUIPanel_Text.text = "Well...Move your phone to detect planes and place the Arena!!";
    }

    private void SetAllPlainsActiveOrDeactive(bool value)
    {
        foreach ( var plane in m_ARPlaneManager.trackables)
        {
            plane.gameObject.SetActive(value);
        }
    }
}
