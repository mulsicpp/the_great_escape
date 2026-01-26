using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

public class Finish : MonoBehaviour
{
    public UIDocument document;
    public GameObject mainmenuobject;

    private VisualElement root;
    private Button mainmenu;
    private Button restart;

    public Label title;

    public MazeManager mazeManager;

    private void Awake()
    {
        root = document.rootVisualElement;
        root.style.display = DisplayStyle.None;
    }

    private void OnEnable()
    {
        mainmenu = root.Q<Button>("main-menu");
        restart = root.Q<Button>("restart");
        title = root.Q<Label>("Title");


        mainmenu.clicked += OnMainMenuPressed;
        restart.clicked += OnRestartPressed;
    }

    private void OnDisable()
    {
        mainmenu.clicked -= OnMainMenuPressed;
        restart.clicked -= OnRestartPressed;
    }

    private void OnMainMenuPressed()
    {
        mainmenuobject.GetComponent<MainMenu>().Show();
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

    public void Show(bool win)
    {
        mazeManager.gameObject.SetActive(false);
        if (win) { title.text = "YOU WON"; } else title.text = "YOU LOST";
        root.style.display = DisplayStyle.Flex;
    }
}
