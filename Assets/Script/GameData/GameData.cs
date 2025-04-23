using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int lineIndex;
    public List<Vector3> deathPosition;

    public GameData()
    {
        this.lineIndex = 0;
        this.deathPosition = new List<Vector3>();
    }

    public void AddLocation(Vector3 location)
    {
        //If the list already contains the same position, do NOT add to prevent duplicate spawning
        if (this.deathPosition.Contains(location))
            return;

        if (this.deathPosition.Count >= 3)
        {
            this.deathPosition.RemoveAt(0);
            this.deathPosition.RemoveAt(0);
        }

        this.deathPosition.Add(location);
    }

    public void ClearLocation()
    {
        if (this.deathPosition.Count == 0)
            return;

        this.deathPosition.Clear();
    }
}
