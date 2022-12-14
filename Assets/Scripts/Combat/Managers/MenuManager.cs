using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    [SerializeField] private GameObject _selectedHeroObject;
    [SerializeField] private GameObject _tileObject;
    [SerializeField] private GameObject _tileUnitObject;
    [SerializeField] private GameObject _avaliableHeroes;
    [SerializeField] private GameObject _avaliableHeroesImg;

    [SerializeField] private GameObject _endTurnButton;

    [SerializeField] private GameObject _fighterImage;
    
    
    
    private void Awake()
    {
        Instance = this;
    }


    public void toggleButtons(bool active)
    {
        _endTurnButton.gameObject.SetActive(active);
    }
    
    public void ShowSelectedHero(BaseHero hero)
    {
        if (hero == null)
        {
            _selectedHeroObject.SetActive(false);
            return;
        }
        _selectedHeroObject.GetComponentInChildren<TextMeshProUGUI>().text = hero.UnitName;
        _selectedHeroObject.SetActive(true);
    }

    public void ShowTileInfo(Tile tile)
    {
        if (tile == null)
        {
            _tileObject.SetActive(false);
            _tileUnitObject.SetActive(false);
            return;
        }
        
        _tileObject.GetComponentInChildren<TextMeshProUGUI>().text = tile.tileName;
        _tileObject.SetActive(true);

        if (tile.tileUnit)
        {
            _tileUnitObject.GetComponentInChildren<TextMeshProUGUI>().text = tile.tileUnit.UnitName;
            _tileUnitObject.SetActive(true);
        }
    }

    public void UpdateHealthBar(BaseUnit unit)
    {
        var slider = unit.gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
        slider.fillAmount = (float)unit.Health / unit.MaxHealth;
    }

    public void ShowAvailableHeroes(List<string> myHeroes)
    {
        _avaliableHeroes.GetComponentInChildren<TextMeshProUGUI>().text = String.Join(", ", myHeroes.ToArray());
        _avaliableHeroes.SetActive(true);

        // Instantiate(_fighterImage, _avaliableHeroesImg.transform);

    }

    public void UpdateAvailableHeroes(List<string> myHeroes)
    {
        _avaliableHeroes.GetComponentInChildren<TextMeshProUGUI>().text = String.Join(", ", myHeroes.ToArray());
    }

    public void DisableAvailableHeroes()
    {
        _avaliableHeroes.SetActive(false);
    }
    
    
}
