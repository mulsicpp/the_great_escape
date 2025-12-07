using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using System;

public class MainMenu : MonoBehaviour, IUIScreen
{
    [SerializeField] private UIDocument menuDocument;
    [SerializeField] private GameObject tutorialDocument;
    [SerializeField] private MazeManager mazePrefab;


    private VisualElement root;

    private Button tutorial;
    private Button play;
    private Slider volume;
    private SliderInt size;
    private TextField seed;

    private float selectedVolume;
    private int selectedSize;
    private string selectedSeed;

    public AudioSource backgroundMusic;
    public AudioSource player;


    private void Awake()
    {
        root = menuDocument.rootVisualElement;

    }

    private void OnEnable()
    {
        tutorial = root.Q<Button>("Tutorial");
        play = root.Q<Button>("Play");
        volume = root.Q<Slider>("Volume");
        size = root.Q<SliderInt>("Size");
        seed = root.Q<TextField>("Seed");

        seed.RegisterValueChangedCallback(OnSeedChanged);
        play.clicked += OnPlayPressed;
        tutorial.clicked += OnTutorialPressed;

        volume.value = backgroundMusic.volume * 100;
        volume.RegisterValueChangedCallback(evt =>
        {
            backgroundMusic.volume = evt.newValue / 100;
            player.volume = evt.newValue / 100;
        });
    }



    private void OnDisable()
    {
        seed.UnregisterValueChangedCallback(OnSeedChanged);
        play.clicked -= OnPlayPressed;
        tutorial.clicked -= OnTutorialPressed;
    }

    private void OnSeedChanged(ChangeEvent<string> evt)
    {
        if (string.IsNullOrEmpty(evt.newValue))
            return;

        if (!int.TryParse(evt.newValue, out _))
            seed.SetValueWithoutNotify(evt.previousValue);
    }

    private void OnPlayPressed()
    {
        selectedVolume = volume.value;
        selectedSize = size.value;
        selectedSeed = seed.value;



        mazePrefab.seed = (selectedSeed != "" ? Int32.Parse(selectedSeed) : new System.Random().Next());
        mazePrefab.dim = selectedSize;
        mazePrefab.gameObject.SetActive(true);

        Debug.Log("Play pressed!");
        Debug.Log("Volume: " + selectedVolume);
        Debug.Log("Size: " + selectedSize);
        Debug.Log("Seed: " + selectedSeed);

        // Hide menu
        root.style.display = DisplayStyle.None;
    }

    private void OnTutorialPressed()
    {
        var tut = tutorialDocument.GetComponent<Tutorial>();
        tut.Show(this.gameObject);

        // Hide
        root.style.display = DisplayStyle.None;
    }

    public void Show()
    {
        volume.value = backgroundMusic.volume * 100;
        root.style.display = DisplayStyle.Flex;
    }
}
