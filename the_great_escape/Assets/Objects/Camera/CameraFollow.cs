using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var delta_x = target.transform.position.x - transform.position.x;
        var delta_y = target.transform.position.y - transform.position.y;

        var delta = new Vector3(delta_x, delta_y, 0.0f);

        transform.position = transform.position + delta * Time.fixedDeltaTime * 3.0f;
    }
}
