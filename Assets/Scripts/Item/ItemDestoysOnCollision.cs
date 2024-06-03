using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDestoysOnCollision : MonoBehaviour
{
    [SerializeField] ItemSpawner itemSpawner;

    // Start is called before the first frame update
    void Start()
    {
        if(itemSpawner == null)
        {
            itemSpawner = GameObject.Find("Item Spawner").GetComponent<ItemSpawner>();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(itemSpawner != null)
        {
            if (collision.gameObject.tag == "Item")
            {
                GameObject item = collision.gameObject;
                itemSpawner.RemoveItem(item);
            }
        }
        else
        {
            if (collision.gameObject.tag == "Item")
            {
                Destroy(collision.gameObject);
            }
        }
    }
}
