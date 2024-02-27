using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Damage : MonoBehaviourPun
{
    HealthBarScript healthBarScript;
    int damageAmount;

    private void Awake()
    {
        healthBarScript = transform.Find("HCanvas").Find("HP").GetComponent<HealthBarScript>();
    }
    private void Start()
    {
        if(gameObject.name == "Attacker(Clone)")
        {
            damageAmount = 15;
        }
        else
        {
            damageAmount = 10;
        }
    }

    [PunRPC]
    public void GetDamage()
    {
        if (healthBarScript.curHp >= 10)
        {
            healthBarScript.curHp -= damageAmount;


        }
        else
        {
            GameObject player = transform.gameObject;
            PhotonNetwork.Destroy(player);
        }

    }
}
