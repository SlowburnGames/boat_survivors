using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class RessourceDisplay : MonoBehaviour
{

    [SerializeField] private Slider food_slider;
    
    [SerializeField] private TMP_Text food_text;
    [SerializeField] private Slider res_slider;
    [SerializeField] private TMP_Text res_text;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateUI(int food, int ressources)
    {
        food_slider.value = food;
        res_slider.value = ressources;
        food_text.text = "" + food;
        res_text.text = "" + ressources;
    }
}
