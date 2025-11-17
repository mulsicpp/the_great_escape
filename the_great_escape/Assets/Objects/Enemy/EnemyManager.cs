using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemy_prefab; 
    public float spawn_duration;
    private float current_spawn_duration;

    public float min_radius;
    public float max_radius;
    public float bias_front;

    public GameObject bottomLeftCorner;
    public GameObject topRightCorner;

    private Vector3 minPos;
    private Vector3 maxPos;

    public List<Enemy> enemies;
    private uint enemies_spawned;
    private float time_since_spawn;
    private Player player;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemies = new List<Enemy>();
        time_since_spawn = 0.0f;
        player = FindFirstObjectByType<Player>();
        minPos = bottomLeftCorner.transform.position;
        maxPos = topRightCorner.transform.position;
        enemies_spawned = 0;

        current_spawn_duration = calc_spawn_duration();
    }

    // Update is called once per frame
    void Update()
    {
        time_since_spawn += Time.deltaTime;

        if(time_since_spawn > current_spawn_duration)
        {
            time_since_spawn = 0;
            spawn_enemy();
            enemies_spawned++;
            current_spawn_duration = calc_spawn_duration();
        }
    }

    private float calc_spawn_duration()
    {
        return spawn_duration / Mathf.Sqrt(enemies_spawned + 1);
    }

    private bool is_position_valid(Vector3 pos)
    {
        if (pos.x < minPos.x || pos.x > maxPos.x || pos.y < minPos.y || pos.y > maxPos.y)
            return false;
        return true;
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

        if (is_position_valid(spawn_pos))
        {
            var enemy = Instantiate(enemy_prefab, spawn_pos, Quaternion.identity);
            enemy.GetComponent<Enemy>().manager = this;
            enemies.Add(enemy.GetComponent<Enemy>());
        }
        
    }
}
