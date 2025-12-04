using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Carousel : VisualElement
{
    private List<Texture2D> _images = new();
    private int _currentIndex = 0;

    private VisualElement _imageContainer;
    private Button _nextButton;
    private Button _prevButton;
    private Image _image;

    public new class UxmlFactory : UxmlFactory<Carousel, UxmlTraits> { }

    public Carousel()
    {
        AddToClassList("carousel");

        // IMAGE CONTAINER (fills entire carousel)
        _imageContainer = new VisualElement();
        _imageContainer.AddToClassList("carousel-image-container");
        Add(_imageContainer);

        // IMAGE (fills container)
        _image = new Image();
        _image.scaleMode = ScaleMode.ScaleToFit;  // Can change to ScaleToFill
        _image.AddToClassList("carousel-image");
        _imageContainer.Add(_image);

        // LEFT BUTTON (transparent overlay)
        _prevButton = new Button(() => ShowPrevious());
        _prevButton.AddToClassList("carousel-button-left");
        _prevButton.text = "<";
        Add(_prevButton);

        // RIGHT BUTTON (transparent overlay)
        _nextButton = new Button(() => ShowNext());
        _nextButton.AddToClassList("carousel-button-right");
        _nextButton.text = ">";
        Add(_nextButton);
    }

    public void SetImages(List<Texture2D> textures)
    {
        _images = textures;
        _currentIndex = 0;
        UpdateImage();
    }

    private void ShowNext()
    {
        if (_images.Count == 0) return;
        _currentIndex = (_currentIndex + 1) % _images.Count;
        UpdateImage();
    }

    private void ShowPrevious()
    {
        if (_images.Count == 0) return;
        _currentIndex = (_currentIndex - 1 + _images.Count) % _images.Count;
        UpdateImage();
    }

    private void UpdateImage()
    {
        if (_images.Count == 0) return;
        _image.image = _images[_currentIndex];
    }
}
