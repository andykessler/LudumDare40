using System;
using UnityEngine;
using UnityEngine.UI; // Required when Using UI elements.

public class ValueSlider : MonoBehaviour
{
    public Slider mainSlider;

    public string valueName;

    private UnitPropertyManager manager;

    private Text text;

    public void Start()
    {
        manager = GetComponentInParent<UnitPropertyManager>();
        mainSlider = GetComponent<Slider>();
        text = GetComponentInChildren<Text>();
        UpdateText();

        //Adds a listener to the main slider and invokes a method when the value changes.
        mainSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
    }

    // Invoked when the value of the slider changes.
    public void ValueChangeCheck()
    {
        Debug.Log(String.Format("Updating {0} from {1} to {2}",
            valueName, manager.ReadProperty(valueName), mainSlider.value));
        manager.UpdateProperty(valueName, mainSlider.value);
        UpdateText();
    }

    public void UpdateText()
    {
        text.text = valueName + " : " + mainSlider.value.ToString();
    }
}