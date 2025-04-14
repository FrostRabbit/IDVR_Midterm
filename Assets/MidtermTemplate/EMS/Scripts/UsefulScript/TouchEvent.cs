using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TouchEvent : MonoBehaviour
{
    public AudioSource sound;
    public UnityEvent UnityEvent;
    public ServoDeviceControl master;
    public bool isTouch = false;
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
        if(other.gameObject.layer == 13 && !isTouch)
        {
            isTouch = true;
            master.StimElectro();
            if(!sound.isPlaying) sound.Play();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == 13)
        {
            isTouch = false;
        }
    }
}
