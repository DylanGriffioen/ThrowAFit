using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomLocation
{
    public static Vector3 GetRandomLocationOnObject(GameObject spawnArea, float dropHeight)
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

        float x = Random.Range(-length / 2, length / 2) + spawnArea.transform.position.x;
        float y = height + spawnArea.transform.position.y + dropHeight;
        float z = Random.Range(-width / 2, width / 2) + spawnArea.transform.position.z;

        //Debug.Log($"ori: {origin}, length: {length}, width: {width}, height: {height}");
        //Debug.Log($"x: {x}, z: {z}, y: {y}");

        return new Vector3(x, y, z);
    }
    public static Vector3 GetRandomLocationOnObject(GameObject spawnArea, float distanceToEdge, float dropHeight)
    {
        Vector3 origin = spawnArea.transform.position;

        BoxCollider collider = spawnArea.GetComponent<BoxCollider>();

        float length = spawnArea.transform.localScale.x - distanceToEdge;
        float width = spawnArea.transform.localScale.z - distanceToEdge;
        float height = spawnArea.transform.localScale.y;

        if (collider != null)
        {
            length *= collider.size.x;
            width *= collider.size.z;
            height *= collider.size.y;
        }

        float x = Random.Range(-length / 2, length / 2) + spawnArea.transform.position.x;
        float y = height + spawnArea.transform.position.y + dropHeight;
        float z = Random.Range(-width / 2, width / 2) + spawnArea.transform.position.z;

        //Debug.Log($"ori: {origin}, length: {length}, width: {width}, height: {height}");
        //Debug.Log($"x: {x}, z: {z}, y: {y}");

        return new Vector3(x, y, z);
    }
}
