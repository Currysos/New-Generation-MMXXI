using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivesUI : MonoBehaviour
{
    public List<GameObject> livesObjects = new List<GameObject>();
    public GameObject livesObject;
    public void SpawnLives(int numberOfLives)
    {
        for (var i = 0; i < 3; i++)
        {
            livesObjects.Add(Instantiate(livesObject, transform));
        }
    }

    // Return true if life was added successfully. 
    // Return false if number of lives already was 3
    public bool AddLife()
    {
        if (livesObjects.Count >= 3) return false;
        livesObjects.Add(Instantiate(livesObject, transform));
        return true;
    }

    // Return true if player is still alive
    // Return false if player died
    public bool RemoveLife()
    {
        Destroy(livesObjects[0]);
        livesObjects.RemoveAt(0);
        return livesObjects.Count > 0;
    }
}
