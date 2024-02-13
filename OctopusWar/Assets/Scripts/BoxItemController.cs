using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEditor.Build.Content;
using UnityEngine;

public class BoxItemController : MonoBehaviour
{
    private bool isDefault;
    private bool isWoodenBox;
    private bool isTNT;
    //public bool isBarricade;

    [Header("Item Prefabs")]
    public GameObject[] itemDrops;

    private float boxHP;

    // Start is called before the first frame update
    void Start()
    {
        CheckPlayerType();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CheckPlayerType()
    {
        if (gameObject.name.Contains("Wooden"))
        {
            isDefault = false;
            isWoodenBox = true;
            isTNT = false;

            boxHP = 100;
        }
        else if (gameObject.name.Contains("TNT"))
        {
            isDefault = false;
            isWoodenBox = false;
            isTNT = true;

            boxHP = 100;
        }
        else if(gameObject.name.Contains("Default"))
        {
            isDefault = true;
            isWoodenBox = false;
            isTNT = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.name.Equals("Attacker") && !isDefault)
        {
            Debug.Log("succes");

            if(isWoodenBox)
            {
                Debug.Log("wooden");
                ItemDrop();
            }
            else if(!isTNT)
            {
                //effect
            }
            Destroy(gameObject);
            

        }
    }

    private int Choose(float[] probs)
    {

        float total = 0;

        foreach (float elem in probs)
        {
            total += elem;
        }

        float randomPoint = Random.value * total;

        for (int i = 0; i < probs.Length; i++)
        {
            if (randomPoint < probs[i])
            {
                switch (i)
                {
                    case 0:
                        Debug.Log("item1 drop");
                        break;
                    case 1:
                        Debug.Log("item2 drop");
                        break;
                    case 2:
                        Debug.Log("nothing");
                        break;
                }
                return i;
            }
            else
            {
                randomPoint -= probs[i];
            }
        }
        return probs.Length - 1;
    }


    private void ItemDrop()
    {
        //Item Drop Percentage
        //item1 : 80%
        //item2 : 15%
        //default : 5%
        int choice = Choose(new float[3] { 10f, 85f, 5f });
        if(choice != 2)
        {
            Instantiate(itemDrops[choice], transform.position, Quaternion.identity);
        }
        
    }
}
