using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData
{
    public enum CharacterType
    {
        RobotKyleCharacter
    }

    public enum WebSocketStatus
    {
        OPEN,
        BATTLE,
        DEAD,
        CLOSE
    }

    public enum RotationStatus
    {
        RIGHT = 90,
        LEFT = 270
    }

    public enum PlayerType
    {
        MAN,
        CPU
    }

    public int id = 0;

    public string name = "noname";

    public int webSocketStatus = (int)WebSocketStatus.CLOSE;

    public int rotationStatus = (int)RotationStatus.RIGHT;

    public int playerType;

    public long unixTime = 0;

    public string character = CharacterType.RobotKyleCharacter.ToString();

    public Vector3 position;

    public Quaternion rotation;

    public int inputType = (int)InputType.KeyType.wait;
}
