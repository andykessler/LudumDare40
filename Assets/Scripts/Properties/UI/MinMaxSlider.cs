using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MinMaxSlider : MonoBehaviour {

    public float min;
    public float max;

    public string minName;
    public string maxName;

    public EventTrigger.TriggerEvent customCallback;

    private Slider minSlider, maxSlider;

    private UnitPropertyManager manager;

    private Text text;

    // Use this for initialization
    void Start() {
        Slider[] s = GetComponentsInChildren<Slider>();
        manager = GetComponentInParent<UnitPropertyManager>();
        text = GetComponentInChildren<Text>();

        if (s.Length != 2) throw new MissingComponentException();

        minSlider = s[0];
        maxSlider = s[1];

        minSlider.minValue = min;
        minSlider.maxValue = max;

        maxSlider.minValue = min;
        maxSlider.maxValue = max;

        minSlider.value = min;
        maxSlider.value = max;

        UpdateText();

    }

    // this is a callback for us to know to alter settings files;
    public void HasChanged()
    {
        if (maxSlider.value < minSlider.value)
        {
            Slider temp = maxSlider;
            maxSlider = minSlider;
            minSlider = temp;
        }

        //text.text = String.Format("{0}: {1} - {2}", transform.parent.name, minSlider.value, maxSlider.value);
        //Debug.Log(String.Format("Updating {0} from {1} to {2}",
        //valueName, manager.ReadProperty(valueName), mainSlider.value));
        manager.UpdateProperty(minName, minSlider.value);
        manager.UpdateProperty(maxName, maxSlider.value);

        UpdateText();


        //BaseEventData eventData = new BaseEventData(EventSystem.current);
        //eventData.selectedObject = gameObject;
        //customCallback.Invoke(eventData);
    }

    void UpdateText()
    {
        text.text = String.Format("{0}: {1}\n{2}: {3}", 
            minName, manager.ReadProperty(minName),
            maxName, manager.ReadProperty(maxName));
    }

}
