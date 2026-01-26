using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

public class Ingame : MonoBehaviour
{
    public UIDocument document;

    private VisualElement root;
    private Label stickers;
    private Label walls;


    public CooldownIcon destroy;


    public Player player;

    private void Awake()
    {
        root = document.rootVisualElement;
        stickers = root.Q<Label>("stickers");
        walls = root.Q<Label>("walls");
        destroy = root.Q<CooldownIcon>("destroy");

    }

    public void Update()
    {
        stickers.text = "" + player.graffiti_count;
        walls.text = "" + player.walls;

        //destroy.CooldownPercent -= 0.001f;
    }


    public void Show()
    {
        root.style.display = DisplayStyle.Flex;
    }
}
