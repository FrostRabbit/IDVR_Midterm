using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMSControl : MonoBehaviour
{

    #region Singleton
    public static EMSControl instance;
    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of EMSControl found!");
            Destroy(gameObject);
            return;
        }
        else
        {
            // DontDestroyOnLoad(gameObject);
            instance = this;
        }
    }

    #endregion
    public ServoDeviceControlNoUI Device1;
    public ServoDeviceControlNoUI Device2;
    public Transform player;
    public float heightThreshold = 0.1f;
    public PlayerPortalController playerPortalController;
    private int player_stay;
    private int player_up_down;
    private int player_grab;
    private GameDataManager gameDataManager;
    private float previousHeight;
    private bool isRightHandStim = true;
    public float hilltime = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.instance != null)
        {
            player_stay = GameManager.instance.player_stay;
            player_up_down = GameManager.instance.player_up_down;
            player_grab = GameManager.instance.player_grab;
        }
        else
        {
            Debug.LogWarning("GameManager is null");
            player_stay = 0;
            player_up_down = 0;
            player_grab = 0;
        }

        if (GameDataManager.instance != null)
        {
            gameDataManager = GameDataManager.instance;
        }
        else
        {
            Debug.LogWarning("GameDataManager is null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        CallEMS_Grab();
    }

    public void CallEMS_Locomotion()
    {
        player_stay = GameManager.instance.player_stay;
        player_up_down = GameManager.instance.player_up_down;

        float currentHeight = transform.position.y;
        float heightChange = currentHeight - previousHeight;

        Debug.Log("player_up_down:" + player_up_down);

        if (Device1.isStim == false)
        {
            /*if (Mathf.Abs(heightChange) > heightThreshold)
            {*/
            if (player_stay == 1) // stair
            {
                if (player_up_down == 2) // up
                {
                    Device1.SetValue(1, gameDataManager.EMS[2] / 2.02f, "AMPL");
                    Device1.SetValue(1, 10, "STIM");
                    Device1.StartStim(1);
                    Device1.isStim = true;
                }
                else if (player_up_down == 1) // down
                {
                    Device1.SetValue(2, gameDataManager.EMS[3] / 2.02f, "AMPL");
                    Device1.SetValue(2, 10, "STIM");
                    Device1.StartStim(2);
                    Device1.isStim = true;
                }
            }
            /*}*/
            
            if (player_stay == 2)
            { // hill
                Debug.Log("hilltime:" + hilltime);
                if (player_up_down == 2) // up
                {
                    Debug.Log("hilltime UP:" + hilltime);
                    Device1.SetValue(1, gameDataManager.EMS[0] / 2.02f, "AMPL");
                    Device1.SetValue(1, hilltime, "STIM");
                    Device1.SetValue(2, gameDataManager.EMS[0] / 2.02f, "AMPL");
                    Device1.SetValue(2, hilltime, "STIM");
                    Device1.StartStimBoth();
                    Device1.isStim = true;
                }
                else if (player_up_down == 1) // down
                {
                    Debug.Log("hilltime DONW:" + hilltime);
                    Device1.SetValue(1, gameDataManager.EMS[1] / 2.02f, "AMPL");
                    Device1.SetValue(1, hilltime, "STIM");
                    Device1.SetValue(2, gameDataManager.EMS[1] / 2.02f, "AMPL");
                    Device1.SetValue(2, hilltime, "STIM");
                    Device1.StartStimBoth();
                    Device1.isStim = true;
                }
            }
        }
    }
    public void CallEMS_Grab()
    {
        player_grab = GameManager.instance.player_grab;
        bool rightController_grab = GameManager.instance.is_right_hand_controller_grab;
        bool lefthand_grab = GameManager.instance.is_left_hand_grab;

        float currentHeight = transform.position.y;
        float heightChange = currentHeight - previousHeight;


        if (Device1.isStim == false)
        {
            if (player_grab > 1) // grab
            {
                if (rightController_grab)
                {
                    Device1.SetValue(2, (gameDataManager.EMS[6] / 2.02f) + 4.0f - player_grab, "AMPL");
                    Device1.SetValue(2, 10, "STIM");
                    Device1.StartStim(2);
                    Device1.isStim = true;
                }
                else if (lefthand_grab)
                {
                    Device1.SetValue(1, (gameDataManager.EMS[6] / 2.02f) + 4.0f - player_grab, "AMPL");
                    Device1.SetValue(1, 10, "STIM");
                    Device1.StartStim(1);
                    Device1.isStim = true;
                }

            }
            else if (player_grab == 1) // climbing
            {
                if (rightController_grab && !lefthand_grab)
                {
                    Device1.SetValue(2, (gameDataManager.EMS[5] / 2.02f), "AMPL");
                    Device1.SetValue(2, 2, "STIM");
                    Device1.StartStim(2);
                    Device1.isStim = true;
                    isRightHandStim = true; // 設置當前電擊的手為右手
                }
                else if (lefthand_grab && !rightController_grab)
                {
                    Device1.SetValue(1, (gameDataManager.EMS[5] / 2.02f), "AMPL");
                    Device1.SetValue(1, 2, "STIM");
                    Device1.StartStim(1);
                    Device1.isStim = true;
                    isRightHandStim = false; // 設置當前電擊的手為左手
                }
                else if (rightController_grab && lefthand_grab)
                {
                    // 同時抓住時，切換電擊的手
                    if (isRightHandStim)
                    {
                        Device1.SetValue(2, (gameDataManager.EMS[5] / 2.02f), "AMPL");
                        Device1.SetValue(2, 2, "STIM");
                        Device1.StartStim(2);
                        Device1.isStim = true;
                        isRightHandStim = false; // 切換到左手
                    }
                    else
                    {
                        Device1.SetValue(1, (gameDataManager.EMS[5] / 2.02f), "AMPL");
                        Device1.SetValue(1, 2, "STIM");
                        Device1.StartStim(1);
                        Device1.isStim = true;
                        isRightHandStim = true; // 切換到右手
                    }
                }
            }


            if (playerPortalController.isTouching) // portal
            {
                Device1.SetValue(1, gameDataManager.EMS[4] / 2.02f, "AMPL");
                Device1.SetValue(1, 5, "STIM");
                Device1.StartStim(1);
                Device1.isStim = true;
            }
        }
    }

        // if (Device2.isStim == false)
        // {
        //     if (player_grab != 0)
        //     {
        //         Device2.SetValue(1, (gameDataManager.EMS[6] / 2.02f) + 4.0f - player_grab, "AMPL");
        //         Device2.SetValue(1, 10, "STIM");
        //         Device2.isStim = true;
        //         Device2.StartStim(1);
        //         return;
        //     }

        //     if (player_stay == 2)
        //     {
        //         if (player_up_down == 1) // up
        //         {
        //             Device2.SetValue(1, gameDataManager.EMS[6] / 2.02f, "AMPL");
        //             Device2.SetValue(1, 10, "STIM");
        //             Device2.isStim = true;
        //             Device2.StartStim(1);
        //         }
        //         else if (player_up_down == 2) // down
        //         {
        //             Device2.SetValue(2, gameDataManager.EMS[6] / 2.02f, "AMPL");
        //             Device2.SetValue(2, 10, "STIM");
        //             Device2.isStim = true;
        //             Device2.StartStim(2);
        //         }
        //     }
        // }

    
}
