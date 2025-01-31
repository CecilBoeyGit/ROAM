using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GeneratorManager : MonoBehaviour
{
    [SerializeField] Transform generatorsGroup;
    [SerializeField] List<Generators> GenList = new List<Generators>();

    public static event Action<bool> GenAnyEmpty;
    public static event Action<bool> GenAllEmpty;

    public static GeneratorManager GenManagerInstance;

    private void OnEnable()
    {
        if (GenManagerInstance == null)
            GenManagerInstance = this;
        else
            Destroy(gameObject);
    }
    void Start()
    {
        generatorsGroup = gameObject.transform;
        foreach (Transform child in generatorsGroup)
        {
            if(child.GetComponent<Generators>() != null)
                GenList.Add(child.GetComponent<Generators>());
        }
    }

    void Update()
    {
        GeneratorsEmptyConditions();
    }
    void GeneratorsEmptyConditions()
    {
        bool AnyEmpty = GenList.Any(value => value.GeneratorPowerAmount <= 0);
        bool AllEmpty = GenList.All(value => value.GeneratorPowerAmount <= 0);
        if (AllEmpty)
        {
            GenAllEmpty?.Invoke(true);
            GenAnyEmpty?.Invoke(true);
        }
        else
        {
            GenAllEmpty?.Invoke(false);
            GenAnyEmpty?.Invoke(AnyEmpty);
        }
    }
}
