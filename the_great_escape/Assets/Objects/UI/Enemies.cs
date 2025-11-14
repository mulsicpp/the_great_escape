using UnityEngine;
using TMPro;

public class Enemies : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI enemiesText;

    private EnemyManager enemyManager;

    void Start()
    {
        enemyManager = FindFirstObjectByType<EnemyManager>();
    }
    void Update()
    {
        string text = "Enemies:\n" + enemyManager.enemies.Count.ToString("0");
        enemiesText.text = text;
    }
}
