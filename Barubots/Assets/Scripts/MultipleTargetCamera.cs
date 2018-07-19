using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class MultipleTargetCamera : MonoBehaviour {

    public List<Transform> targets;

    public Vector3 offset;
    public float smoothTime = 0.5f;

    public float minZoom = 40f;
    public float maxZoom = 30f;
    public float zoomLimiter = 50f;

    private Camera cam;
    private Vector3 velocity;

    // Use this for initialization
    void Start () {
        targets = GameManager.Instance.GetAllRobots().ToList<Transform>();
        Debug.Log(targets.Count);
        cam = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (targets.Count == 0)
        {
            return;
        }

        Move();
        Zoom();
	}

    void Zoom()
    {
        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimiter);
        Debug.Log(newZoom);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime);
        Debug.Log(cam.fieldOfView);
    }

    private void Move()
    {
        Vector3 centerPoint = GetCenterPoint();

        Vector3 newPosition = centerPoint + offset;

        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }

    float GetGreatestDistance()
    {
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds.size.x;
    }

    private Vector3 GetCenterPoint()
    {
        if (targets.Count == 1)
        {
            return targets[0].position;
        }

        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds.center;
    }
}
