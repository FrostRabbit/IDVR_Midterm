//
// Mecanimのアニメーションデータが、原点で移動しない場合の Rigidbody付きコントローラ
// サンプル
// 2014/03/13 N.Kobyasahi
//
using UnityEngine;
using System.Collections;
using System;
using Unity.VisualScripting;
using UnityEditor.PackageManager;

namespace UnityChan
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Rigidbody))]

	public class UnityChanControlScriptWithRgidBody : MonoBehaviour
	{
        #region Singleton 
        public static UnityChanControlScriptWithRgidBody instance;
        void Awake()
        {
            if (instance != null)
            {
                Debug.LogWarning("More than one instance of UnityChanControlScriptWithRgidBody found!");
                return;
            }
            else
            {
                instance = this;
            }
        }

        #endregion
        public float animSpeed = 1.5f;	
		public float lookSmoother = 3.0f;		
		public bool useCurves = true;	
		public float useCurvesHeight = 0.5f;		
		public float forwardSpeed = 7.0f;
		public float backwardSpeed = 2.0f;
		public float rotateSpeed = 2.0f;
		public float jumpPower = 3.0f; 
		private CapsuleCollider col;
		private Rigidbody rb;
		private Vector3 velocity;
		private Vector3 side_velocity;
		private float orgColHight;
		private Vector3 orgVectColCenter;
		private Animator anim;							

		private GameObject cameraObject;	
		
		static string idleState = "Pending";
		static string forwardState = "Forwarding";
		static string backwardState = "Backwarding";
		static string RightSideStepState = "RightSideStep";
		static string LeftSideStepdState = "LeftSideStep";
		static string TurnRightState = "Turn Right";
		static string TurnLeftState = "Turning Left";
		static string jumpState = "Jump";
		static string runState = "Run";
		private LocomotionStateMachine.LocomotionState LS;
        float h;
        float v;
        bool isGrounded;
        bool isClimbimg;
        private float GroundCheckRadius;
        public LayerMask groundMask;
		public bool is_climbing = false;
		GameManager gm;
		public bool use_joystick = false;
		GameObject CamAndParent;
		Transform _transform;
        void Start ()
		{
			anim = GetComponent<Animator> ();
			col = GetComponent<CapsuleCollider> ();
			rb = GetComponent<Rigidbody> ();
			orgColHight = col.height;
			orgVectColCenter = col.center;
            GroundCheckRadius = col.height/2;

            LS = LocomotionStateMachine.LocomotionStateMachine.instance.CurrentState;
			gm = GameManager.instance;
			CamAndParent = new GameObject();
        }

        void FixedUpdate ()
		{
			if ((!gm.is_left_hand_grab && !gm.is_right_hand_grab) && (!gm.is_left_hand_controller_grab && !gm.is_right_hand_controller_grab))
			{
				is_climbing = false;
				gm.player_grab = 0;	
			}
			gm.DetermineHandState();  // 這邊本來是想用 grabbable 呼叫再判斷，但是 release 的時候好像沒有完全偵測到，所以 update every frame..
            if (is_climbing)
			{
                rb.useGravity = false;
            }
			else
			{
                rb.useGravity = true;
            }
            // isGrounded = Physics.CheckSphere(transform.position, GroundCheckRadius, groundMask);
            isGrounded = Physics.CheckCapsule(transform.position - transform.up * col.height / 2 + Vector3.forward * 0.5f, transform.position + transform.up * col.height / 2 + Vector3.forward * 0.5f, 0.1f, groundMask);
			// check the layer around player is stair or hill
			if (Physics.CheckCapsule(transform.position - transform.up * col.height / 2 + Vector3.forward * 0.5f, transform.position + transform.up * col.height / 2 + Vector3.forward * 0.5f, 0.1f, gm.stairMask))
			{
				gm.player_stay = 1;
			}
			else if (Physics.CheckCapsule(transform.position - transform.up * col.height / 2 + Vector3.forward * 0.5f, transform.position + transform.up * col.height / 2 + Vector3.forward * 0.5f, 0.1f, gm.hillMask))
            {
                gm.player_stay = 2;
            }
			else {
                gm.player_stay = 0;
            }


            Debug.Log("@@@@@@@@@@@@isGrounded" + isGrounded);
            LS = LocomotionStateMachine.LocomotionStateMachine.instance.CurrentState;
            (h, v) = ReturnMoveMent(h, v);
            anim.SetFloat("Speed", v);                          
            anim.SetFloat ("Direction", h); 						
			anim.speed = animSpeed;


            
			CamAndParent.transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles.x-transform.rotation.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y- transform.rotation.eulerAngles.y, Camera.main.transform.rotation.eulerAngles.z- transform.rotation.eulerAngles.z);
			if (GameDataManager.instance.forward_type == 0) _transform = CamAndParent.transform;
			else _transform = transform;
            // 左右移動
            if (GameDataManager.instance.forward_type == 0) side_velocity = _transform.right * h;
            else side_velocity = _transform.forward * h;  // because player init is 45 degree
            side_velocity *= backwardSpeed*0.3f;


            // 前後
            if (GameDataManager.instance.forward_type == 0) velocity = transform.TransformDirection((_transform.forward) * v);
            else velocity = _transform.right * -v;
            velocity *= backwardSpeed;

			
            if ((LS.State == jumpState || (OVRInput.Get(OVRInput.Button.Two) && use_joystick)) && rb.useGravity) {	

				if (!anim.IsInTransition (0)) {
					if (isGrounded) // add in Jump state start
					{
						rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
                    }
                    anim.SetBool ("Jump", true);		
				}
			}

            if ((LS.State == TurnLeftState || LS.State == TurnRightState) && GameDataManager.instance.turn_type == 1)
            {
                Quaternion targetRot;
				if(LS.State == TurnRightState) targetRot = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + 45, transform.rotation.eulerAngles.z);
				else targetRot = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y - 45, transform.rotation.eulerAngles.z);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime);
            }
            // move
            if (h == 0 && rb.useGravity) transform.localPosition += velocity * Time.fixedDeltaTime;
            else if (v == 0 && rb.useGravity) transform.localPosition += side_velocity * Time.fixedDeltaTime;

            // transform.Rotate (0, h * rotateSpeed, 0);	

            
            if (LS.State == forwardState || LS.State == backwardState || LS.State == LeftSideStepdState || LS.State == LeftSideStepdState) {
				if (useCurves) {
					resetCollider ();
				}
			}

            else if ((LS.State == jumpState || (OVRInput.Get(OVRInput.Button.Two) && use_joystick)) && rb.useGravity) {
				if (!anim.IsInTransition (0)) {
                    if (useCurves) {
                        // 下面附加到 JUMP00 動畫的曲線 JumpHeight 和 GravityControl
                        // JumpHeight：JUMP00處的跳躍高度（0到1）
                        // GravityControl:1 ⇒ 跳躍（禁用重力），0 ⇒ 啟用重力
                        float jumpHeight = anim.GetFloat ("JumpHeight");
						float gravityControl = anim.GetFloat ("GravityControl"); 
						if (gravityControl > 0)
							rb.useGravity = false;	
										
						Ray ray = new Ray (transform.position + Vector3.up, -Vector3.up);
						RaycastHit hitInfo = new RaycastHit ();
						if (Physics.Raycast (ray, out hitInfo)) {
							if (hitInfo.distance > useCurvesHeight) {
                                col.height = orgColHight - jumpHeight;			
								float adjCenterY = orgVectColCenter.y + jumpHeight;
								col.center = new Vector3 (0, adjCenterY, 0);	
							} else {		
								resetCollider ();
							}
						}
					}
                    StartCoroutine(WaitToCancelJump());
                    //anim.SetBool("Jump", false);
                }
			}
			else if (LS.State == idleState) {
				if (useCurves) {
					resetCollider ();
				}
				if (Input.GetButtonDown ("Jump")) {
					anim.SetBool ("Rest", true);
				}
			}
			else if (anim.GetBool("Rest")) {
				if (!anim.IsInTransition (0)) {
					anim.SetBool ("Rest", false);
				}
			}
		}

		/*void OnGUI ()
		{
			GUI.Box (new Rect (Screen.width - 260, 10, 250, 150), "Interaction");
			GUI.Label (new Rect (Screen.width - 245, 30, 250, 30), "Up/Down Arrow : Go Forwald/Go Back");
			GUI.Label (new Rect (Screen.width - 245, 50, 250, 30), "Left/Right Arrow : Turn Left/Turn Right");
			GUI.Label (new Rect (Screen.width - 245, 70, 250, 30), "Hit Space key while Running : Jump");
			GUI.Label (new Rect (Screen.width - 245, 90, 250, 30), "Hit Spase key while Stopping : Rest");
			GUI.Label (new Rect (Screen.width - 245, 110, 250, 30), "Left Control : Front Camera");
			GUI.Label (new Rect (Screen.width - 245, 130, 250, 30), "Alt : LookAt Camera");
		}*/


		void resetCollider ()
		{
			col.height = orgColHight;
			col.center = orgVectColCenter;
		}
		(float, float) ReturnMoveMent(float h, float v) // return with state machine, return(h, v) // h 控制左右，v 控制前後
        {
            //Debug.Log("@@@@@@@@@@@@@@ LS.State:" + LS.State);
            Vector2 primaryAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
            if (use_joystick)
            {
                if (primaryAxis != Vector2.zero)
                {
                    if (primaryAxis.y > 0.95) return (0, 0.8f);
                    if (primaryAxis.x > 0.9) return (1, 0);
                    if (primaryAxis.x < -0.9) return (-1, 0);
                    if (primaryAxis.y > 0.1) return (0, 0.4f);
                    if (primaryAxis.y < -0.1) return (0, -0.4f);
                }
                else return (0, 0);
            }
            if (!isGrounded)
			{
                return(h, v);
            }
			else if (LS.State == idleState)
			{
				return (0, 0);
			}
            else if (LS.State == runState)
            {
                return (0, 0.8f);
            }
            else if (LS.State == forwardState)
			{
                return (0, 0.4f);
			}
			else if (LS.State == backwardState)
			{
				return (0, -0.4f);
			}
			else if (LS.State == RightSideStepState)
			{
				return (1, 0);
			}
			else if (LS.State == LeftSideStepdState)
			{
				return (-1, 0);
			}

            return (h, v);
        }

		IEnumerator WaitToCancelJump()
		{
			yield return new WaitForSeconds(0.1f);
            anim.SetBool("Jump", false);
        }

        private void OnDrawGizmos()
        {
			//Gizmos.DrawSphere(transform.position, GroundCheckRadius);
			//Gizmos.DrawSphere(transform.position, col.radius);
			//Gizmos.DrawLine(transform.position - transform.up * col.height / 2 + Vector3.forward*0.5f, transform.position + transform.up * col.height / 2 + Vector3.forward * 0.5f);
        }

        /*public static Vector3 GetProjectOnPlane(Vector3 vector, Vector3 planeNormal)
        {
            return (vector - planeNormal * Vector3.Dot(vector, planeNormal)).normalized;
        }*/

		public void IsClimbing(Vector3 movement, bool dir = false)
		{
			//isClimbimg = Physics.CheckSphere(transform.position, col.radius + 0.01f, groundMask);
			/*RaycastHit[] front_col = Physics.RaycastAll(transform.position, transform.forward);
            if (front_col.Length != 0)
            {
                Debug.Log("@@@@@@@@@@@@front_col[0].transform.name" + front_col[0].transform.name);
                GetProjectOnPlane(transform.position, front_col[0].transform.position);
                transform.localPosition += GetProjectOnPlane(movement, front_col[0].transform.position) * Time.fixedDeltaTime;
            }
			else
			{
                transform.localPosition -= movement * Time.fixedDeltaTime;
            }*/
			//Debug.Log("@@@@@@@@@@@@isClimbimg" + isClimbimg);
			//movement.z = 0;
			if(dir) transform.localPosition += movement * Time.fixedDeltaTime;
            else transform.localPosition -= movement * Time.fixedDeltaTime;

        }
    }
}