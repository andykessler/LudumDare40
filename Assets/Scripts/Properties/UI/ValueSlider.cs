using System;
using UnityEngine;
using UnityEngine.UI; // Required when Using UI elements.

public class ValueSlider : MonoBehaviour // Inherit from Slider?
{

    public string valueName;

    private Slider mainSlider;

    private UnitPropertyManager manager;

    private Text text;

    private float lastValue;

    public void Start()
    {
        manager = GetComponentInParent<UnitPropertyManager>();
        mainSlider = GetComponent<Slider>();
        text = GetComponentInChildren<Text>();

        UpdateText();

        //Adds a listener to the main slider and invokes a method when the value changes.
        mainSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });

        lastValue = mainSlider.value;
    }

    // Invoked when the value of the slider changes.
    public void ValueChangeCheck()
    {
        
        if(!manager.UpdateProperty(valueName, mainSlider.value))
        {
            mainSlider.value = lastValue;
        }
        else
        {
            lastValue = mainSlider.value;
            UpdateText();
        }
    }

    public void UpdateText()
    {
        text.text = valueName + " : " + mainSlider.value.ToString();
    }
}