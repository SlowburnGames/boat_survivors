using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroDisplay : MonoBehaviour
{

    private BaseHero currentHero;
    public Slider healthSlider;
    public TMP_Text heroName;

    public Image heroPortrait;
    public TMP_Text healCostText;
    public Button healButton;
    public TMP_Text healthAmount;

    private int healthToMax;
    private int healCost;


    // Start is called before the first frame update
    void Start()
    {

    }

    public void updateDisplay(BaseHero hero)
    {
        currentHero = hero;
        heroName.SetText(hero.name);
        //set HP bar
        healthSlider.maxValue = hero.unitClass.maxHealth;
        healthSlider.value = hero.unitClass.health;
        Debug.Log(hero.Health);
        heroPortrait.sprite = hero.portrait;

        healthAmount.text = hero.unitClass.health + "/" + hero.unitClass.maxHealth;

        healthToMax = hero.unitClass.maxHealth - hero.unitClass.health;
        healCost = healthToMax * GameManager.Instance.healCost;

        if(hero.unitClass.health < hero.unitClass.maxHealth && GameManager.Instance.Resource >= healCost)
        {
            healButton.interactable = true;
            healCostText.text = "Cost:" + (healCost);
        }
        else
        {
            healButton.interactable = false;
            healCostText.text = "";
        }
    }

    public void healHero()
    {
        GameManager.Instance.healHero(currentHero, healthToMax);
        GameManager.Instance.addRes(-healCost);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
