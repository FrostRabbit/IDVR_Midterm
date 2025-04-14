# VR Teleportation & EMS Integration Project
This Unity VR project features two intuitive teleportation mechanisms and integrates EMS (Electrical Muscle Stimulation) for enhanced physical feedback. The system allows players to move, interact, and feel different environments through muscle stimulation.
## 1. Portal-Based Teleportation
- Use **hand gestures (finger pinching)** to cycle through preset portals.
- Touching a portal triggers a **screen fade-out/fade-in effect** and moves the player.
- Each pinch selects a different portal; portals are spawned in front of the player.

```csharp
if (PinchIndex(lefthands) && !isPinching)
{
    isPinching = true;
    portalIndex = (portalIndex + 1) % portals.Length;
    Debug.Log("portalIndex: " + portalIndex);
    closeallportals();
    respawnPlayerPortal();
    portals[portalIndex].SetActive(true);
}

void respawnPlayerPortal()
{
    PlayerPortal.SetActive(true);
    portalOpenAnim.SetTrigger("Open");
    // 門生成在頭盔面前
    PlayerPortal.transform.position = Player.position + new Vector3(0, 0.5f, 0) + Player.forward * 0.8f;
    PlayerPortal.transform.rotation = Player.rotation;
}
```

## 2. Pointable Teleportation
- Use the right controller to aim at a location.
- Rotate your wrist to set the post-teleport orientation.
- Reticle arrows indicate facing direction adjustment.

```csharp
protected override void Align(ReticleDataTeleport data)
{
    ...
    Vector3 position = data.ProcessHitPoint(_interactor.ArcEnd.Point);
    Quaternion rotation = Quaternion.LookRotation(_interactor.ArcEnd.Normal);
    // 鎖定 X 軸的旋轉
    Vector3 eulerRotation = rotation.eulerAngles;
    eulerRotation.x = -_controller.rotation.z; // 鎖定 X 軸旋轉
    eulerRotation.y = -OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch).eulerAngles.z;
    rotation = Player.rotation * Quaternion.Euler(eulerRotation);
    this.transform.SetPositionAndRotation(position, rotation);
    // 根據玩家看向終點的角度旋轉
    PlayerRotate = rotation;
    ...
}

```

# EMS
Movement Feedback

    Uphill/Downhill: Leg muscles stimulated when climbing or descending.

    Portal Touch: Top hand muscles stimulated when touching a portal.

Interaction Feedback

    Grab & Throw: Stimulates hand muscles when grabbing stones or objects.

    Climbing: Simulates grip through alternating hand stimulation while climbing.
    
```csharp
public void CallEMS_Locomotion()
{
    player_stay = GameManager.instance.player_stay;
    player_up_down = GameManager.instance.player_up_down;

    float currentHeight = transform.position.y;
    float heightChange = currentHeight - previousHeight;

    Debug.Log("player_up_down:" + player_up_down);

    if (Device1.isStim == false)
    {
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
```
# EMS Stimulate Amplitude Setup
- In the initial setup scene, the player configures stimulation intensity (amplitude) for various body parts.

- Menu interface allows testing and saving settings.

- Settings are carried over to the main gameplay scene.

# EMS Tool Used
This project uses the open-source EMS tool from Servo EMS for Unity by [BrianGodd](https://github.com/BrianGodd/Servo-EMS-for-Unity).

# Demo Video
[Click to watch](https://youtu.be/-D1EIMHxVFY)
