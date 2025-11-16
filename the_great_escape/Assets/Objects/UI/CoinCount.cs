using UnityEngine;
using TMPro;

public class CoinCount : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI coinCountText;

    private Player player;

    void Start()
    {
        player = FindFirstObjectByType<Player>();
    }
    void Update()
    {
        coinCountText.text = "Coins:\n" + player.ammo.ToString("0");
    }
}
