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

    private void Awake()
    {
        root = document.rootVisualElement;
        root.style.display = DisplayStyle.None;
    }

    private void OnEnable()
    {
        mainmenu = root.Q<Button>("main-menu");
        restart = root.Q<Button>("restart");

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
    }


}
