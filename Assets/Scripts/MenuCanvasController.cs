using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuCanvasController : MonoBehaviour
{

    public void Play()
    {
        Debug.Log("Loading level");
        StartCoroutine(SceneLoader(1f));
    }

    IEnumerator SceneLoader(float time)
    {
        yield return new WaitForSeconds(time);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void QuitGame()
    {
        Debug.Log("Exiting Game");
        StartCoroutine(QuitFade(1));
    }

    IEnumerator QuitFade(float time)
    {
        yield return new WaitForSeconds(time);
        Application.Quit();
    }
}