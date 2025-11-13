using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemy_prefab;
    public float spawn_duration;

    public float min_radius;
    public float max_radius;
    public float bias_front;

    public List<Enemy> enemies;
    private float time_since_spawn;
    private Player player;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemies = new List<Enemy>();
        time_since_spawn = 0.0f;
        player = FindFirstObjectByType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        time_since_spawn += Time.deltaTime;

        if(time_since_spawn > spawn_duration)
        {
            time_since_spawn -= spawn_duration;
            spawn_enemy();
        }
    }

    private void spawn_enemy()
    {
        var random_val = Random.value;

        var velocity = player.rb.linearVelocity;

        var vel_mag = velocity.magnitude;
        var vel_dir = velocity.normalized;

        var angle = Mathf.Pow(random_val, 1 + vel_mag * 0.2f) * Mathf.PI * (Random.value > 0.5f ? -1.0f : 1.0f) + Vector2.SignedAngle(Vector2.right, vel_dir) * Mathf.PI / 180.0f;

        var relative_pos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        relative_pos *= Random.Range(min_radius, max_radius);

        var spawn_offset = vel_mag / (vel_mag + 1.0f) * vel_dir;

        var spawn_pos =  player.transform.position + new Vector3(relative_pos.x + spawn_offset.x, relative_pos.y + spawn_offset.y, 0.0f);


        var enemy = Instantiate(enemy_prefab, spawn_pos, Quaternion.identity);
        enemy.GetComponent<Enemy>().manager = this;
        enemies.Add(enemy.GetComponent<Enemy>());
    }
}
