using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HandRecorder : MonoBehaviour
{
    public List<GameObject> VRHand;

    public List<GameObject> IKHand;
    public Vector3 trackingRotationOffset;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAllPosition();
    }

    void SyncPosition(GameObject g1, GameObject g2)
    {
        g1.transform.position = g2.transform.position;
        g1.transform.rotation = g2.transform.rotation * Quaternion.Euler(trackingRotationOffset);
    }
    void UpdateAllPosition()
    {
        for (int i = 0; i < VRHand.Count; i++)
        {
            UpdatePosition(IKHand[i], VRHand[i]);
        }
    }
    void UpdatePosition(GameObject g1, GameObject g2)
    {
        Transform g1_child1 = g1.transform.GetChild(0);
        Transform g1_child2 = g1_child1.GetChild(0);
        Transform g2_child1 = g2.transform.GetChild(0);
        Transform g2_child2 = g2_child1.GetChild(0);
        SyncPosition(g1, g2);
        SyncPosition(g1_child1.gameObject, g2_child1.GetChild(0).gameObject);
        SyncPosition(g1_child2.gameObject, g2_child2.GetChild(0).gameObject);
    }
}
