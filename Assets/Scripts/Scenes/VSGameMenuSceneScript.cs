using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VSGameMenuSceneScript : MonoBehaviour
{
    [SerializeField]
    GameObject backRawImage;
    [SerializeField]
    GameObject backTappedRawImage;
    [SerializeField]
    UnityEngine.UI.InputField userNameInputField;
    [SerializeField]
    UnityEngine.UI.Text userNameInputFieldText;

    [SerializeField]
    UnityEngine.UI.RawImage robotKyleCharacterThumbnailRawImage;

    [SerializeField]
    UnityEngine.UI.RawImage playerCharacterRawImage;
    [SerializeField]
    UnityEngine.UI.RawImage cpu1CharacterRawImage;
    [SerializeField]
    UnityEngine.UI.RawImage cpu2CharacterRawImage;
    [SerializeField]
    UnityEngine.UI.RawImage cpu3CharacterRawImage;
    [SerializeField]
    UnityEngine.UI.Text playerCharacterNameText;
    [SerializeField]
    UnityEngine.UI.Text cpu1CharacterNameText;
    [SerializeField]
    UnityEngine.UI.Text cpu2CharacterNameText;
    [SerializeField]
    UnityEngine.UI.Text cpu3CharacterNameText;

    [SerializeField]
    GameObject playerIconRawImage;
    [SerializeField]
    GameObject cpu1IconRawImage;
    [SerializeField]
    GameObject cpu2IconRawImage;
    [SerializeField]
    GameObject cpu3IconRawImage;
    IconRawImageScript playerIconRawImageScript;
    IconRawImageScript cpu1IconRawImageScript;
    IconRawImageScript cpu2IconRawImageScript;
    IconRawImageScript cpu3IconRawImageScript;

    [SerializeField]
    GameObject startText;

    // Start is called before the first frame update
    void Start()
    {
        userNameInputField.text = GameSetting.selfUserData.name;
        playerCharacterRawImage.gameObject.SetActive(false);
        cpu1CharacterRawImage.gameObject.SetActive(false);
        cpu2CharacterRawImage.gameObject.SetActive(false);
        cpu3CharacterRawImage.gameObject.SetActive(false);
        playerIconRawImageScript = playerIconRawImage.GetComponent<IconRawImageScript>();
        cpu1IconRawImageScript = cpu1IconRawImage.GetComponent<IconRawImageScript>();
        cpu2IconRawImageScript = cpu2IconRawImage.GetComponent<IconRawImageScript>();
        cpu3IconRawImageScript = cpu3IconRawImage.GetComponent<IconRawImageScript>();
        playerCharacterNameText.gameObject.SetActive(false);
        cpu1CharacterNameText.gameObject.SetActive(false);
        cpu2CharacterNameText.gameObject.SetActive(false);
        cpu3CharacterNameText.gameObject.SetActive(false);
        startText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerIconRawImageScript.selectedCharacter != "" && Input.GetKeyDown("return"))
        {
            GameSetting.selfUserData.name = userNameInputFieldText.text;
            GameSetting.selfUserData.character = playerIconRawImageScript.selectedCharacter;

            UserData[] cpuUserDatas = new UserData[0] { };
            if (cpu1IconRawImageScript.selectedCharacter != "")
            {
                Array.Resize(ref cpuUserDatas, cpuUserDatas.Length + 1);
                UserData userData = new UserData();
                userData.character = cpu1IconRawImageScript.selectedCharacter;
                cpuUserDatas[cpuUserDatas.Length - 1] = userData;
            }
            if (cpu2IconRawImageScript.selectedCharacter != "")
            {
                Array.Resize(ref cpuUserDatas, cpuUserDatas.Length + 1);
                UserData userData = new UserData();
                userData.character = cpu2IconRawImageScript.selectedCharacter;
                cpuUserDatas[cpuUserDatas.Length - 1] = userData;
            }
            if (cpu3IconRawImageScript.selectedCharacter != "")
            {
                Array.Resize(ref cpuUserDatas, cpuUserDatas.Length + 1);
                UserData userData = new UserData();
                userData.character = cpu3IconRawImageScript.selectedCharacter;
                cpuUserDatas[cpuUserDatas.Length - 1] = userData;
            }
            GameSetting.cpuUserDatas = cpuUserDatas;
            SceneManager.LoadScene("Scenes/StageSampleScene");
        }
    }

    public void OnMouseEnterBackRawImage()
    {
        backRawImage.SetActive(false);
    }

    public void OnMouseExitBackRawImage()
    {
        backRawImage.SetActive(true);
    }

    public void OnMouseDownBackRawImage()
    {
        SceneManager.LoadScene("Scenes/MenuScene");
    }

    public void OnDragPlayerIconRawImage()
    {
        ChangeCharacter(playerIconRawImageScript.selectedCharacter, playerCharacterRawImage, playerCharacterNameText);
    }

    public void OnEndDragPlayerIconRawImage()
    {
        if (playerIconRawImageScript.selectedCharacter != "")
        {
            startText.SetActive(true);
        }
        else
        {
            startText.SetActive(false);
        }
    }

    public void OnDragCpu1IconRawImage()
    {
        ChangeCharacter(cpu1IconRawImageScript.selectedCharacter, cpu1CharacterRawImage, cpu1CharacterNameText);
    }

    public void OnDragCpu2IconRawImage()
    {
        ChangeCharacter(cpu2IconRawImageScript.selectedCharacter, cpu2CharacterRawImage, cpu2CharacterNameText);
    }

    public void OnDragCpu3IconRawImage()
    {
        ChangeCharacter(cpu3IconRawImageScript.selectedCharacter, cpu3CharacterRawImage, cpu3CharacterNameText);
    }

    private void ChangeCharacter(string selectedCharacter, UnityEngine.UI.RawImage rawImage, UnityEngine.UI.Text text)
    {
        if (selectedCharacter == UserData.CharacterType.RobotKyleCharacter.ToString())
        {
            rawImage.gameObject.SetActive(true);
            rawImage.texture = robotKyleCharacterThumbnailRawImage.texture;
            text.gameObject.SetActive(true);
            text.text = "Robot Kyle";
        }
        else
        {
            rawImage.gameObject.SetActive(false);
            text.gameObject.SetActive(false);
        }
    }
}
