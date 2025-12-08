using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderInteraction : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text text;
    [SerializeField] private bool showDecimal;

    private void Reset()
    {
        slider = GetComponent<Slider>();
        text = GetComponentInChildren<TMP_Text>();
    }

    public void HandleSliderValueChanged(float value)
    {
        if(showDecimal)
            text.SetText(value.ToString("F2"));
        else   
            text.SetText(value.ToString("F0"));
    }
}
