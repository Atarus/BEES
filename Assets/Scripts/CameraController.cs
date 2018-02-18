using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public Transform target;
    float cameraSpeed = 5;

    void Start()
    {
        target = this.transform;
    }

    // Update is called once per frame
    void LateUpdate () {
        transform.position = Vector3.Lerp(transform.position, new Vector3(target.position.x, target.position.y, -10), cameraSpeed);
	}
}
