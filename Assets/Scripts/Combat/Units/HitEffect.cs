using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : MonoBehaviour
{
    private Material normal;
    public Material red;
    public Material white;

    private float hitDuration = 0.05f;

    public void Start()
    {
        normal = transform.GetComponent<SpriteRenderer>().material;
    }
    public IEnumerator hitFlash()
    {
        transform.GetComponent<SpriteRenderer>().material = red;
        yield return new WaitForSeconds(0.05f);
        transform.GetComponent<SpriteRenderer>().material = white;
        yield return new WaitForSeconds(0.05f);
        transform.GetComponent<SpriteRenderer>().material = red;
        yield return new WaitForSeconds(0.05f);
        transform.GetComponent<SpriteRenderer>().material = normal;
    }
}
