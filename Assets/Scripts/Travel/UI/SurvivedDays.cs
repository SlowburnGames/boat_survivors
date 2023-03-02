using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SurvivedDays : MonoBehaviour
{
    // Start is called before the first frame update
    private TMP_Text text;

    void Start()
    {
        text = GetComponent<TMP_Text>();
        text.text = "You survived for " + Mathf.Floor(GameManager.Instance.travel_distance / 2) + " days...";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
