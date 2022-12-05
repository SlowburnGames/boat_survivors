using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    [SerializeField] private GameObject selectedHeroObject, tileObject, tileUnitObject;
    
    private void Awake()
    {
        Instance = this;
    }
    
    public void ShowSelectedHero(BaseHero hero)
    {
        if (hero == null)
        {
            selectedHeroObject.SetActive(false);
            return;
        }
        selectedHeroObject.GetComponentInChildren<TextMeshProUGUI>().text = hero.unitName;
        selectedHeroObject.SetActive(true);
    }

    public void ShowTileInfo(Tile tile)
    {
        if (tile == null)
        {
            tileObject.SetActive(false);
            tileUnitObject.SetActive(false);
            return;
        }
        
        tileObject.GetComponentInChildren<TextMeshProUGUI>().text = tile.tileName;
        tileObject.SetActive(true);

        if (tile.tileUnit)
        {
            tileUnitObject.GetComponentInChildren<TextMeshProUGUI>().text = tile.tileUnit.unitName;
            tileUnitObject.SetActive(true);
        }
    }
}
