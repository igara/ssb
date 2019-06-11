using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData
{
    public enum CharacterType
    {
        SampleCharacter
    }

    public enum Status
    {
        OPEN,
        BATTLE,
        DEAD,
        CLOSE
    }

    public enum PlayerType
    {
        MAN,
        CPU
    }

    public int id = 0;

    public string name = "noname";

    public int status = (int)Status.CLOSE;

    public int playerType;

    public long unixTime = 0;

    public string character = CharacterType.SampleCharacter.ToString();

    public Vector3 position;

    public Quaternion rotation;

    public int inputType = (int)InputType.KeyType.wait;
}
