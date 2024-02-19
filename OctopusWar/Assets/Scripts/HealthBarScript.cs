using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    [SerializeField] Slider hpbar;
    [SerializeField] Transform player;

    private float maxHp = 100;
    private float curHp = 100;

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

        HandleHp();
    }
    private void HandleHp() //Hp 값을 update시켜주는 함수
    {
        hpbar.value = Mathf.Lerp(hpbar.value, (float)curHp / (float)maxHp, Time.deltaTime); 
    }
    public void GetDamage()
    {
        if (curHp >= 10)
        {
            curHp -= 10;


        }
        else
        {
            GameObject player = transform.parent.parent.gameObject;
            Destroy(player);
        }
    }
}
