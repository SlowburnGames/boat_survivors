using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrolling_Background : MonoBehaviour
{
    [SerializeField] float parallaxSpeed = 4f;
    Vector3 initPosition;
    // Start is called before the first frame update
    void Start()
    {
        // 1.51
        initPosition = transform.position;
    }

    // Update is called once per frame
    // -17.99877
    void Update()
    {
        transform.Translate(Vector3.left*parallaxSpeed*Time.deltaTime);
        if(transform.position.x <= -36.89)
        {
            transform.position = initPosition;
        }
    }
}
