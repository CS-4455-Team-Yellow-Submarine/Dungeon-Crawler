using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class PauseMenuToggle : MonoBehaviour
{

    private CanvasGroup canvasGroup;
    private AudioSource pauseScreenMusic;
    private Camera camera;
    private AudioSource backgroundMusic;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (canvasGroup.interactable)
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.alpha = 0f;
                Time.timeScale = 1f;
                backgroundMusic.UnPause();
                pauseScreenMusic.Pause();
            }
            else
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                canvasGroup.alpha = 1f;
                Time.timeScale = 0f;
                backgroundMusic.Pause();
                pauseScreenMusic.Play();
            }
        }
    }

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            Debug.LogError("Canvas Group not found.");
        }

        pauseScreenMusic = GetComponent<AudioSource>();
        if (pauseScreenMusic == null)
        {
            Debug.LogError("Audio Source (pauseScreenMusic) not found.");
        }

        camera = Camera.main;
        if (camera == null)
        {
            Debug.LogError("Main Camera not found.");
        }

        backgroundMusic = camera.GetComponent<AudioSource>();
        if (backgroundMusic == null)
        {
            Debug.LogError("Audio Source (backgroundMusic) not found.");
        }
    }
}
