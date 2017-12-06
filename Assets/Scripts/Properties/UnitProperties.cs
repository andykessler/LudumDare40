using System.Collections.Generic;
using UnityEngine;

public abstract class UnitProperties
{
    // FIXME Don't assume everything is float!
    protected Dictionary<string, float> d;

    //protected bool isDirty;

    protected UnitProperties()
    {
         d = new Dictionary<string, float>();
    }

    public Dictionary<string, float> Get()
    {
        // return copy of our dictionary so cant change it without us knowing
        return new Dictionary<string, float>(d); 
    }

    public float Get(string key)
    {
        if(d.ContainsKey(key))
        {
            return d[key];
        }
        else
        {
            Debug.Log("Could not find property with key " + key);
            return 0f; // Throw exception instead?

        }
    }

    // Switch visibility to just package?
    public void Set(string key, float value)
    {
        d[key] = value;
        //isDirty = true;
        Update();
    }

    // FIXME you have to remember to call this in constructor of children?
    protected abstract void Update();

    // have update callback a func on UI change

    // use our own callback to broadcast to all
    // game objects of certain type to load from our new dict

    // in there we use hardcoded string key names (consistent)
    //      eventually move to classes that inherit this with string names with their default?
    // handle assignment of these values
    // if they would effect current gameplay we can adjust SOME of them
    // CANT adjust ball#, hunter#, player#, life#
    // CAN clamp maximum speed, accel, etc.
    // NEED single sliders for the singular values.

    // then next time make sure game obeys these values.
}

