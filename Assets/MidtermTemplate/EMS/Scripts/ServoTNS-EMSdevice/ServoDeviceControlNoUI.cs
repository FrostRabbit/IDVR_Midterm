using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ServoDeviceControlNoUI : MonoBehaviour
{
    [HelpBox("Control the Servo by serial write. You can build your own stimulation function here.", HelpBoxMessageType.Info)]

    public int setBothIntense = 0;
    public int setCh1Intense = 0;
    public int setCh2Intense = 0;
    
    private ServoDeviceSetupNoUI deviceSetup;
    private int currDeviceId = 0;

    private int maxIntense = 202;
    private int minIntense = 0;
    private int[] currIntense = { 0, 0 };
    public bool isCancelStim = false;


    public float stim_dur = 0, stim_dur2 = 0;
    public int ampl = 0, ampl2 = 0, largestAMPL = 170, smallestAMPL = 140, testCH = 1;
    public Button Start1, Start2, StartBoth;
    public GameObject hint,hint2, testPanel;
    public Scrollbar test;
    public TextMeshProUGUI commandTxt, hintT1, hintT2, timeT, time2T;
    public TMP_InputField timeT_t, time2T_t;
    public bool isStim = false, isTest = false;


    private void Awake()
    {
        deviceSetup = GetComponent<ServoDeviceSetupNoUI>();
        currDeviceId = deviceSetup.deviceId;
    }

    private void Update()
    {
        if (!deviceSetup.isReadyForStim) return;

        // ServoDeviceInterface.instance.setChannelIntenseValue(currDeviceId, 1, currIntense[0]);
        // ServoDeviceInterface.instance.setChannelIntenseValue(currDeviceId, 2, currIntense[1]);
    }




////////////Stimulation Function example//////////////////
    //short-time electro , you need to set the amplitude by your test
    public void StimElectro()
    {
        if(!isStim)
        {
            stim_dur = 0.38f;
            int stim_amplitude = 35;
            int fixed_amplitude = (int)(stim_amplitude*2.02f); //35 amplitude for biceps up, depending on you
            StartCoroutine(StimWholeAction(fixed_amplitude, stim_dur, 0, 0));
            isStim = true;
        }
    }

    //pick up , you need to set the amplitude by your test
    public void StimPickUpL(int _ch, int min = 30, int max = 70, int degree = 2, float time_gap = 0.1f)
    {
        if(!isStim)
        {
            StartCoroutine(StimWithTimeGap(_ch, min, max, degree, time_gap)); //1 for biceps, 2 for carpis ulnaris
            isStim = true;
        }
    }

    //foot up , you need to set the amplitude by your test
    public void StimFootUp()
    {
        if(!isStim)
        {
            stim_dur = 10f;
            int stim_amplitude = 45;
            int fixed_amplitude = (int)(stim_amplitude*2.02f);
            StartCoroutine(StimWholeAction(fixed_amplitude, stim_dur, 0, 0f)); 
            isStim = true;
        }
    }
////////////Stimulation Function example//////////////////





////////////Stimulation Mechanism//////////////////
    //increase one unit
    public void IncreaseIntense(int _ch) {
        if (currIntense[_ch - 1] == maxIntense) return;
        currIntense[_ch - 1] += 2;
        if(_ch == 1) ampl = currIntense[_ch - 1];
        if(_ch == 2) ampl2 = currIntense[_ch - 1];
    }

    //decrease one unit
    public void DecreaseIntense(int _ch) {
        if (currIntense[_ch - 1] == minIntense) return;
        currIntense[_ch - 1] -= 2;
        if(_ch == 1) ampl = currIntense[_ch - 1];
        if(_ch == 2) ampl2 = currIntense[_ch - 1];
    }

    //set intense at value
    public void DirectSetIntense(int _ch, int _setIntense) {
        if (_setIntense > maxIntense || _setIntense < minIntense) return;
        currIntense[_ch - 1] = _setIntense;
        if(_ch == 1) ampl = _setIntense;
        if(_ch == 2) ampl2 = _setIntense;
    }

    //set both intense at value
    public void DirectSetBothIntense(int _setIntense) {
        if (_setIntense > maxIntense || _setIntense < minIntense) return;
        currIntense[0] = _setIntense;
        currIntense[1] = _setIntense;
        ampl = currIntense[0];
        ampl2 = currIntense[1];
    }

    //show test UI
    public void SetTestPanel(int which)
    {
        if(which == 1)
        {
            testCH = 1;
            testPanel.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
            testPanel.SetActive(true);
        }
        else
        {
            testCH = 2;
            testPanel.GetComponent<RectTransform>().anchoredPosition = new Vector3(399, 0, 0);
            testPanel.SetActive(true);
        }
    }

    //test user's comfortable amplitude by increasing 3 amplitude every 1 sec
    public void StimTest()
    {
        if(!isStim)
        {
            StartCoroutine(StimWithTimeGap(testCH, 0, 100, 3, 1f));
            isStim = true;
            isTest = true;
        }
    }

    //record the smallest amplitude and largest amplitide during stim test
    public void StimTestRecord(int which)
    {
        if(which == 1)
        {
            hint.transform.parent = hint.transform.parent.parent;
            hintT1.gameObject.name = "Finish";
            smallestAMPL = (testCH == 1)? ampl:ampl2;
            smallestAMPL = (int)((float)smallestAMPL/2.02f);
        }
        else
        {
            largestAMPL = (testCH == 1)? ampl:ampl2;
            largestAMPL = (int)((float)largestAMPL/2.02f);
            isCancelStim = true;
            isTest = false;
        }
    }

    //set ampl or stim_dur
    //100ma = 202rot = 202*0.00268=0.54136s => 來回 0.54136*2(s)
    public void SetValue(int _ch = 1, float _setValue = 30, string kind = "AMPL")
    {
        if(kind  == "STIM")
        {
            if(_ch == 1) stim_dur = _setValue;
            if(_ch == 2) stim_dur2 = _setValue;
            if(_ch == 1 && stim_dur < ampl*0.00268f*2)
            {
                stim_dur = ampl*0.00268f*2;
                // timeT_t.text = "";  //reset inputfield
            }
            if(_ch == 2 && stim_dur2 < ampl2*0.00268f*2)
            {
                stim_dur2 = ampl2*0.00268f*2;
                // time2T_t.text = ""; //reset inputfield
            }
        }
        if(kind  == "AMPL")
        {
            DirectSetIntense(_ch, (int)(_setValue*2.02f));
            if(_ch == 1 )
            {
                stim_dur = (_setValue*2.02f)*0.00268f*2;
                // timeT_t.text = ""; //reset inputfield
            }
            if(_ch == 2 )
            {
                stim_dur2 = (_setValue*2.02f)*0.00268f*2;
                // time2T_t.text = ""; //reset inputfield
            }
        }

        // timeT.text = stim_dur.ToString();
        // time2T.text = stim_dur2.ToString();
    }

    //start stim at target channel
    public void StartStim(int _ch = 1)
    {
        Debug.Log("start stim now");
        if(_ch == 1) StartCoroutine(StimWithTime(1, stim_dur));
        else
        {
            StartCoroutine(StimWithTime(2, 0, stim_dur2));
        }
    }

    //start stim at both channel at the same time
    public void StartStimBoth()
    {
        Debug.Log("start stim both now");
        StartCoroutine(StimWholeAction(ampl, stim_dur, ampl2, stim_dur2));
    }

    //stop stim at target channel (servo return to 0)
    public void StopStim(int _ch = 1)
    {
        isCancelStim = true;
        if(_ch == 0) StartCoroutine(StopStimWhich(testCH));
        StartCoroutine(StopStimWhich(_ch));
    }

    //long-time stimulate (stim with time gap), for pushing, pulling or stim test.
    IEnumerator StimWithTimeGap(int _ch, int min = 0, int max = 80, int degree = 2, float time_gap = 0.014f)
    {
        // hint.transform.parent = hint2.transform;
        // hint.GetComponent<RectTransform>().anchoredPosition = new Vector3(4.365015f, 10, 0);
        // hintT1.gameObject.name = "text";
        GiveCommandToArduinoBoard(_ch, (int)(min*2.02f));
        isCancelStim = false;
        Debug.Log("ActiveTimeGap");
        int nowAMPL = min;
        while(nowAMPL < max)
        {
            test.value = (_ch == 1)? (float)nowAMPL/100f:(float)nowAMPL/100f;
            // if(hintT1.gameObject.name != "Finish") hintT1.text = (_ch == 1)? nowAMPL.ToString():nowAMPL.ToString();
            // hintT2.text = (_ch == 1)? nowAMPL.ToString():nowAMPL.ToString();
            yield return new WaitForSeconds(time_gap);
            if(isCancelStim)
            {
                GiveCommandToArduinoBoard(_ch, 0);
                yield return new WaitForSeconds(nowAMPL*0.00268f);
                isStim = false;
                yield break;
            }
            nowAMPL+=degree;
            GiveCommandToArduinoBoard(_ch, (int)(nowAMPL*2.02f));
        }
    }

    //stim target channel
    IEnumerator StimWithTime(int _ch, float time = 0.1f, float time2 = 0)
    {
        isCancelStim = false;

        if(_ch == 1)
        {
            GiveCommandToArduinoBoard(1, ampl);
            yield return new WaitForSeconds(time);
            GiveCommandToArduinoBoard(1, 0);
            // Start1.interactable = true;
        }
        if(_ch == 2)
        {
            GiveCommandToArduinoBoard(2, ampl2);
            yield return new WaitForSeconds(time2);
            GiveCommandToArduinoBoard(2, 0);
            // Start2.interactable = true;
        }
        if(_ch == 3)
        {
            GiveCommandToArduinoBoard(3, ampl, ampl2);
            if(time > time2)
            {
                yield return new WaitForSeconds(time2);
                GiveCommandToArduinoBoard(3, -1, 0);
                yield return new WaitForSeconds(time-time2);
                GiveCommandToArduinoBoard(3, 0, -1);
            }
            else if(time < time2)
            {
                yield return new WaitForSeconds(time);
                GiveCommandToArduinoBoard(3, 0, -1);
                yield return new WaitForSeconds(time2-time);
                GiveCommandToArduinoBoard(3, -1, 0);
            }
            else
            {
                yield return new WaitForSeconds(time);
                GiveCommandToArduinoBoard(3, 0, 0);
            }
            // StartBoth.interactable = true;
        }
        // yield return new WaitForSeconds(2.0f);
        isStim = false;
    }

    //stop target channel
    IEnumerator StopStimWhich(int _ch = 1)
    {
        if (_ch == 1) {
            int temp = ampl;
            currIntense[0] = 0;
            ampl = 0;
            GiveCommandToArduinoBoard(1, 0);
            // ServoDeviceInterface.instance.activeDeviceChannelInterface(currDeviceId, 1, false);  
            yield return new WaitForSeconds(temp*0.00268f);
        }

        if (_ch == 2) {
            int temp = ampl2;
            currIntense[0] = 0;
            ampl2 = 0;
            GiveCommandToArduinoBoard(2, 0);
            // ServoDeviceInterface.instance.activeDeviceChannelInterface(currDeviceId, 2, false);
            yield return new WaitForSeconds(temp*0.00268f);
        }
    }

    //stim both channel
    IEnumerator StimWholeAction(float ampl, float time, float ampl2, float time2)
    {
        if(ampl != 0)
        {
            DirectSetIntense(1, (int)ampl);
        }
        if(ampl2 != 0)
        {
            DirectSetIntense(2, (int)ampl2);
        }

        StartCoroutine(StimWithTime(3, time, time2));

        yield return null;
    }

    //communicate with servo
    public void GiveCommandToArduinoBoard(int _channel, int _setIntense, int _setIntense2 = 0) {
        if (_channel < 1 || _channel > 3) return;
    
        string cmds;
        if(_channel == 1) cmds = _setIntense.ToString() + ",-1";
        else if(_channel == 2) cmds = "-1," + _setIntense.ToString();
        else cmds = _setIntense + "," + _setIntense2;
        if(deviceSetup.DeviceSerialPort.IsOpen)
        {
            deviceSetup.DeviceSerialPort.Write(cmds);
        }

        if (_channel == 3) ShowSystemConsole("Set Both Intense - " + _setIntense);
        else ShowSystemConsole("Set Channel " + _channel + " Intense - " + _setIntense);
    }

    public int getCurrChannelIntense(int ch) {
        return currIntense[ch - 1];
    }

    private void ShowSystemConsole(string _debugLine) {
        Debug.Log("(System) " + _debugLine);
        // commandTxt.text = _debugLine;
    }

    public bool isReadyForStimulate() {
        return deviceSetup.isReadyForStim;
    }

    public void OnApplicationQuit()
    {
        DirectSetBothIntense(0);
    }
////////////Stimulation Mechanism//////////////////
}
