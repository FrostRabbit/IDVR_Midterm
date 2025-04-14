using Oculus.Interaction;
using UnityEngine;
using static OVRPlugin;


// unuse
[System.Serializable]
public class MapTransform
{
    public Transform vrTarget_hand;
    public Transform vrTarget_controller;
    private Transform vrTarget;
    public Transform IKTarget;
    public Vector3 trackingPositionOffset;
    public Vector3 trackingRotationOffset;
    public void MapVRAvatar()
    {
        IKTarget.position = vrTarget.TransformPoint(trackingPositionOffset);
        IKTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
    }
    public void MapVRAvatarController()
    {
        IKTarget.position = vrTarget.position+ trackingPositionOffset;
        IKTarget.rotation = vrTarget.rotation * Quaternion.Euler(new Vector3(0,90,0));
    }
    public void MapVRAvatar2()
    {
        IKTarget.position = vrTarget.position + trackingPositionOffset;
        IKTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
    }
    public void UpdateBodyPosition(Transform body, Vector3 offset, float distance)
    {
        float camera_y = vrTarget.rotation.eulerAngles.y;
        // Debug.Log("@@@@@@@@@@@@@@@@@@@@@camera_y:" + camera_y);
        Vector3 CameraOffset = new Vector3(Mathf.Sin(camera_y * Mathf.Deg2Rad), 0, Mathf.Cos(camera_y * Mathf.Deg2Rad)) * distance;

        Vector3 vpos = vrTarget.position + trackingPositionOffset - CameraOffset;
        body.position = vpos + offset;
        //body.position = vpos;
        IKTarget.position = vpos;
        IKTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
    }

    public void setVrTarget(bool state)
    {
        if (state) vrTarget = vrTarget_hand;
        else vrTarget = vrTarget_controller;
    }

    public void setObjectActive(bool state) // for hand grab, show fingers IK
    {
        vrTarget_controller.gameObject.SetActive(state); // vrTarget_controller == IK parent
    }
}
public class AvatarController : MonoBehaviour
{
    [SerializeField] private ControllerActiveState left_hand_controller;
    [SerializeField] private HandActiveState left_hand;
    [SerializeField] private ControllerActiveState right_hand_controller;
    [SerializeField] private HandActiveState right_hand;

    [SerializeField] private MapTransform head;
    [SerializeField] private MapTransform leftHand;
    [SerializeField] private MapTransform rightHand;

    /*
    [SerializeField] private MapTransform rightHandThumb;
    [SerializeField] private MapTransform rightHandIndex;
    [SerializeField] private MapTransform rightHandMiddle;
    [SerializeField] private MapTransform rightHandRing;
    [SerializeField] private MapTransform rightHandPinky;
    [SerializeField] private MapTransform leftHandThumb;
    [SerializeField] private MapTransform leftHandIndex;
    [SerializeField] private MapTransform leftHandMiddle;
    [SerializeField] private MapTransform leftHandRing;
    [SerializeField] private MapTransform leftHandPinky;*/

    [SerializeField] private float turnSmoothness;

    [SerializeField] private Transform IKHead;

    [SerializeField] private Vector3 headBodyOffset;
    [SerializeField] private float CameraDistance = 1;
    private bool state = false; // false = use controller, ture: use hand
    private void Start()
    {

    }
    void LateUpdate()
    {
        if (left_hand_controller.Active || right_hand_controller.Active) state = false;
        if (right_hand.Active || left_hand.Active) state = true;

        head.setVrTarget(state);
        head.UpdateBodyPosition(transform, headBodyOffset, CameraDistance);
        transform.position = IKHead.position + headBodyOffset;

        // 手暫時先關閉 IKRightArm...，因為有些難點: 手掌無法往前、還有位置的 bug (動值後 ctrl+z 就會恢復正常，很怪)
        //SetHandPosAndRot(leftHand);
        //SetHandPosAndRot(rightHand);
        transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
        transform.forward = Vector3.Lerp(transform.forward, Vector3.ProjectOnPlane(IKHead.forward, Vector3.up).normalized, Time.deltaTime * turnSmoothness);
    }

    private void SetHandPosAndRot(MapTransform target)
    {
        target.setVrTarget(state);
        if(state) target.MapVRAvatar2();
        else target.MapVRAvatarController();
    }
}
