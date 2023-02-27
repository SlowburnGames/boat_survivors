using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectNames
{
    Barrier = 0,
    Rage = 1
}

[System.Serializable]
public class EffectList
{
    public int id;
    public EffectNames name;
    public int turnsLeft;
}

public class Effect : MonoBehaviour
{
    public static Effect Instance;

    public void Awake()
    {
        Instance = this;
    }

    public bool EditEffect(EffectList e, BaseUnit self)
    {
        if(e.turnsLeft > 0)
        {
            return false;
        }
        switch (e.name)
        {
            case EffectNames.Barrier:
                self.changeBlock();
                self.effects.RemoveAll(effect => effect.name == e.name);
                break;
            default:
                return false;
        }
        return true;
    }
}
