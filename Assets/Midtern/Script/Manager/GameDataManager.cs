using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    // record option data, setting ...
    #region Singleton 
    public static GameDataManager instance;
    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of GameDataManager found!");
            Destroy(gameObject);
            return;
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
    }

    #endregion
    public int turn_type = 0;  // { Snap = 0, Continuous = 1};
    public int forward_type = 0;  // { follow camera = 0, follow palyer = 1};
    public List<float> EMS;  // { 0 = none, 1 = EMS, 2 = EMS+};
}
