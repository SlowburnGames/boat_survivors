using UnityEngine;

public class StatusEffect
{
    //Von dieser Klasse ableiten um neuen Statuseffekt zu bauen.
    //applyStatus() overriden um eigenen effekt zu programmieren
    //wenn der status nur eine zeit lang Moral/Resourcen verändern soll,
    //bitte GenericStatus Klasse benutzen.
    public string _name = "Status Effect";
    public string _description;

    //für anzeige, wenn true wird es als positiv angezeigt sonst negativ
    public bool _isGood;

    //wenn true, wird der status nicht angezeigt
    public bool _hidden = false;
    public string _icon;
    public int _duration;

    public bool _infinite = false;
    public TravelManager _travel_manager;

    public StatusEffect(TravelManager t_mgr)
    {
        _travel_manager = t_mgr;
    }

    public void applyStatusTick()
    {
        applyStatus();
        if(!_infinite)
        {
            countDuration();
        }
    }

    public virtual void applyStatus()
    {
        Debug.Log("Applied Status!");
    }

    private void countDuration()
    {
        _duration--;
    }
}
