using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    public GameObject PlayerPortal;
    public GameObject[] portals;
    public Transform Player;
    public int portalIndex = 0;
    public Animator portalOpenAnim;
    public OVRHand lefthands;
    public float Threshold = 0.5f;
    private bool isPinching = false;

    void Start()
    {
        PlayerPortal.SetActive(false);
        closeallportals();
    }

    void Update()
    {
        HandleKeyboardInput();

        // 檢查手是否離玩家超過一定距離
        if (IsHandTooClose())
        {
            return;
        }
        
        HandleHandGestures();
    }

    private void HandleKeyboardInput()
    {
        for (int i = 0; i < portals.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Keypad0 + i))
            {
                portalIndex = i;
                closeallportals();
                respawnPlayerPortal();
                portals[i].SetActive(true);
            }
        }
    }

    private bool IsHandTooClose()
    {
        return Vector3.Distance(Player.position, lefthands.transform.position) < Threshold;
    }

    private void HandleHandGestures()
    {
        if (PinchIndex(lefthands) && !isPinching)
        {
            isPinching = true;
            portalIndex = (portalIndex + 1) % portals.Length;
            Debug.Log("portalIndex: " + portalIndex);
            closeallportals();
            respawnPlayerPortal();
            portals[portalIndex].SetActive(true);
        }

        if (PinchMiddle(lefthands) && !isPinching)
        {
            isPinching = true;
            if (portalIndex == 0)
            {
                portalIndex = portals.Length;
            }
            portalIndex = (portalIndex - 1) % portals.Length;
            Debug.Log("portalIndex: " + portalIndex);
            closeallportals();
            respawnPlayerPortal();
            portals[portalIndex].SetActive(true);
        }

        if (PinchRing(lefthands) && !isPinching)
        {
            isPinching = true;
            closeallportals();
        }

        if (!PinchIndex(lefthands) && !PinchMiddle(lefthands) && !PinchRing(lefthands))
        {
            isPinching = false;
        }
    }

    private bool PinchIndex(OVRHand hand)
    {
        return 
            hand.GetFingerIsPinching(OVRHand.HandFinger.Thumb) &&
            hand.GetFingerIsPinching(OVRHand.HandFinger.Index) &&
            !hand.GetFingerIsPinching(OVRHand.HandFinger.Middle) &&
            !hand.GetFingerIsPinching(OVRHand.HandFinger.Ring) &&
            !hand.GetFingerIsPinching(OVRHand.HandFinger.Pinky);
    }

    private bool PinchMiddle(OVRHand hand)
    {
        return 
            hand.GetFingerIsPinching(OVRHand.HandFinger.Thumb) &&
            !hand.GetFingerIsPinching(OVRHand.HandFinger.Index) &&
            hand.GetFingerIsPinching(OVRHand.HandFinger.Middle) &&
            !hand.GetFingerIsPinching(OVRHand.HandFinger.Ring) &&
            !hand.GetFingerIsPinching(OVRHand.HandFinger.Pinky);
    }

    private bool PinchRing(OVRHand hand)
    {
        return 
            hand.GetFingerIsPinching(OVRHand.HandFinger.Thumb) &&
            !hand.GetFingerIsPinching(OVRHand.HandFinger.Index) &&
            !hand.GetFingerIsPinching(OVRHand.HandFinger.Middle) &&
            hand.GetFingerIsPinching(OVRHand.HandFinger.Ring) &&
            !hand.GetFingerIsPinching(OVRHand.HandFinger.Pinky);
    }

    void closeallportals()
    {
        foreach (GameObject portal in portals)
        {
            portal.SetActive(false);
        }
        PlayerPortal.SetActive(false);
    }

    void respawnPlayerPortal()
    {
        PlayerPortal.SetActive(true);
        portalOpenAnim.SetTrigger("Open");
        // 門生成在頭盔面前
        PlayerPortal.transform.position = Player.position + new Vector3(0, 0.5f, 0) + Player.forward * 0.8f;
        PlayerPortal.transform.rotation = Player.rotation;
    }
}