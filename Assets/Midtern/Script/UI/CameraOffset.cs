using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOffset : MonoBehaviour
{
    public Transform target; //Purpose of the object
    public Vector2 angle; //Purpose of the object
    public Vector3 offset; //Purpose of the object
    public float distance = 5; //Distance


    private void Update()
    {
        Quaternion rotation = Quaternion.Euler(angle.x, angle.y, 0);
        Vector3 position = rotation * new Vector3(0, 0, -distance) + target.position + offset;

        transform.position = position;
        transform.rotation = rotation;
    }
}
