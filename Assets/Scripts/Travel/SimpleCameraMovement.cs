using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleCameraMovement : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] Button sailButton;
    private bool sail = false;
    void Start()
    {
        
    }
    void Update()
    {
        sailButton.onClick.AddListener(SetSail);
        if(sail)
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
    }

    void SetSail()
    {
        sail = true;
        Invoke("SetIdle", 5);
        
    }
    
    void SetIdle()
    {
        sail = false;
    }
}
