using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Bullets : MonoBehaviour
{

    public LayerMask enemyLayer;

    float timer = 0f;
    float lifeTime = 2.0f;

    ObjectsPoolingDefault BulletsPool;
    GamePadVibrationManager _GamePadVibInstance;

    public static Action OnHitTriggered;

    private void Start()
    {
        _GamePadVibInstance = GamePadVibrationManager.instance;

        BulletsPool = GameObject.Find("BulletsPool")?.GetComponent<ObjectsPoolingDefault>();
    }

    private void Update()
    {
        if (!gameObject.activeInHierarchy)
        {
            timer = 0;
            return;
        }

        if (timer < lifeTime)
            timer += Time.deltaTime;
        else
        {
            timer = 0;
            BulletsPool.ReturnPooledObject(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(enemyLayer == (enemyLayer | 1 << collision.gameObject.layer))
        {
            if (!collision.gameObject.GetComponent<EnemyBehavior>().forcedVisibilityToggle)
                return;

            collision.gameObject.GetComponent<EnemyBehavior>().healthPoint -= 1;
            if(_GamePadVibInstance != null)
                _GamePadVibInstance.Rumble(0.1f, 0.1f, 0.5f);
            OnHitTriggered?.Invoke();
            BulletsPool.ReturnPooledObject(gameObject);
        }
        else
        {
            BulletsPool.ReturnPooledObject(gameObject);
        }

        if (BulletsPool == null)
        {
            Destroy(gameObject);
            Debug.LogError("NO BULLETS POOL REFERENCED!!!");
            return;
        }
    }
}
