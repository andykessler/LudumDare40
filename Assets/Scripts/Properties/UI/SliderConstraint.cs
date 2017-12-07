using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderConstraint : MonoBehaviour {

    public Slider slider;
    public ConstraintType constraintType;

    private Slider self;

	// Use this for initialization
	void Start () {
        self = GetComponent<Slider>();	
	}

    public bool Holds()
    {
        bool holds = true;
        switch(constraintType)
        {
            case ConstraintType.LT:
                holds = self.value < slider.value;
                break;
            case ConstraintType.LTE:
                holds = self.value <= slider.value;
                break;
            case ConstraintType.EQ:
                holds = self.value == slider.value;
                break;
            case ConstraintType.GTE:
                holds = self.value >= slider.value;
                break;
            case ConstraintType.GT:
                holds = self.value > slider.value;
                break;
            default:
                holds = false; // or throw exception?
                break;
        }

        //if(!holds)
        //{
        //// slider color, error messages, etc;
        //}
        return holds;
    }
}


