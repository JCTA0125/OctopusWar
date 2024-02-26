using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Damage : MonoBehaviourPun
{
    HealthBarScript healthBarScript;
    private float curHp; 

    private void Awake()
    {
        healthBarScript = transform.Find("HCanvas").Find("HP").GetComponent<HealthBarScript>();
    }

    [PunRPC]
    public void GetDamage()
    {
        if (healthBarScript.curHp >= 10)
        {
            healthBarScript.curHp -= 10;


        }
        else
        {
            GameObject player = transform.gameObject;
            PhotonNetwork.Destroy(player);
        }

    }
}
