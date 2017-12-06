using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MinMaxSlider : MonoBehaviour {
    
    public float min;
    public float max;

    public EventTrigger.TriggerEvent customCallback;

    private Slider minSlider, maxSlider;

    private Text textRange;

    // Use this for initialization
    void Start () {
        Slider[] s = GetComponentsInChildren<Slider>();
        if (s.Length != 2) throw new MissingComponentException();

        minSlider = s[0];
        maxSlider = s[1];

        minSlider.minValue = min;
        minSlider.maxValue = max;

        maxSlider.minValue = min;
        maxSlider.maxValue = max;

        minSlider.value = min;
        maxSlider.value = max;
    }

    // this is a callback for us to know to alter settings files;
    public void HasChanged()
    {
        if(maxSlider.value < minSlider.value)
        {
            Slider temp = maxSlider;
            maxSlider = minSlider;
            minSlider = temp;
        }
        
        textRange.text = String.Format("{0}: {1} - {2}", transform.parent.name, minSlider.value, maxSlider.value);

        BaseEventData eventData = new BaseEventData(EventSystem.current);
        eventData.selectedObject = gameObject;
        customCallback.Invoke(eventData);
    }
}
