using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyManager manager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        manager.enemies.Remove(this);
    }
}
