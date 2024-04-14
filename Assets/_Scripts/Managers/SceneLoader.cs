using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;
    [SerializeField] Animator _sceneTransitionAnimator;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _sceneTransitionAnimator.SetBool("IsLoading", false);
    }


    public IEnumerator LoadScene(string sceneName)
    {
        GameManager.Instance.SetTimeScale(1f);
        GameManager.Instance.SetTargetTimeScale(1f);
        Debug.Log("Loading scene : " + sceneName);

        yield return StartCoroutine(AnimateIn());
        //SceneManager.LoadScene(sceneName);
        yield return StartCoroutine(LoadSceneProgress(sceneName));
        yield return StartCoroutine(AnimateOut());
    }

    public IEnumerator LoadSceneProgress(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;
        while (operation.progress < 0.9f)
        {
            yield return new WaitForEndOfFrame();
        }
        operation.allowSceneActivation = true;
    }


    public IEnumerator AnimateIn()
    {

        GameManager.Instance.SetTimeScale(1f);
        GameManager.Instance.SetTargetTimeScale(1f);
        _sceneTransitionAnimator.SetBool("IsLoading", true);
        yield return new WaitForSeconds(1);
    }

    public IEnumerator AnimateOut()
    {
        _sceneTransitionAnimator.SetBool("IsLoading", false);
        yield return new WaitForSeconds(1);
    }

    public void LoadGameScene()
    {
        GameManager.Instance.ResetLevel();
        StartCoroutine(LoadScene("GameScene"));
    }
}
