using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Event : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public int id;
    [SerializeField] public string title;
    [SerializeField] public string text;

    [SerializeField] public List<string> choices;
    [SerializeField] public string image_name;

    public Event(string[] input)
    {
        Debug.Log(input[1]);
        id = int.Parse(input[0]);
        title = input[1];
        text = input[2];
        for (int i = 3; i < 7; i++)
        {
            if(input[i] != "")
            {
                choices.Add(input[i]);
            }
        }
        image_name = input[7];
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
