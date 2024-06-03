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

        if (collider == null)
            return Vector3.zero;

        float max_X = (spawnArea.transform.localScale.x * collider.size.x) / 2 * (distanceToEdge * 0.01f);
        float min_X = -max_X;

        float max_Z = (spawnArea.transform.localScale.z * collider.size.z) / 2 * (distanceToEdge * 0.01f);
        float min_Z = -max_Z;

        float height = (spawnArea.transform.localScale.y * collider.size.y);


        float x = Random.Range(min_X, max_X) + origin.x;
        float z = Random.Range(min_Z, max_Z) + origin.z;
        float y = height + origin.y + dropHeight;

        Debug.Log($"ori: {origin}, x: {min_X} / {max_X}, y: {min_Z} / {max_X}");
        Debug.Log($"x: {x}, z: {z}, y: {y}");

        return new Vector3(x, y, z);
    }
}
