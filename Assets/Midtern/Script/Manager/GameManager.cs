using Meta.XR.MRUtilityKit.SceneDecorator;
using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton 
    public static GameManager instance;
    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of GameManager found!");
            // Destroy(gameObject);
            return;
        }
        else
        {
            // DontDestroyOnLoad(gameObject);
            instance = this;
        }
    }

    #endregion
    public BestHoverInteractorGroup right_hand;
    public BestHoverInteractorGroup left_hand;
    public BestHoverInteractorGroup right_hand_controller;
    public BestHoverInteractorGroup left_hand_controller;
    private bool _is_right_hand_grab;
    private bool _is_left_hand_grab;
    private bool _is_left_hand_controller_grab;
    private bool _is_right_hand_controller_grab;
    // A read-write instance property:
    public bool is_right_hand_grab
    {
        get => _is_right_hand_grab;
        set => _is_right_hand_grab = value;
    }
    public bool is_left_hand_grab
    {
        get => _is_left_hand_grab;
        set => _is_left_hand_grab = value;
    }
    
    public bool is_right_hand_controller_grab
    {
        get => _is_right_hand_controller_grab;
        set => _is_right_hand_controller_grab = value;
    }
    public bool is_left_hand_controller_grab
    {
        get => _is_left_hand_controller_grab;
        set => _is_left_hand_controller_grab = value;
    }

    public LayerMask stairMask;
    public LayerMask hillMask;
    public LayerMask playerMask;
    public LayerMask itemMask;
    public int player_stay = 0; // 0 = else, 1 = stair, 2 = hill  // judge in UnityChan......cs
    public int player_up_down = 0; // 0 = normal, 1 = up, 2 = down  // judge in forward, backward
    public int player_grab = 0; // 0 = no grab, 1 = is climbing, 2 = grab small stone, 3 = grab median stone, 4 = grab large stone
    public int player_attacked = 0; // 0 = no attacked, 1 = attacked

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DetermineHandState()
    {
        _is_right_hand_grab = DetectHandGrab(right_hand);
        _is_left_hand_grab = DetectHandGrab(left_hand);
        _is_left_hand_controller_grab = DetectHandGrab(left_hand_controller);
        _is_right_hand_controller_grab = DetectHandGrab(right_hand_controller);
    }
    private bool DetectHandGrab(BestHoverInteractorGroup hand)
    {
        foreach (var item in hand.Interactors)
        {
            if (item.State == InteractorState.Select)
            {
                return true;
            }
        }
        return false;
    }
}
