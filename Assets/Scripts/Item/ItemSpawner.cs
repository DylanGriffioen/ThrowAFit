using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] float spawnInterval = 5f;
    [SerializeField] float nextSpawn;
    [SerializeField] int totalMaxItems = 100;
    [SerializeField] int currentMaxItems = 5;
    [SerializeField] GameObject[] itemList;

    [Header("Location")]
    [SerializeField] GameObject spawnArea;
    [SerializeField] float dropHeight;

    [Header("Ingame")]
    [SerializeField] List<GameObject> spawnedItems;
    [SerializeField] List<GameObject> currentItems;

     // Start is called before the first frame update
    void Start()
    {
        nextSpawn = spawnInterval;
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnInterval > 0)
        {
            nextSpawn -= Time.deltaTime;

            if (nextSpawn < 0) // and currentItem.size < currentMaxItems
            {
                GameObject item = RandomItem();
                Vector3 pos = RandomLocation(spawnArea);

                SpawnItem(item, pos);
                nextSpawn = spawnInterval;
            }
        }

    }


    private Vector3 RandomLocation(GameObject spawnArea)
    {
        Vector3 origin = spawnArea.transform.position;

        BoxCollider collider = spawnArea.GetComponent<BoxCollider>();

        float length = spawnArea.transform.localScale.x;
        float width = spawnArea.transform.localScale.z;
        float height = spawnArea.transform.localScale.y;

        if (collider != null)
        {
            length *= collider.size.x;
            width *= collider.size.z;
            height *= collider.size.y;
        }

        float x = Random.Range(-length/2, length/2) + spawnArea.transform.position.x;
        float y = height + spawnArea.transform.position.y + dropHeight;
        float z = Random.Range(-width / 2, width / 2) + spawnArea.transform.position.z;

        Debug.Log($"ori: {origin}, length: {length}, width: {width}, height: {height}");
        Debug.Log($"x: {x}, z: {z}, y: {y}");

        return new Vector3(x, y, z);
    }

    private void SpawnItem(GameObject item, Vector3 pos)
    {
        GameObject go = GameObject.Instantiate(item);
        spawnedItems.Add(go);
        currentItems.Add(item);
        go.transform.position = pos;
    }

    private GameObject RandomItem()
    {
        return itemList[Random.Range(0, itemList.Length)];
    }

    public void RemoveItem(GameObject go)
    {
        currentItems.Remove(go);
    }
}
