using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesDestroyOnDeath : MonoBehaviour
{

    private ParticleSystem particlesSelf;
    void Start()
    {
        particlesSelf = GetComponent<ParticleSystem>();
    }
    void Update()
    {
        if (particlesSelf.isStopped)
        {
            Destroy(gameObject);
        }
    }
}
