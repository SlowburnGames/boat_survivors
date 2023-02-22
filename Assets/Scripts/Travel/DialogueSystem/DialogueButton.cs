using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueButton : MonoBehaviour
{
    // Start is called before the first frame update
    public int index;
    public bool ends_dialogue = false;
    public bool starts_combat = false;

    public void buttonPress()
    {
        if(ends_dialogue)
        {
            ends_dialogue = false;
            DialogueDisplay.instance.closeEvent();
            return;
        }

        if(starts_combat)
        {
            starts_combat = false;
            DialogueDisplay.instance.startCombat();
        }

        DialogueDisplay.instance.buttonPress(index);
    }


}
