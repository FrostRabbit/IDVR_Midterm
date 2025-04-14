using UnityEngine;

public class PortalCameraController : MonoBehaviour
{
    public Transform playerCamera;   // 玩家相機
    public Transform portalA;        // 入口傳送門
    public Transform portalB;        // 出口傳送門
    public Camera portalCamera;      // 出口傳送門相機

    void Update()
    {
        // 計算入口和出口之間的旋轉差異 
        Quaternion portalRotationDifference = Quaternion.Inverse(portalA.rotation) * portalB.rotation;

        // 計算玩家頭盔相對於入口傳送門的角度
        Vector3 playerOffsetFromPortalA = playerCamera.position - portalA.position;
        Vector3 newCameraPosition = portalB.position + portalRotationDifference * playerOffsetFromPortalA;

        // 計算新的相機方向
        Vector3 newCameraDirection = portalRotationDifference * (portalA.position - playerCamera.position).normalized;

        // 固定上下角度
        newCameraDirection.y = 0;
        newCameraDirection.Normalize();


        // 更新相機的位置和旋轉方向
        portalCamera.transform.rotation = Quaternion.LookRotation(newCameraDirection, Vector3.up);
    }
}