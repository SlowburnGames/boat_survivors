using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    [SerializeField] private GameObject selectedHeroObject;
    [SerializeField] private GameObject tileObject;
    [SerializeField] private GameObject tileUnitObject;
    [SerializeField] private GameObject avaliableHeroes;
    
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
        selectedHeroObject.GetComponentInChildren<TextMeshProUGUI>().text = hero.UnitName;
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
            tileUnitObject.GetComponentInChildren<TextMeshProUGUI>().text = tile.tileUnit.UnitName;
            tileUnitObject.SetActive(true);
        }
    }

    public void UpdateHealthBar(BaseUnit unit)
    {
        var slider = unit.gameObject.GetComponentInChildren<Slider>();
        slider.value = (float)unit.Health / unit.MaxHealth;
    }

    public void ShowAvailableHeroes(List<string> myHeroes)
    {
        avaliableHeroes.GetComponentInChildren<TextMeshProUGUI>().text = String.Join(", ", myHeroes.ToArray());
        avaliableHeroes.SetActive(true);
    }

    public void UpdateAvailableHeroes(List<string> myHeroes)
    {
        avaliableHeroes.GetComponentInChildren<TextMeshProUGUI>().text = String.Join(", ", myHeroes.ToArray());
    }

    public void DisableAvailableHeroes()
    {
        avaliableHeroes.SetActive(false);
    }
    
    
}
