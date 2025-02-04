using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullets : MonoBehaviour
{

    public LayerMask enemyLayer;

    float timer = 0f;
    float lifeTime = 3.0f;

    private void Update()
    {
        if (timer < lifeTime)
            timer += Time.deltaTime;
        else
            Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(enemyLayer == (enemyLayer | 1 << collision.gameObject.layer))
        {
            collision.gameObject.GetComponent<EnemyBehavior>().healthPoint -= 1;
        }

        Destroy(gameObject);
    }
}
