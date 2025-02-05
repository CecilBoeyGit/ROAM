using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIGroup : MonoBehaviour
{

    [SerializeField] float InterpolateTime = 10.0f;
    [SerializeField] float offsetY;
    [SerializeField] float offsetX;[SerializeField] float offsetZ;
    Vector3 initialOffset;

    PlayerController playerInstance;

    Vector3 dampRef;

    private void Start()
    {
        if(playerInstance == null)
            playerInstance = PlayerController.instance;

        initialOffset = playerInstance.transform.position - transform.position;
    }
    void Update()
    {
        Vector3 targetPosition = playerInstance.transform.position - initialOffset + new Vector3(offsetX, offsetY, offsetZ);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref dampRef, Time.deltaTime, InterpolateTime);
    }
}
