using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EMSAttribute : MonoBehaviour
{
    [HelpBox("UI Event to trigger ServoDeviceControl.", HelpBoxMessageType.Info)]

    public ServoDeviceControl EMSMaster;
    public GameObject Block, SwitchOn, SwitchOff;
    public int ch_id = 1;
    public string kind = "AMPL";
    public float interval = 1, nowV = 0;
    public TMP_InputField nowValue, setValue;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetNow(float value)
    {
        if(value == -1) value = (float)nowV;
        nowValue.text = value.ToString();
        nowV = value;
    }
 
    public void SetUp()
    {
        setValue.text = (nowV + interval).ToString();
        nowV += interval;
    }

    public void SetDown()
    {
        setValue.text = (nowV - interval).ToString();
        nowV -= interval;
    }

    public void SendSet()
    {
        EMSMaster.SetValue(ch_id, float.Parse(setValue.text), kind);
        SetNow(float.Parse(setValue.text));
    }

    public void SetSlideValue(int value)
    {
        Debug.Log("SetSlideValue: " + value);
        setValue.text = (value).ToString();
        nowV = (float)value;
    }
    public void SendStart()
    {
        EMSMaster.StartStim(ch_id);
    }

    public void SendStartBoth()
    {
        EMSMaster.StartStimBoth();
    }

    public void SendStop()
    {
        EMSMaster.StopStim(ch_id);
    }

    public void SwitchChannel()
    {
        if(!Block.active)
        {
            EMSMaster.SetValue(ch_id, 0, "ENAB");
            Block.SetActive(true);
            SwitchOn.SetActive(true);
            SwitchOff.SetActive(false);
        }
        else
        {
            EMSMaster.SetValue(ch_id, 1, "ENAB");
            Block.SetActive(false);
            SwitchOn.SetActive(false);
            SwitchOff.SetActive(true);
        }
    }
}
