using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroDisplay : MonoBehaviour
{
    public Slider healthSlider;
    public TMP_Text heroName;

    public Image heroPortrait;


    // Start is called before the first frame update
    void Start()
    {

    }

    public void updateDisplay(BaseHero hero)
    {
        heroName.SetText(hero.name);
        //set HP bar
        healthSlider.maxValue = hero.unitClass.maxHealth;
        healthSlider.value = hero.unitClass.health;
        Debug.Log(hero.Health);
        heroPortrait.sprite = hero.portrait;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
