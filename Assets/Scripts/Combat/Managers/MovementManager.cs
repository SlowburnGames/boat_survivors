using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    private float time;
    private float t;
    Vector3[] points;
    public bool move;
    Vector3 startPosition;

    int currentWaypoint = 0;

    public void Awake()
    {
        
    }

    public void moveUnit(Vector3[] pos, float duration)
    {
        print("Moving " + this.name + " to pos: " + pos[pos.Length - 1]);
        time = duration;
        t = 0;
        currentWaypoint = 0;
        startPosition = transform.position;
        points = pos;
        move = true;
    }

    private void Update()
    {
        if(move)
        {
            t += Time.deltaTime / (time/points.Length);
            transform.position = Vector3.Lerp(startPosition, points[currentWaypoint], t);
            //Quaternion.LookRotation(FindObjectOfType<Camera>().transform.position - transform.position);
            //this.transform.LookAt(FindObjectOfType<Camera>().transform.position, Vector3.up);
            this.transform.rotation = Quaternion.Euler(new Vector3(10, -45, 0));
            if (Vector3.Distance(transform.position, points[currentWaypoint]) < 0.01f)
            {   if(currentWaypoint >= points.Length - 1)
                {
                    move = false;
                }
                t = 0;
                startPosition = transform.position;
                currentWaypoint++;
            }
        }

    }
}
