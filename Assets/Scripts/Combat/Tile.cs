using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Material baseMaterial, offsetMaterial;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private GameObject enemy1;
    
    public void Init(bool isOffset)
    {
        meshRenderer.material = isOffset ? offsetMaterial : baseMaterial;
    }
    public void OnMouseEnter()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }
    
    public void OnMouseExit()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void OnMouseDown()
    {
        Instantiate(enemy1, transform.position + Vector3.up * (1-enemy1.transform.localScale.y), Quaternion.identity);
    }
}
