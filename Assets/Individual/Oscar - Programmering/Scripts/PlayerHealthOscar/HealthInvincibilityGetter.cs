using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthInvincibilityGetter : MonoBehaviour
{
    public HealthInvincibilitySO healthInvincibilitySo;

    

    public Toggle toggle;
    // Start is called before the first frame update
    public void Start()
    {
        //healthInvincibilitySo.damageReductionValue = 1;
        //Fetch the Toggle GameObject
        
        //Add listener for when the state of the Toggle changes, and output the state
        toggle.onValueChanged.AddListener(delegate {
            ToggleValueChanged(toggle);
        });
    }

    private void ToggleValueChanged(Toggle change)
    {

        if (!change.isOn)
        {
            healthInvincibilitySo.damageReductionValue = 1;
        }
        else
        {
            healthInvincibilitySo.damageReductionValue = 0;
        }

    }
}
