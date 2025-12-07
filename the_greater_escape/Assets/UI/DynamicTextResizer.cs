using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class DynamicTextResizer : MonoBehaviour
{
    public UIDocument uiDocument;

    // Stores the initial font size (percentage) for each element
    private Dictionary<VisualElement, float> fontSizePercentages = new Dictionary<VisualElement, float>();

    private void OnEnable()
    {
        var root = uiDocument.rootVisualElement;

        // Store initial font sizes as percentages
        StoreInitialPercentages(root);

        // Resize all text immediately
        ResizeAllText(root);

        // Update on any geometry change (resize)
        root.RegisterCallback<GeometryChangedEvent>(evt => ResizeAllText(root));
    }

    private void StoreInitialPercentages(VisualElement root)
    {
        // Labels
        foreach (var label in root.Query<Label>().ToList())
        {
            StorePercentage(label, label.resolvedStyle.fontSize);
        }

        // TextFields
        foreach (var textField in root.Query<TextField>().ToList())
        {
            StorePercentage(textField, textField.resolvedStyle.fontSize);
        }

        // TextFields
        foreach (var slider in root.Query<Slider>().ToList())
        {
            StorePercentage(slider, slider.resolvedStyle.fontSize);
        }

        foreach (var sliderint in root.Query<SliderInt>().ToList())
        {
            StorePercentage(sliderint, sliderint.resolvedStyle.fontSize);
        }
        // Buttons
        foreach (var button in root.Query<Button>().ToList())
        {
            StorePercentage(button, button.resolvedStyle.fontSize);
        }
    }

    private void StorePercentage(VisualElement element, float fontSize)
    {
        if (fontSize >= 0 && fontSize <= 100)
            fontSizePercentages[element] = fontSize / 100;
        else
            fontSizePercentages[element] = 1f; // fallback to 100%
    }

    private void ResizeAllText(VisualElement root)
    {
        // Labels
        foreach (var label in root.Query<Label>().ToList())
        {
            ResizeElement(label);
        }

        // TextFields
        foreach (var textField in root.Query<TextField>().ToList())
        {
            ResizeElement(textField);
        }

        // TextFields
        foreach (var slider in root.Query<Slider>().ToList())
        {
            ResizeElement(slider);
        }
        // TextFields
        foreach (var sliderint in root.Query<SliderInt>().ToList())
        {
            ResizeElement(sliderint);
        }

        // Buttons
        foreach (var button in root.Query<Button>().ToList())
        {
            ResizeElement(button);
        }
    }

    private void ResizeElement(VisualElement element)
    {
        if (element == null) return;



        float height = element.resolvedStyle.height;
        if (!fontSizePercentages.TryGetValue(element, out float percentage))
            percentage = 1f;


        Debug.Log(element);

        if (element is TextField)
        {
            element.Q<TextElement>().style.fontSize = height * percentage;
            element = element.Q<Label>();
        }
        if (element is Slider|| element is SliderInt)
        {
            element = element.Q<Label>();
        }
        element.style.fontSize = height * percentage;
    }
}
