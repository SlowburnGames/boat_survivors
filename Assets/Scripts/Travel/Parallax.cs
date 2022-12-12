using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Parallax : MonoBehaviour
{
    private float length, startpos;
    [SerializeField] GameObject camera;
    [SerializeField] float parallaxEffect;
    // [SerializeField] float idleSpeed;
    [SerializeField] Button sailButton;
    private bool sail = false;
    // [SerializeField] Image image;
    // Start is called before the first frame update
    void Start()
    {
        startpos = transform.position.x;
        // length = image.sprite.rect.width;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        float temp, distance;
        sailButton.onClick.AddListener(SetSail);
        if(sail) // MOVEMENT
        {
            temp = (camera.transform.position.x * (1 - parallaxEffect));
            distance = (camera.transform.position.x * parallaxEffect);
            transform.position = new Vector3(startpos + distance, transform.position.y, transform.position.z);
            if(temp > startpos + length)
            {
                startpos += length;
            }
            else if(temp < startpos - length)
            {
                startpos -= length;
            }
        }
        /*else // IDLE
        {
            temp = (camera.transform.position.x * (1 - idleSpeed));
            distance = (camera.transform.position.x * idleSpeed);
            //float temp = 1 - idleSpeed;
            //float distance = camera.transform.position.x * idleSpeed;
            transform.position = new Vector3(startpos + distance, transform.position.y, transform.position.z);
            if(temp > startpos + length)
            {
                startpos += length;
            }
            else if(temp < startpos - length)
            {
                startpos -= length;
            }
        }*/
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
