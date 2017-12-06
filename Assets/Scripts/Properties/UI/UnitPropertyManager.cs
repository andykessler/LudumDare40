using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnitPropertyManager : MonoBehaviour
{
    public string unitName;

    UnitProperties properties;

    Text text;
    MinMaxSlider[] minMaxSliders;
    Slider[] valSliders;

    // Use this for initialization
    void Start()
    {
        switch(unitName)
        {
            case "Player":
                properties = PlayerProperties.props;
                break;
            case "Opponent":
                properties = OpponentProperties.props;
                break;
            case "Hunter":
                properties = HunterProperties.props;
                break;
            case "Ball":
                properties = BallProperties.props;
                break;
            case "Game":
                properties = GameProperties.props;
                break;
            default:
                Debug.Log("No properties found for " + unitName);
                enabled = false;
                return;
        }
        
        text = GetComponentInChildren<Text>();
        text.text = properties.GetType().Name;

        // TODO Right now we have prebuilt UI elements, later should be dynamic.
        //for each range value create a new min max slider as child?
        //for each single value create a new slider as child?

    }

    public void UpdateProperties(string key, float value)
    {
        properties.Set(key, value);
    }
}
