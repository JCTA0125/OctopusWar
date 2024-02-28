using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class AReplacementAndPlaneDetection : MonoBehaviour
{
    ARPlaneManager m_ARPlaneManager;
    ARPlacementManager m_ARPlacementManager; //��ġ�� ���� ��ũ��Ʈ

    public GameObject placeButton;
    public GameObject adjustButton;
    public GameObject searchForGameButton;
    public GameObject scaleSlider;

    public TextMeshProUGUI informUIPanel_Text;

    private void Awake()
    {
        m_ARPlaneManager = GetComponent<ARPlaneManager>();
        m_ARPlacementManager = GetComponent<ARPlacementManager>(); 
    }

    // Start is called before the first frame update
    void Start()
    {
        placeButton.SetActive(true);
        scaleSlider.SetActive(true);
        adjustButton.SetActive(false);
        searchForGameButton.SetActive(false);

        informUIPanel_Text.text = "���� ������ ��Ʋ �Ʒ����� ��ġ�� �������ּ���!";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisableARPlacementAndPlaneDetection()
    {
        m_ARPlaneManager.enabled = false;
        m_ARPlacementManager.enabled = false; //�Ʒ����� �������� ���ϰ� �Ѵ�.
        SetAllPlaneActiveOrDeactive(false);

        scaleSlider.SetActive(false);

        placeButton.SetActive(false);
        adjustButton.SetActive(true);
        searchForGameButton.SetActive(true);

        informUIPanel_Text.text = "��Ҹ� ���߱���! Search for the Game ��ư�� �����ּ���! ";
    }

    public void EnableARPlacementAndPlaneDetection()
    {
        m_ARPlaneManager.enabled = true;
        m_ARPlacementManager.enabled = true;
        SetAllPlaneActiveOrDeactive(true);

        scaleSlider.SetActive(true);

        placeButton.SetActive(true);
        adjustButton.SetActive(false);
        searchForGameButton.SetActive(false);
    }

    private void SetAllPlaneActiveOrDeactive(bool value) //��� plane ��ü false
    {
        foreach (var plane in m_ARPlaneManager.trackables) //������ plane �� ������ �� �� �ִ�. (RPlaneManager���� ���� ���� ������ ���)
        {
            plane.gameObject.SetActive(value);
        }
    }
}
