using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] float speed = 20f;
    //[SerializeField] Joystick joystick;
    public Joystick joystick;

    Rigidbody rb;
    Vector3 moveVec;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // 1. Input Value
        float x = joystick.Horizontal;
        float z = joystick.Vertical;

        // 2. Move Position 
        moveVec = new Vector3(x, 0, z) * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveVec);

        if (moveVec.sqrMagnitude == 0)
            return; // #. No input = No Rotation

        // 3. Move Rotation
        Quaternion dirQuat = Quaternion.LookRotation(moveVec);
        Quaternion moveQuat = Quaternion.Slerp(rb.rotation, dirQuat, 0.3f);
        rb.MoveRotation(moveQuat);
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
