using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ExitButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(exitGame);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void exitGame()
    {
        Debug.Log("it's working");
        Application.Quit();
    }
}
