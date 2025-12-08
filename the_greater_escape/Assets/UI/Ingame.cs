using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

public class Ingame : MonoBehaviour
{
    public UIDocument document;

    private VisualElement root;
    private Label stickers;

    public Player player;

    private void Awake()
    {
        root = document.rootVisualElement;
        stickers = root.Q<Label>("stickers");
    }

    public void Update()
    {
        stickers.text = "" + player.graffiti_count;
    }


    public void Show()
    {
        root.style.display = DisplayStyle.Flex;
    }
}
