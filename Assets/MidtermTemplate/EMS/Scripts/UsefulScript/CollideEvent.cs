using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollideEvent : MonoBehaviour
{
    public UnityEvent UnityEvent;
    public ServoDeviceControl master;
    public bool isClick = false, isFake = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Button" && !isClick)
        {
            isClick = true;
            if(isFake) master.StimElectro();
            else UnityEvent?.Invoke();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.tag == "Button")
        {
            isClick = false;
        }
    }
}
