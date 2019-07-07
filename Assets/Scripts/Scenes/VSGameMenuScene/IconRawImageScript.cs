using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IconRawImageScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string selectedCharacter = "";

    [SerializeField]
    AudioSource selectCharacterAudioSource;

    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;

        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        foreach (var hit in raycastResults)
        {
            if (hit.gameObject.name == "RobotKyleCharacterThumbnailRawImage")
            {
                selectedCharacter = UserData.CharacterType.RobotKyleCharacter.ToString();
                break;
            }
            else if (hit.gameObject.name == "UnityChanCharacterThumbnailRawImage")
            {
                selectedCharacter = UserData.CharacterType.UnityChanCharacter.ToString();
                break;
            }
            else
            {
                selectedCharacter = "";
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (selectedCharacter == UserData.CharacterType.RobotKyleCharacter.ToString())
        {
            AudioClip audioClip = Resources.Load("Sounds/RobotKyle/RobotKyle") as AudioClip;
            selectCharacterAudioSource.clip = audioClip;
            selectCharacterAudioSource.Play();
        }
        else if (selectedCharacter == UserData.CharacterType.UnityChanCharacter.ToString())
        {
            AudioClip audioClip = Resources.Load("Sounds/UnityChan/UnityChan0045") as AudioClip;
            selectCharacterAudioSource.clip = audioClip;
            selectCharacterAudioSource.Play();
        }
    }
}
