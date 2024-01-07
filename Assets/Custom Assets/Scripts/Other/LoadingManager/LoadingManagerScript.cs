using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using GameEnumsNamespace;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance { get; private set; }
    [SerializeField] Canvas myCanvas;
    [SerializeField] Animator myAnimator;
    [SerializeField] AnimationClip fadeInAnimationClip;
    private float fadeInAnimationClipLength;
    [SerializeField] AnimationClip fadeOutAnimationClip;
    private float fadeOutAnimationClipLength;

    private AsyncOperation asyncLoad;

    [SerializeField] Image loadingImage;
    [SerializeField] Image fadeImage;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        fadeInAnimationClipLength = fadeInAnimationClip.length;
        fadeOutAnimationClipLength = fadeOutAnimationClip.length;
        StartCoroutine(LoadInitialization());
    }

    public void LoadScene(GameSceneEnums sceneEnum)
    {
        StartCoroutine(BeginLoad(sceneEnum));
    }

    IEnumerator LoadInitialization()
    {
        myAnimator.Play("FadeIn");
        yield return new WaitForSeconds(fadeInAnimationClipLength);

        asyncLoad = SceneManager.LoadSceneAsync(GameSceneEnums.MainMenu.ToString());
        asyncLoad.allowSceneActivation = false; // Prevent the scene from activating immediately

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f) // 0.9 is usually when it's ready to activate
            {
                myAnimator.Play("FadeOut");
                
                yield return new WaitForSeconds(fadeOutAnimationClipLength);

                loadingImage.gameObject.SetActive(false);
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    IEnumerator BeginLoad(GameSceneEnums sceneEnum)
    {
        myAnimator.Play("FadeOut");
        yield return new WaitForSeconds(fadeOutAnimationClipLength);
        loadingImage.gameObject.SetActive(true);
        myAnimator.Play("FadeIn");
        yield return new WaitForSeconds(fadeInAnimationClipLength);


        asyncLoad = SceneManager.LoadSceneAsync(sceneEnum.ToString());
        asyncLoad.allowSceneActivation = false; // Prevent the scene from activating immediately

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f) // 0.9 is usually when it's ready to activate
            {
                myAnimator.Play("FadeOut");
                
                yield return new WaitForSeconds(fadeOutAnimationClipLength);

                loadingImage.gameObject.SetActive(false);
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        myAnimator.Play("FadeIn");
    }
}
