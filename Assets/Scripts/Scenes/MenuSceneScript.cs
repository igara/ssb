﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSceneScript : MonoBehaviour
{
    [SerializeField]
    GameObject backRawImage;
    [SerializeField]
    GameObject backTappedRawImage;
    [SerializeField]
    GameObject vsGameRawImage;
    [SerializeField]
    GameObject tutorialRawImage;
    [SerializeField]
    GameObject creditRawImage;
    [SerializeField]
    UnityEngine.UI.Text descriptionText;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnMouseEnterBackRawImage()
    {
        backRawImage.SetActive(false);
        descriptionText.text = "戻る";
    }

    public void OnMouseExitBackRawImage()
    {
        backRawImage.SetActive(true);
        descriptionText.text = "";
    }

    public void OnMouseDownBackRawImage()
    {
        SceneManager.LoadScene("Scenes/StartScene");
    }

    public void OnMouseEnterVsGameRawImage()
    {
        descriptionText.text = "対戦ゲーム";
    }

    public void OnMouseExitVsGameRawImage()
    {
        descriptionText.text = "";
    }

    public void OnMouseDownVsGameRawImage()
    {
        SceneManager.LoadScene("Scenes/VSGameMenuScene");
    }

    public void OnMouseEnterCreditRawImage()
    {
        descriptionText.text = "クレジット";
    }

    public void OnMouseExitTutorialRawImage()
    {
        descriptionText.text = "";
    }

    public void OnMouseDownTutorialRawImage()
    {
        Link.Move("https://syonet.work/tools/ssb");
    }

    public void OnMouseEnterTutorialRawImage()
    {
        descriptionText.text = "チュートリアル";
    }

    public void OnMouseExitCreditRawImage()
    {
        descriptionText.text = "";
    }

    public void OnMouseDownCreditRawImage()
    {
        SceneManager.LoadScene("Scenes/CreditScene");
    }
}
