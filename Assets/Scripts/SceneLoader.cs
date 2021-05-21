using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    private string sceneNameToBeLoaded;
    public void LoadScene(string _sceneName)
    {
        sceneNameToBeLoaded = _sceneName;

        StartCoroutine(InitializeSceneLoading());
    }

    IEnumerator InitializeSceneLoading()
    {
        //1st , load the Loading Scene!
        yield return SceneManager.LoadSceneAsync("Scene_Loading");

        //Load the Actual Scene!
        StartCoroutine(LoadActualScene());
    }

    IEnumerator LoadActualScene()
    {
        var asynceSceneLoading = SceneManager.LoadSceneAsync(sceneNameToBeLoaded);

        // this value stops the scene from displaying when it is still loading!
        asynceSceneLoading.allowSceneActivation = false;

        while (!asynceSceneLoading.isDone)
        {
            Debug.Log(asynceSceneLoading.progress);
            if(asynceSceneLoading.progress >= 0.9f)
            {
                // Finally shows the scene!
                asynceSceneLoading.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
