using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

public class Pause : MonoBehaviour, IUIScreen
{
    public UIDocument document;
    public GameObject mainmenuobject;
    public GameObject tutorialDocument;

    private VisualElement root;
    private Button resume;
    private Button mainmenu;
    private Button tutorial;
    private Button restart;

    public MazeManager mazeManager;

    private void Awake()
    {
        root = document.rootVisualElement;
        root.style.display = DisplayStyle.None;
    }

    private void OnEnable()
    {
        resume = root.Q<Button>("Play");
        mainmenu = root.Q<Button>("main-menu");
        tutorial = root.Q<Button>("Tutorial");
        restart = root.Q<Button>("restart");

        resume.clicked += OnResumePressed;
        mainmenu.clicked += OnMainMenuPressed;
        tutorial.clicked += OnTutorialPressed;
        restart.clicked += OnRestartPressed;
    }

    private void OnDisable()
    {
        resume.clicked -= OnResumePressed;
        mainmenu.clicked -= OnMainMenuPressed;
        tutorial.clicked -= OnTutorialPressed;
        restart.clicked -= OnRestartPressed;
    }

    private void OnResumePressed()
    {
        root.style.display = DisplayStyle.None;
        Time.timeScale = 1;
    }

    private void OnMainMenuPressed()
    {
        mainmenuobject.GetComponent<MainMenu>().Show();
        root.style.display = DisplayStyle.None;
        Time.timeScale = 1;

        mazeManager.gameObject.SetActive(false);
    }

    private void OnTutorialPressed()
    {
        var tut = tutorialDocument.GetComponent<Tutorial>();
        tut.Show(this.gameObject);
        root.style.display = DisplayStyle.None;
    }

    private void OnRestartPressed()
    {
        // Hide pause UI
        root.style.display = DisplayStyle.None;
        Time.timeScale = 1;

        mazeManager.gameObject.SetActive(false);

        mazeManager.seed = new System.Random().Next();

        mazeManager.gameObject.SetActive(true);
    }

    public void Show()
    {
        Time.timeScale = 0;
        root.style.display = DisplayStyle.Flex;
    }
}
