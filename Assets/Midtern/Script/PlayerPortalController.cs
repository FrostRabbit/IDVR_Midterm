using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPortalController : MonoBehaviour
{
    public Animator blackScreen;
    public Transform Player;
    public float CloseTime = 3.0f;
    public GameObject Effect;
    public ServoDeviceControl servoDeviceControl;
    [Header("Stimulus Parameters")]
    public float AMPL;
    public float STIM;
    public int channel = 1;

    private float timer;
    public bool isTouching = false;
    private PortalController portalcontroller;
    private AudioSource audioSource;


    // Start is called before the first frame update
    void Start()
    {
        portalcontroller = GetComponentInParent<PortalController>();
        STIM = CloseTime;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isTouching)
        {
            timer += Time.deltaTime;
            if (timer >= CloseTime)
            {
                blackScreen.SetTrigger("Start");
                StartCoroutine(WaitForAnimationAndTeleport());
                
                timer = 0f;
                isTouching = false;
            }
        }
    }

    private IEnumerator WaitForAnimationAndTeleport()
    {
        // 等待動畫播放完成
        yield return new WaitForSeconds(blackScreen.GetCurrentAnimatorStateInfo(0).length);

        // 傳送玩家到指定的傳送門位置
        Player.position = portalcontroller.portals[portalcontroller.portalIndex].transform.position;
        // 保持相同的旋轉
        Player.rotation = portalcontroller.portals[portalcontroller.portalIndex].transform.rotation;

        // 關閉所有傳送門
        closeallportals();
    }

    void closeallportals()
    {
        foreach (GameObject portal in portalcontroller.portals)
        {
            portal.SetActive(false);
        }
        this.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerHand")
        {
            // 開始計時並記錄觸碰狀態
            isTouching = true;
            // 設置裝置參數並啟動
            // servoDeviceControl.SetValue(channel, AMPL, "AMPL");
            // servoDeviceControl.SetValue(channel, STIM, "STIM");
            // servoDeviceControl.StartStim(channel);
            // 生成特效
            Effect.SetActive(false);
            Effect.SetActive(true);

            // 播放音效
            audioSource.Play();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "PlayerHand")
        {
            Effect.SetActive(false);
            audioSource.Stop();
            // 當手離開傳送門時，重置計時器和觸碰狀態
            isTouching = false;
            timer = 0f;
        }
    }
}