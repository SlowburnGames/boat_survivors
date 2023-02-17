using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    [SerializeField] private GameObject _selectedHeroObject;
    [SerializeField] private GameObject _placeHeroes;
    [SerializeField] private GameObject _tileObject;
    [SerializeField] private GameObject _tileUnitObject;
    [SerializeField] private GameObject _avaliableHeroes;
    [SerializeField] private GameObject _avaliableHeroesImg;

    [SerializeField] private GameObject _endTurnButton;

    [SerializeField] private GameObject _combatEndScreen;

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
        var hero_image = _selectedHeroObject.transform.Find("HeroImage").gameObject.GetComponent<Image>();
        hero_image.sprite = hero.GetComponent<SpriteRenderer>().sprite;
        _selectedHeroObject.GetComponentInChildren<TextMeshProUGUI>().text = hero.UnitName + "\n" + hero.UnitDescription;
        _selectedHeroObject.SetActive(true);
        updateAttacks(hero);
        var ability_button = _selectedHeroObject.transform.Find("HeroAbility").gameObject;
        ability_button.gameObject.SetActive(false);
        if(true/*hero.standAction*/)
        {
            ability_button.transform.Find("HeroAbilityText").GetComponent<TMP_Text>().SetText(hero.StandActionName);
            ability_button.gameObject.SetActive(true);
        }
        var heroHPDisplay = _selectedHeroObject.transform.Find("HP");
        string heroHPText = hero.Health + "/" + hero.MaxHealth;
        heroHPDisplay.transform.Find("HeroHP").gameObject.GetComponent<TMP_Text>().SetText(heroHPText);
        var heroSpeedDisplay = _selectedHeroObject.transform.Find("Speed");
        string heroSpeedText = hero.MoveDistance - hero.tilesWalked + "/" + hero.MoveDistance;
        heroSpeedDisplay.transform.Find("HeroSpeed").gameObject.GetComponent<TMP_Text>().SetText(heroSpeedText);

        updataAbility(hero);

        updateInitiative();
    }

    public void updateInitiative()
    {
        var initiativeDisplay = _selectedHeroObject.transform.Find("Initiative");

        List<string> initList = new List<string>();
        string last = CombatManager.Instance._turnQueue.ToArray().Last().name.Remove(
            CombatManager.Instance._turnQueue.ToArray().Last().name.Length - 7);
        initList.Add(last);
        int maxShow = 3;
        foreach (var currentUnit in CombatManager.Instance._turnQueue)
        {
            if (currentUnit.name.Remove(currentUnit.name.Length - 7) == last)
                break;
            maxShow--;
            initList.Add(currentUnit.name.Remove(currentUnit.name.Length - 7));
            if (maxShow <= 0)
                break;
        }   

        string initiativeText = "Next: " + String.Join(", ", initList);
        initiativeDisplay.transform.Find("Text").gameObject.GetComponent<TMP_Text>().SetText(initiativeText);
    }

    public void updataAbility(BaseHero hero)
    {
        var ability_button = _selectedHeroObject.transform.Find("HeroAbility").gameObject;

        if(hero.cooldown != 0)
        {
            ability_button.GetComponent<Button>().enabled = false;
            ability_button.transform.Find("HeroAbilityText").gameObject.SetActive(false);
            ability_button.transform.Find("AbilityCooldown").gameObject.SetActive(true);
            ability_button.transform.Find("AbilityCooldown").GetComponent<TMP_Text>().SetText(hero.cooldown.ToString());
        }
        else
        {
            ability_button.GetComponent<Button>().enabled = true;
            ability_button.transform.Find("HeroAbilityText").gameObject.SetActive(true);
            ability_button.transform.Find("AbilityCooldown").gameObject.SetActive(false);
        }  
    }

    public void updateAttacks(BaseUnit unit)
    {
        _selectedHeroObject.transform.Find("Attack").Find("HeroAttacks").GetComponent<TMP_Text>().SetText(unit.AttacksMade.ToString() + "/" + unit.MaxAttacks.ToString());
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

    public List<String> heroListToStringList(List<BaseHero> heroes)
    {
        List<String> heroNames = new List<string>();
        foreach (var hero in heroes)
            heroNames.Add(hero.name);

        return heroNames;
    }

    public void ShowAvailableHeroes(List<BaseHero> myHeroes)
    {
        List<String> myHeroNames = heroListToStringList(myHeroes);
        
        _avaliableHeroes.GetComponentInChildren<TextMeshProUGUI>().text = String.Join(", ", myHeroNames.ToArray());
        _avaliableHeroes.SetActive(true);

        _placeHeroes.SetActive(true);
        // Instantiate(_fighterImage, _avaliableHeroesImg.transform);

    }

    public void UpdateAvailableHeroes(List<BaseHero> myHeroes)
    {
        // TODO: Fix, and use portraits
        List<String> myHeroNames = heroListToStringList(myHeroes);
        _avaliableHeroes.GetComponentInChildren<TextMeshProUGUI>().text = String.Join(", ", myHeroNames.ToArray());
        _placeHeroes.SetActive(true);
    }

    public void DisableAvailableHeroes()
    {
        _avaliableHeroes.SetActive(false);
        _placeHeroes.SetActive(false);
    }

    public void openVictoryScreen()
    {
        var stats = _combatEndScreen.transform.Find("Stats").GetComponent<TMP_Text>();
        stats.SetText("Rewards:\nMorale:\t"+ GameManager.Instance.combatMoraleReward + "\nResources:\t" + GameManager.Instance.combatResReward);

        _combatEndScreen.SetActive(true);
    }

    public void victoryReturnButton()
    {
        SceneManager.LoadScene("Travel");
    }
    
    
}
