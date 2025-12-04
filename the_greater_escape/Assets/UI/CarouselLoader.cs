using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CarouselLoader : MonoBehaviour
{
    [Header("UI Document")]
    public UIDocument document;

    [Header("Carousel Settings")]
    public string carouselName = "TutorialCarousel";  // name in UXML
    public List<Texture2D> images;                    // assign in Inspector

    private Carousel carousel;

    private void Start()
    {
        var root = document.rootVisualElement;

        // Find the carousel in the UIDocument
        carousel = root.Q<Carousel>(carouselName);

        if (carousel == null)
        {
            Debug.LogError("Carousel not found in UI Document: " + carouselName);
            return;
        }

        // Pass the images into the carousel
        carousel.SetImages(images);
        //document.root.style.display = DisplayStyle.None;
    }
}
