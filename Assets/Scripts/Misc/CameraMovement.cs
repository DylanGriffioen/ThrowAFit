using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public List<Transform> targets;
    public GameObject[] players;
    public Vector3 offset = new Vector3(0, 8, -15);
    private Vector3 velocity;
    public float smoothTime = 0.5f;
    public float minZoom = 70f;
    public float maxZoom = 55f;
    public float zoomLimiter = 30f;
    private Camera cam;
    public List<Transform> copylist;

    Vector3 NewPosition = new Vector3(0, 20f, -10f);

    void Start()
    {
        cam = GetComponent<Camera>();
        FindPlayers();
    }
    public void FindPlayers()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            targets.Add(players[i].transform);
        }
    }

    void LateUpdate()
    {
        if (targets.Count == 0)
        {
            return;
        }
        Vector3 CenterPoint = GetCenterPoint();
        if (NewPosition.y <= 30f)
        {
            NewPosition = CenterPoint + offset;
        }
        if (NewPosition.y > 30f)
        {
            NewPosition = new Vector3(0, 20f, -20f);
        }
        transform.position = Vector3.SmoothDamp(transform.position, NewPosition, ref velocity, smoothTime);
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].position.y < 10f && targets.Count > 1)
            {
                copylist.Add(targets[i]);
                targets.Remove(targets[i]);
            }
        }
        for (int i = 0; i < copylist.Count; i++)
        {
            if (copylist[i].transform.position.y > 20f)
            {
                targets.Add(copylist[i]);
                copylist.Remove(copylist[i]);
            }
        }

        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimiter);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime);
    }
    float GetGreatestDistance()
    {
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }
        return Mathf.Max(bounds.size.x, bounds.size.z);
    }

    Vector3 GetCenterPoint()
    {
        if (targets.Count == 1)
        {
            return targets[0].position;
        }
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].position.y > 10f && targets[i].position.y < 20f)
            {
                bounds.Encapsulate(targets[i].position);
            }
        }
        return bounds.center;
    }
}