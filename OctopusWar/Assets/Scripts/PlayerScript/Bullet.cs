using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bullet : MonoBehaviour
{


    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("you just bump into the Player!");
            /*
            Destroy(gameObject);
            //Debug.Log(collision.gameObject.name);

            if (collision.collider.gameObject.GetComponent<PhotonView>().IsMine) 
            {
                //나보다 속도가 느린 스피너에게 데미지를 입힘.
                collision.collider.gameObject.GetComponent<PhotonView>().RPC("GetDamage", RpcTarget.AllBuffered); 
            }
            */

        }
        else if (collision.gameObject.tag == "Wall")
        {
            Debug.Log("you just bump into the wall!");
        }
        
    }
}
