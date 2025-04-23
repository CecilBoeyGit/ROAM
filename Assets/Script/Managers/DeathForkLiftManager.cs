using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathForkLiftManager : MonoBehaviour, IDataPersistence
{

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
        if (spawnLocation.Count == 0)
            return;


        for(int i = 0; i < spawnLocation.Count; i++)
        {
            Instantiate(deathActors, spawnLocation[i], Quaternion.identity);
            var dActorComp = deathActors.GetComponent<DeadForkLiftBehaviors>();
            dActorComp.visualToDisplay = i;
            dActorComp.lineToPlay = i; //Update this to split dialogues between day 1 and 2
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
}
