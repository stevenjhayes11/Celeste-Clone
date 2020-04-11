using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    static int level;
    // Start is called before the first frame update
    void Start()
    {
        level = 0;
        MakeSingleton();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void MakeSingleton()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    public int GetLevel()
    {
        return level++;
    }
}
