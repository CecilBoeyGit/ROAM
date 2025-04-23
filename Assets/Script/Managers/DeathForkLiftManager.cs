using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathForkLiftManager : MonoBehaviour, IDataPersistence
{

    public bool Day01 = false, Day02 = false;
    [SerializeField] int cycle01ElementSize = 9, cycle02ElementSize = 11;
    int cycleElementIndex = 0;

    [SerializeField] GameObject deathActors;
    private List<Vector3> spawnLocation;
    private int lineIndex;

    public static DeathForkLiftManager instance;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("DeathForkLiftManager already exists!");
        }

        instance = this;
    }

    public void LoadData(GameData data)
    {
        //Spawn the dead fork lift actor to previous death position if present
        spawnLocation = data.deathPosition;
        lineIndex = data.lineIndex;
    }
    public void SaveData(GameData data)
    {
        data.lineIndex = this.lineIndex;
    }

    private void OnEnable()
    {
       
    }
    // Start is called before the first frame update
    void Start()
    {
        if (spawnLocation.Count < 1)
            return;

        print("SpawnDataCount: " + spawnLocation.Count);

        for (int i = 0; i < spawnLocation.Count; i++)
        {
            GameObject dActor = Instantiate(deathActors, spawnLocation[i], Quaternion.identity);
            var dActorComp = dActor.GetComponent<DeadForkLiftBehaviors>();
            if (Day01)
            {
                cycleElementIndex = Random.Range(0, cycle01ElementSize - spawnLocation.Count);
                dActorComp.SetActiveState(i, cycleElementIndex + i, 0, cycle01ElementSize);
            }
            else if (Day02)
            { 
                cycleElementIndex = Random.Range(cycle01ElementSize, cycle01ElementSize + cycle02ElementSize - spawnLocation.Count);
                dActorComp.SetActiveState(i, cycleElementIndex + i, cycle01ElementSize, cycle01ElementSize + cycle02ElementSize);
            }
            //print("Actor display index: " + i + " List index: " + cycleElementIndex);
        }
    }
}
