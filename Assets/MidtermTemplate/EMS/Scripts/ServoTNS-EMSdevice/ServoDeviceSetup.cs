using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO.Ports;
using System.Threading;

public class ServoDeviceSetup : MonoBehaviour
{
    [HelpBox("Enable Serial Port at beginning. Remember to check your COM_ID!", HelpBoxMessageType.Info)]

    public EMSAttribute AMPL1;
    public EMSAttribute Freq1, PDur1, Stim1;
    public EMSAttribute AMPL2, Freq2, PDur2, Stim2;
    public GameObject PanelIni;
    public int deviceId = 1;
    [SerializeField] public int COM_ID;
    private int baudRate = 115200;
    int dataBits = 8;
    StopBits stopBits = StopBits.One;
    Parity parity = Parity.None;
    [HideInInspector] public SerialPort DeviceSerialPort;

    [HideInInspector] public bool isReadyForStim = false;
    float times = 0;

    private void Start()
    {
        EnableDeviceSerialPort();
    }

    private void Update()
    {
        if (!isReadyForStim)
            WaitAndReadDeviceSetupCommand();
    }


    private void WaitAndReadDeviceSetupCommand()
    {
        if (DeviceSerialPort.IsOpen)
        {
            times += Time.deltaTime;
            string cmd = DeviceSerialPort.ReadExisting();
            ServoDeviceInterface.instance.setStatusTxtForDevice(deviceId, "Setup...");
            ServoDeviceInterface.instance.setCommandTxtForDevice(deviceId, cmd);
            ServoDeviceInterface.instance.setStatusSetupProgressBar(deviceId, times);

            //set Initial Channel1
            AMPL1.SetNow(0);
            Freq1.SetNow(90);
            PDur1.SetNow(200);
            Stim1.SetNow(0);

            //set Initial Channel2
            AMPL2.SetNow(0);
            Freq2.SetNow(90);
            PDur2.SetNow(200);
            Stim2.SetNow(0);

            isReadyForStim = true;
            ServoDeviceInterface.instance.setStatusTxtForDevice(deviceId, "Complete");
            ServoDeviceInterface.instance.setCommandTxtForDevice(deviceId, "Ready to Stim");
            ServoDeviceInterface.instance.completeToCloseSliderBarAndAwakeUI(deviceId);
            PanelIni.SetActive(false);

            Thread.Sleep(100);
        }
        else { 
            Debug.LogWarning("Device " + deviceId.ToString() + " - Serial Not Open Yet. Please recheck the COM Port...");
            ServoDeviceInterface.instance.setStatusTxtForDevice(deviceId, "No Serial");
        }
        
    }


    private void EnableDeviceSerialPort() 
    {
        this.DeviceSerialPort = new SerialPort("COM" + COM_ID.ToString(), baudRate, parity, dataBits, stopBits);
        try
        {
            DeviceSerialPort.Open();
            Debug.Log("(System) Open Device_" + deviceId.ToString() + " Serial Port Success.");
        }
        catch (Exception emsg) {
            Debug.LogWarning("Warning: " + emsg.Message);
        }
    
    }

    private void CloseDeviceSerialPort()
    {
        try
        {
            DeviceSerialPort.Close();
            Debug.LogWarning("(System) Close Device_" + deviceId.ToString() + " Serial Port Success.");
        }
        catch (Exception emsg) {
            Debug.LogWarning("Warning: " + emsg.Message);
        }
    
    }

    private void OnApplicationQuit()        // close the application, then close serial reader port
    {
        CloseDeviceSerialPort();
    }


}
