using UnityEngine;

public class Projectile : MonoBehaviour
{
    float time_since_spawn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        time_since_spawn = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        time_since_spawn += Time.deltaTime;
        if (time_since_spawn > 2.0f)
            Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6 /*Enemy*/)
        {
            Destroy( collision.gameObject );
            Destroy( gameObject );
        }
    }
}
