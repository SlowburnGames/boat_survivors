using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipMovement : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] Button sailButton;
    private bool sail = false;
    float sinCenterY;
    public float amplitutde = 2;
    public float frequenzy = 0.5f;

    public bool inverted;

    // Start is called before the first frame update
    void Start()
    {
        sinCenterY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        sailButton.onClick.AddListener(SetSail);
        Vector2 pos = transform.position;
            float sin = Mathf.Sin(pos.x * frequenzy) * amplitutde;
            if(inverted)
            {
                sin = sin * -1;
            }
            pos.y = sinCenterY + sin;
            transform.position = pos;
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
