using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EMS_option_Slide : MonoBehaviour
{
    public GameObject EmpAmp;
    public int max = 100;
    private Scrollbar Slider;
    private void Start() {
        Slider = GetComponent<Scrollbar>();
    }

    public void SetSlideValue()
    {
    
        EmpAmp.GetComponent<EMSAttribute>().SetSlideValue((int)(Slider.value*max));
    }
}
