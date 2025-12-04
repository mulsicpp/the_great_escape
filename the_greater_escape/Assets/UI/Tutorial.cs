using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using System.Collections.Generic;

public class Tutorial : MonoBehaviour
{
    public UIDocument document;

    private VisualElement root;
    private Button back;
    public GameObject backUi;

    [Header("Carousel Settings")]
    public string carouselName = "TutorialCarousel";
    public List<Texture2D> images;

    private Carousel carousel;

    private void Awake()
    {
        root = document.rootVisualElement;
        root.style.display = DisplayStyle.None;
    }

    private void OnEnable()
    {
        back = root.Q<Button>("Back");
        carousel = root.Q<Carousel>(carouselName);

        back.clicked += OnBackPressed;

        if (carousel != null)
            carousel.SetImages(images);
    }

    private void OnDisable()
    {
        back.clicked -= OnBackPressed;
    }

    private void OnBackPressed()
    {
        if (backUi != null)
        {
            var ui = backUi.GetComponent<IUIScreen>();
            ui?.Show();
        }

        root.style.display = DisplayStyle.None;
    }

    public void Show(GameObject returnUi)
    {
        backUi = returnUi;
        root.style.display = DisplayStyle.Flex;
    }
}


public interface IUIScreen
{
    void Show();
}