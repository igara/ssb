using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneScript : MonoBehaviour
{
    [SerializeField]
    GameObject title1RawImage;
    [SerializeField]
    GameObject title2RawImage;
    [SerializeField]
    UnityEngine.UI.Text tmText;
    [SerializeField]
    UnityEngine.UI.Button startButton;
    [SerializeField]
    UnityEngine.UI.Text copyrightText;
    [SerializeField]
    AudioSource startAudioSource;

    private int title1RawImageType = 1;
    private float timer;
    private int time = 1;

    // Start is called before the first frame update
    void Start()
    {
        startButton.onClick.AddListener(OnClickStartButton);

        int year = DateTime.Now.Year;
        copyrightText.text = $"©{year} syonet.work / Syo Igarashi";
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0.0)
        {
            timer = 0.1f;
            time -= 1;
            if (time == 0)
            {
                time = 2;
                if (title1RawImageType == 1)
                {
                    title1RawImageType = 2;
                    title1RawImage.SetActive(false);
                    tmText.color = new Color(255.0f / 255.0f, 255.0f / 255.0f, 0.0f / 255.0f);
                }
                else
                {
                    title1RawImageType = 1;
                    title1RawImage.SetActive(true);
                    tmText.color = new Color(255.0f / 255.0f, 0.0f / 255.0f, 0.0f / 255.0f);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            SceneManager.LoadScene("Scenes/StageSampleScene");
        }
    }

    void OnClickStartButton()
    {
        startAudioSource.Play();
        StartCoroutine(StartAudioSourceChecking(() =>
        {
            SceneManager.LoadScene("Scenes/MenuScene");
        }));
    }

    public delegate void functionType();

    private IEnumerator StartAudioSourceChecking(functionType callback)
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            if (!startAudioSource.isPlaying)
            {
                callback();
                break;
            }
        }
    }
}
