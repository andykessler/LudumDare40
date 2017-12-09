using UnityEngine;

public class SettingsController : MonoBehaviour {

    private static KeyCode[] hotkeys = {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5
    };

    private int currManager;
    private UnitPropertyManager[] managers;

    void Start () {
        managers = GetComponentsInChildren<UnitPropertyManager>();
        for(int i=0; i<managers.Length; i++)
        {
            managers[i].gameObject.SetActive(false);
        }
        ShowManager(-1); // Hide all managers to start
        currManager = -1;
	}
	
	void Update () {
        for(int i=0; i<managers.Length; i++)
        {
            if(Input.GetKeyUp(hotkeys[i]))
            {
                GameObject go = managers[i].gameObject;
                if(currManager == i)
                {
                    go.SetActive(false);
                }
                else
                {
                    currManager = i;
                    ShowManager(i);
                }
                // only handle one input at a time
                // not sure if multiple possible for key up
                return; 
            }
        }
	}

    void ShowManager(int index)
    {
        for(int i=0; i<managers.Length; i++)
        {
            managers[i].gameObject.SetActive(i == index);
        }
    }
}
