using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bullet : MonoBehaviour
{
    [SerializeField] float bulletSpeed = 2f;
    Rigidbody rb;

    private void Awake()
    {
       rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("ball just bumped into the Enemy!");

           //���ݿ����� ������� �ʴ� ������
            gameObject.SetActive(false);
            //Debug.Log(collision.gameObject.name);

            if (collision.collider.gameObject.transform.parent.GetComponent<PhotonView>().IsMine) 
            {
                
                //bullet�� �������� ���� �ƴ϶��(���� �ʿ�)

                //������ �ӵ��� ���� ���ǳʿ��� �������� ����.
                collision.collider.gameObject.transform.parent.GetComponent<PhotonView>().RPC("GetDamage", RpcTarget.AllBuffered); 
            }
            

        }
        else if (collision.gameObject.tag == "Wall")
        {
            //Vector3 newDirection = Quaternion.AngleAxis(90, Vector3.up) * rb.velocity.normalized;
            // rb.velocity = newDirection * bulletSpeed;
            // ���� ǥ�� ���� ���� ���
            Vector3 wallNormal = collision.contacts[0].normal;

            // ���� �ӵ� ���͸� ���� ǥ��� �ݻ�
            Vector3 reflectedVelocity = Vector3.Reflect(rb.velocity, wallNormal);

            // �ݻ�� �ӵ� ���͸� ����Ͽ� �ӵ� ������Ʈ
            rb.velocity = reflectedVelocity.normalized * bulletSpeed;

           
            Debug.Log("ball just bumped into the wall!");
        }
        
    }
}
