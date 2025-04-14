using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoaderManager : MonoBehaviour
{
    #region Singleton 
    public static SceneLoaderManager instance;
    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of SceneManager found!");
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

    // Start is called before the first frame update
    public Animator transition;
    public float transitionTime = 1f;
    public void LoadStartScene()
    {
        StartCoroutine(LoadScene("StartScene", 0));
    }
    public void LoadStage1()
    {
        StartCoroutine(LoadScene("Stage1", 1));
    }
    public void Stop()
    {
        Time.timeScale = 0.0f;
    }
    public void Resume()
    {
        //ClickSound();
        Time.timeScale = 1f;
    }
    /*public void ClickSound()
    {
        AudioManager.instance.Play("Click");
    }*/
    public void LoadStage2()
    {
        StartCoroutine(LoadScene("Stage2", 2));
    }
    public void Exit()
    {
        Application.Quit();
    }
    IEnumerator LoadScene(string scene, int index)
    {
        //AudioManager.instance.Play("Click");

        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(scene);
        // GameManager.instance.SceneIndex = index;
    }
}
