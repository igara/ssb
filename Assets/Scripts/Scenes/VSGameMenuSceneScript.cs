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

    public void OnEndDragPlayerIconRawImage()
    {
        if (playerIconRawImageScript.selectedCharacter == UserData.CharacterType.RobotKyleCharacter.ToString())
        {
            playerCharacterRawImage.gameObject.SetActive(true);
            playerCharacterRawImage.texture = robotKyleCharacterThumbnailRawImage.texture;
            startText.SetActive(true);
        }
        else
        {
            playerCharacterRawImage.gameObject.SetActive(false);
            startText.SetActive(false);
        }
    }

    public void OnEndDragCpu1IconRawImage()
    {
        if (cpu1IconRawImageScript.selectedCharacter == UserData.CharacterType.RobotKyleCharacter.ToString())
        {
            cpu1CharacterRawImage.gameObject.SetActive(true);
            cpu1CharacterRawImage.texture = robotKyleCharacterThumbnailRawImage.texture;
        }
        else
        {
            cpu1CharacterRawImage.gameObject.SetActive(false);
        }
    }

    public void OnEndDragCpu2IconRawImage()
    {
        if (cpu2IconRawImageScript.selectedCharacter == UserData.CharacterType.RobotKyleCharacter.ToString())
        {
            cpu2CharacterRawImage.gameObject.SetActive(true);
            cpu2CharacterRawImage.texture = robotKyleCharacterThumbnailRawImage.texture;
        }
        else
        {
            cpu2CharacterRawImage.gameObject.SetActive(false);
        }
    }

    public void OnEndDragCpu3IconRawImage()
    {
        if (cpu3IconRawImageScript.selectedCharacter == UserData.CharacterType.RobotKyleCharacter.ToString())
        {
            cpu3CharacterRawImage.gameObject.SetActive(true);
            cpu3CharacterRawImage.texture = robotKyleCharacterThumbnailRawImage.texture;
        }
        else
        {
            cpu3CharacterRawImage.gameObject.SetActive(false);
        }
    }
}
