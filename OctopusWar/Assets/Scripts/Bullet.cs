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
            Destroy(gameObject);
            //Debug.Log(collision.gameObject.name);

            Transform child = collision.transform.parent.Find("HCanvas").Find("HP"); //Hp를 감소시킨다.

            if (child != null)
            {
                HealthBarScript healthBarScript = child.GetComponent<HealthBarScript>();
                Debug.Log("found it");
                healthBarScript.GetDamage();
            }
            else
            {
                Debug.Log("can't find script!");
            }

        }
        else if (collision.gameObject.tag == "Wall")
        {
            Debug.Log("you just bump into the wall!");
        }
    }
}
