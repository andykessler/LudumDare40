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

    public void Set(string key, float value)
    {
        d[key] = value;
        //isDirty = true;
        Update();
    }

    // FIXME you have to remember to call this in constructor of children?
    protected abstract void Update();
}

