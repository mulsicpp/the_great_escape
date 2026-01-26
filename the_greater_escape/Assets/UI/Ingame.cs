using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

public class Ingame : MonoBehaviour
{
    public UIDocument document;

    private VisualElement root;
    private Label stickers;

    private CooldownIcon destroy;
    private CooldownIcon build;


    public Player player;

    private void Awake()
    {
        root = document.rootVisualElement;
        stickers = root.Q<Label>("stickers");
        destroy = root.Q<CooldownIcon>("destroy");
        build = root.Q<CooldownIcon>("build");

    }

    public void Update()
    {
        stickers.text = "" + player.graffiti_count;
        destroy.CooldownPercent -= 0.001f;
        build.CooldownPercent -= 0.001f;
    }


    public void Show()
    {
        root.style.display = DisplayStyle.Flex;
    }
}
