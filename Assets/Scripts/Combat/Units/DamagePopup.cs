using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;
    private float x;
    private float y;
    private float drift;
    public Color damageColor;
    public Color critColor;
    public Color healColor;
    int dir;

    public static DamagePopup Create(Vector3 pos, float amount, int typ, string prefix = "")
    {
        GameObject damagePopupOBJ = Instantiate(GameAssets.i.damagePopupPrefab, pos, Quaternion.identity);
        //Quaternion.LookRotation(FindObjectOfType<Camera>().transform.position - damagePopupOBJ.transform.position);
        //damagePopupOBJ.transform.LookAt(FindObjectOfType<Camera>().transform.position, Vector3.up);
        damagePopupOBJ.transform.rotation = Quaternion.Euler(new Vector3(10, -45, 0));
        DamagePopup damagePopup = damagePopupOBJ.GetComponent<DamagePopup>();
        damagePopup.Setup(amount, typ, prefix);

        return damagePopup;
    }

    public void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
    }

    public void Setup(float amount, int typ, string prefix)
    {
        if(typ == 0)
        {
            textMesh.color = damageColor;
            if(Mathf.RoundToInt(amount) > 0)
            {
                textMesh.SetText(prefix + Mathf.RoundToInt(amount).ToString());
            }
            else
            {
                textMesh.SetText(prefix + "0.5");
            }
            
        }
        else if (typ == 1)
        {
            textMesh.color = critColor;
            textMesh.SetText(prefix + Mathf.RoundToInt(amount).ToString());
        }
        else
        {
            textMesh.color = healColor;
            textMesh.SetText(prefix + (amount).ToString("F1"));
        }
        
        textColor = textMesh.color;
        disappearTimer = 0.5f;
        x = 0.5f;
        y = 2f;
        drift = Random.Range(0f, 0.5f);
        dir = Random.Range(1, 3);
    }

    private void Update()
    {
        x *= 1 + (drift * Time.deltaTime);
        
        if(dir%2 == 0)
        {
            transform.position += new Vector3(x, y, 0) * Time.deltaTime;
        }
        else
        {
            transform.position += new Vector3(-x, y, 0) * Time.deltaTime;
        }

        disappearTimer -= Time.deltaTime;

        if(disappearTimer < 0)
        {
            float disappearSpeed = 3f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if(textColor.a < 0)
            {
                Destroy(this.gameObject);
            }
        }
    }

}
