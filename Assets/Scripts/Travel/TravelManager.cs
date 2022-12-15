using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class TravelManager : MonoBehaviour
{
    public static TravelManager instance {get; private set; }

    [Header("Resources/Resource Display")]
    [SerializeField]public RessourceDisplay resource_display;
    [SerializeField]private Button[] buttons;

    [Header("Dialogue System")]
    private DialogueDisplay dialogueDisplay;

    [Header("Time, Day/Night Cycle")]
    private GameObject time_display;
    [SerializeField]public Sprite sun_sprite;
    [SerializeField]public Sprite moon_sprite;
    [Header("Boat")]
    private Animator boat_animator;


    private void Awake() {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        setupCamera();
        updateResUI();
        Transform canvas = transform.Find("Canvas");
        Transform time_display_transform = canvas.Find("TimeDisplay");
        time_display = time_display_transform.gameObject;
        Transform boat = canvas.Find("Boat");
        boat_animator = boat.GetComponent<Animator>();
        dialogueDisplay = DialogueDisplay.instance;
    }

    void setupCamera()
    {
        Camera.main.orthographicSize = 100 * Screen.height / Screen.width * 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void sailButton()
    {
        //set animations for sailing
        //wait for some time 2 seconds or so
        setButtonsInteractable(false);
        boat_animator.Play("boat_travel");
        Invoke("sail", 5);
    }

    private void sail()
    {
        //time goes on
        GameManager.Instance.travel_distance++;
        switchTime();
        GameManager.Instance.consumeResources();
        GameManager.Instance.checkLose();
        GameManager.Instance.applyStatusEffects();
        GameManager.Instance.startRandomEvent();
    }

    private void switchTime()
    {
        GameManager.Instance.isDay = !GameManager.Instance.isDay;

        if(GameManager.Instance.isDay)
        {
            time_display.GetComponent<Image>().sprite = sun_sprite;
        }
        else
        {
            time_display.GetComponent<Image>().sprite = moon_sprite;
        }

    }

    public void setButtonsInteractable(bool interactable)
    {
        foreach (Button button in buttons)
        {
            button.interactable = interactable;
        }
    }

    public void updateResUI()
    {
        resource_display.updateUI(GameManager.Instance.Morale, GameManager.Instance.Resource);
    }
}


