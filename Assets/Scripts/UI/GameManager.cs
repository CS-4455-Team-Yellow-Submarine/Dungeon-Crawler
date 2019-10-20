using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public GameObject youwintext;
    public float resetDelay = 1.0f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
            Destroy(gameObject);
    }

    public void Win()
    {
        youwintext.SetActive(true);
        Time.timeScale = .5f;
        Invoke("Reset", resetDelay);

    }

    void Reset()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartScene");
    }

}
