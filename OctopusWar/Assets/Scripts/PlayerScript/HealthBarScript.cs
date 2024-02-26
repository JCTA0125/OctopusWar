using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    [SerializeField] Slider hpbar;
    [SerializeField] TextMeshProUGUI playerNameText;
    [SerializeField] Transform player;

    public float maxHp = 100;
    public float curHp = 100;

    // Start is called before the first frame update
    void Start()
    {
        hpbar.value = (float)curHp / (float)maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = transform.position;
        newPosition.x = player.position.x;
        newPosition.z = player.position.z;
        transform.position = newPosition;
        playerNameText.transform.position = newPosition;

        HandleHp();
    }
    private void HandleHp() //Hp ���� update�����ִ� �Լ�
    {
        hpbar.value = Mathf.Lerp(hpbar.value, (float)curHp / (float)maxHp, Time.deltaTime); 
    }


}
