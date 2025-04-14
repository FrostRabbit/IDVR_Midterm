using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class ServoDeviceInterface : MonoBehaviour
{
    [HelpBox("Synchronous servo control and the UI.", HelpBoxMessageType.Info)]

    public KeyCode SwitchSettingPanelKey = KeyCode.RightArrow;
    private ServoDeviceInterface deviceInterface;
    public static ServoDeviceInterface instance;
    public TMP_Text[] deviceStatusTxt;
    public TMP_Text[] deviceCommandTxt;
    public Slider[] progreeSlider;
    public GameObject[] progressBar;
    public static bool isSwitchAutoSetPanel = false;

    // Device-1 Pannel
    public GameObject[] d1_activeImg;
    public TMP_Text[] d1_intense;
    public TMP_Text[] d1_switchState;
    public GameObject[] d1_directSetInputField;
    public GameObject[] d1_autoSetIntenseInputField;
    public GameObject[] d1_autoTimeGapInputField;

    // Device-2 Pannel
    public GameObject[] d2_activeImg;
    public TMP_Text[] d2_intense;
    public TMP_Text[] d2_switchState;
    public GameObject[] d2_directSetInputField;
    public GameObject[] d2_autoSetIntenseInputField;
    public GameObject[] d2_autoTimeGapInputField;


    private void Awake()
    {
        this.deviceInterface = GetComponent<ServoDeviceInterface>();
        instance = deviceInterface;

        for (int i = 0; i < d1_activeImg.Length; i++) { 
            d1_activeImg[i].SetActive(false);
            d1_intense[i].SetText("...");
            d2_activeImg[i].SetActive(false);
            d2_intense[i].SetText("...");
        }

    }

    private void Update()
    {
        SwitchSettingPanel();
    }


    public void setStatusTxtForDevice(int _deviceCOM, string _ss)
    {
        deviceStatusTxt[_deviceCOM - 1].SetText("Device-" + _deviceCOM.ToString() + " Status: " + _ss);
    }

    public void setCommandTxtForDevice(int _deviceCOM, string _ss)
    {
        deviceCommandTxt[_deviceCOM - 1].SetText(_ss);
    }

    public void setStatusSetupProgressBar(int _deviceCOM, float setVal) {
        this.progreeSlider[_deviceCOM - 1].value = setVal;
    }

    public void activeDeviceChannelInterface(int _deviceCOM, int _channel, bool isActive) {
        if (_deviceCOM == 1)
            activeDevice1ChannelInterface(_channel, isActive);
        else activeDevice2ChannelInterface(_channel, isActive);
    }

    private void activeDevice1ChannelInterface(int _channel, bool isActive) {
        d1_activeImg[_channel-1].SetActive(isActive);
       if (isActive) d1_switchState[_channel - 1].SetText("OFF");
       else d1_switchState[_channel - 1].SetText("ON");
    }

    private void activeDevice2ChannelInterface(int _channel, bool isActive){
        d2_activeImg[_channel - 1].SetActive(isActive);
       if (isActive) d2_switchState[_channel - 1].SetText("OFF");
       else d2_switchState[_channel - 1].SetText("ON");
    }

    public void completeToCloseSliderBarAndAwakeUI(int _deviceCOM) {
        progressBar[_deviceCOM - 1].SetActive(false);
    }

    public void setChannelIntenseValue(int _deviceCOM, int _channel, int _intenseval) {
        if (_deviceCOM == 1)
            setDevice1ChannelValue(_channel, _intenseval);
        else setDevice2ChannelValue(_channel, _intenseval);
    }

    public void setDevice1ChannelValue(int _channel, int _intenseval) {
        d1_intense[_channel - 1].SetText(_intenseval.ToString());
    }

    public void setDevice2ChannelValue(int _channel, int _intenseval) {
        d2_intense[_channel - 1].SetText(_intenseval.ToString());
    }

    public int getDeviceInputFieldIntenseSet(int _deviceCOM, int _cmd) {
        if (_deviceCOM == 1)
        {
            string intense = d1_directSetInputField[_cmd - 1].GetComponent<TMP_InputField>().text;
            if (intense == "") return -1;
            return int.Parse(intense);
        }
        else {
            string intense = d2_directSetInputField[_cmd - 1].GetComponent<TMP_InputField>().text;
            if (intense == "") return -1;
            return int.Parse(intense);
        }
    }

    public int getDeviceAutoInputFieldIntenseSet(int _deviceCOM, int _cmd)
    {
        if (_deviceCOM == 1)
        {
            string intense = d1_autoSetIntenseInputField[_cmd - 1].GetComponent<TMP_InputField>().text;
            if (intense == "") return 0;
            return int.Parse(intense);
        }
        else
        {
            string intense = d2_autoSetIntenseInputField[_cmd - 1].GetComponent<TMP_InputField>().text;
            if (intense == "") return 0;
            return int.Parse(intense);
        }
    }

    public float getDeviceAutoInputFieldTimeGap(int _deviceCOM, int _cmd)
    {
        if (_deviceCOM == 1)
        {
            string intense = d1_autoTimeGapInputField[_cmd - 1].GetComponent<TMP_InputField>().text;
            if (intense == "") return 0.1f;
            return float.Parse(intense);
        }
        else
        {
            string intense = d2_autoTimeGapInputField[_cmd - 1].GetComponent<TMP_InputField>().text;
            if (intense == "") return 0.1f;
            return float.Parse(intense);
        }
    }

    private void SwitchSettingPanel() { 
        if(Input.GetKeyDown(SwitchSettingPanelKey)) {
            if (!isSwitchAutoSetPanel)
            {
                for (int i = 0; i < d1_directSetInputField.Length; i++) 
                {
                    d1_directSetInputField[i].SetActive(false);
                    d2_directSetInputField[i].SetActive(false);
                }

                for (int i = 0; i < d1_autoSetIntenseInputField.Length; i++) {
                    d1_autoSetIntenseInputField[i].SetActive(true);
                    d1_autoTimeGapInputField[i].SetActive(true);
                    d2_autoSetIntenseInputField[i].SetActive(true);
                    d2_autoTimeGapInputField[i].SetActive(true);
                }

            }
            else {
                for (int i = 0; i < d1_directSetInputField.Length; i++)
                {
                    d1_directSetInputField[i].SetActive(true);
                    d2_directSetInputField[i].SetActive(true);
                }

                for (int i = 0; i < d1_autoSetIntenseInputField.Length; i++)
                {
                    d1_autoSetIntenseInputField[i].SetActive(false);
                    d1_autoTimeGapInputField[i].SetActive(false);
                    d2_autoSetIntenseInputField[i].SetActive(false);
                    d2_autoTimeGapInputField[i].SetActive(false);
                }
            }
            isSwitchAutoSetPanel = !isSwitchAutoSetPanel;
        }
    }
 



}
