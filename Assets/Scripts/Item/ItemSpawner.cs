using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] float spawnInterval = 5f;
    [SerializeField] float nextSpawn;
    [SerializeField] int currentMaxItems = 5;
    [SerializeField] GameObject itemsParent;
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
        if (GameManager._instance != null)
        {
            currentMaxItems = GameManager._instance.MaxItemAmount > 0 ? GameManager._instance.MaxItemAmount : currentMaxItems;
            spawnInterval = GameManager._instance.ItemSpawnInterval > 0 ? GameManager._instance.ItemSpawnInterval : spawnInterval;
        }
        nextSpawn = spawnInterval;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.GAME_STATE == GameStatus.PREGAME && GameManager._instance != null)
        {
            currentMaxItems = GameManager._instance.MaxItemAmount > 0 ? GameManager._instance.MaxItemAmount : currentMaxItems;
            spawnInterval = GameManager._instance.ItemSpawnInterval > 0 ? GameManager._instance.ItemSpawnInterval : spawnInterval;
        }

        if (spawnInterval > 0 && GameManager.GAME_STATE != GameStatus.COUNTDOWN)
        {
            nextSpawn -= Time.deltaTime;

            if (nextSpawn < 0 && currentItems.Count < currentMaxItems)
            {
                GameObject item = RandomItem();
                //Vector3 pos = RandomLocation(spawnArea);
                Vector3 pos = RandomLocation.GetRandomLocationOnObject(spawnArea, dropHeight);

                SpawnItem(item, pos);
                nextSpawn = spawnInterval;
            }
        }

    }

    private void SpawnItem(GameObject item, Vector3 pos)
    {
        GameObject go = GameObject.Instantiate(item);
        spawnedItems.Add(go);
        currentItems.Add(go);
        go.transform.position = pos;
        if(itemsParent != null)
            go.transform.parent = itemsParent.transform;
    }

    private GameObject RandomItem()
    {
        return itemList[Random.Range(0, itemList.Length)];
    }

    public void RemoveItem(GameObject go)
    {
        if (currentItems.Contains(go))
        {
            currentItems.Remove(go);
            Destroy(go);
        }
    }
}
