using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericStatus : StatusEffect
{

    private int _morale_change;
    private int _resource_change;
    
    public GenericStatus(TravelManager t_mgr, int morale_change, int resource_change, int dur, bool inf = false) : base(t_mgr)
    {
        _morale_change = morale_change;
        _resource_change = resource_change;
        _duration = dur;
        _infinite = inf;
    }


  public override void applyStatus()
  {
    _travel_manager.addMorale(_morale_change);
    _travel_manager.addRes(_resource_change);
  }


}
