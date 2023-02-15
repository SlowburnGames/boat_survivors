using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Base Ability", menuName = "Ability/Base")]

public class Ability : ScriptableObject
{
    public string abilityName;
    public int cooldown;
    public string description;
    public string effectName;
    //public AbilityBaseClass ability;

    public virtual void Activate(BaseUnit self)
    {

    }
}
