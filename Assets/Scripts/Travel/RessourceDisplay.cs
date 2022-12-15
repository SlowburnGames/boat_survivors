using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RessourceDisplay : MonoBehaviour
{

    [SerializeField] private Slider food_slider;
    [SerializeField] private Slider res_slider;

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
    }
}
