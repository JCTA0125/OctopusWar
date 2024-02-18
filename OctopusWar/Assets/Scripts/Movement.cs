using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] float speed = 20f;
    //[SerializeField] Joystick joystick;
    public Joystick joystick;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    void Update()
    {
        Vector3 direction = Vector3.forward * joystick.Vertical + Vector3.right * joystick.Horizontal;
        rb.AddForce(direction * speed * Time.deltaTime, ForceMode.VelocityChange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("Item"))
        {
            Debug.Log("Item collision");
            Destroy(collision.gameObject);
            if (collision.gameObject.name.Contains("1"))
            {
                Debug.Log("velocity");
                
                StartCoroutine(OnActivateItem("velocityItem", 5));
            }
            else if (collision.gameObject.name.Contains("2"))
            {
                //reload
            }
        }
    }

    IEnumerator OnActivateItem(string buffName, int time)
    {
        if(buffName == "velocityItem")
        {
            speed = 30f;
        }
        else if(buffName == "reloadItem")
        {
            //reload Item Activate
        }
            
        yield return new WaitForSeconds(time);

        speed = 20f;
        //reload speed reset
        
    }

}
