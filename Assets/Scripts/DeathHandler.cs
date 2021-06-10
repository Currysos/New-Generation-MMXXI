using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

public class DeathHandler : MonoBehaviour
{
    public PlayableDirector director;
    public Animator BackgroundFadeAnim;

    private void Awake()
    {
        director.played += DirectorStarted;
        director.stopped += DirectorFinished;
    }

    public void StartDeathSequence()
    {
        director.Play();
    }

    private void DirectorFinished(PlayableDirector obj)
    {
        StartCoroutine(SceneLoader());
    }

    private void DirectorStarted(PlayableDirector obj)
    {
        BackgroundFadeAnim.SetTrigger("FadeOut");
    }
    
    IEnumerator SceneLoader()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(0);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
