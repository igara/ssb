using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityWebSocket;

public class StageSampleSceneScript : MonoBehaviour
{
    [SerializeField]
    GameObject gameOverRawImageGameObject;

    [SerializeField]
    GameObject defaultCharacterGameObject;

    [SerializeField]
    GameObject selectedCharactersGameObject;

    [SerializeField]
    GameObject charactersGameObject;

    private GameObject selfUserGameObject;

    private Dictionary<int, UserData> userDatasDictionary;
    private Dictionary<int, UserData> cpuUserDatasDictionary;

    private WebSocket ws;

    // Start is called before the first frame update
    void Start()
    {
        gameOverRawImageGameObject.SetActive(false);

        // Create WebSocket instance
        ws = new WebSocket(GameSetting.webSoketServerUri);
        userDatasDictionary = new Dictionary<int, UserData>();
        cpuUserDatasDictionary = new Dictionary<int, UserData>();

        // Add OnOpen event listener
        ws.onOpen += (object sender, EventArgs e) =>
        {
            GameSetting.selfUserData.webSocketStatus = (int)UserData.WebSocketStatus.OPEN;
            GameSetting.selfUserData.playerType = (int)UserData.PlayerType.MAN;
            GameSetting.selfUserData.unixTime = TimeUtil.GetUnixTime(DateTime.Now);
            GameSetting.selfUserData.position =
                defaultCharacterGameObject.transform.position;
            GameSetting.selfUserData.rotation =
                defaultCharacterGameObject.transform.rotation;
            string selfUserDataJsonString =
                JsonUtility.ToJson(GameSetting.selfUserData);
            ws.Send(Encoding.UTF8.GetBytes(selfUserDataJsonString));

            for (int i = 0; i < GameSetting.cpuUserDatas.Length; i++)
            {
                GameSetting.cpuUserDatas[i].name = "CPU";
                GameSetting.cpuUserDatas[i].webSocketStatus = (int)UserData.WebSocketStatus.OPEN;
                GameSetting.cpuUserDatas[i].playerType = (int)UserData.PlayerType.CPU;
                GameSetting.cpuUserDatas[i].unixTime = TimeUtil.GetUnixTime(DateTime.Now);
                GameSetting.cpuUserDatas[i].position =
                    defaultCharacterGameObject.transform.position;
                GameSetting.cpuUserDatas[i].rotation =
                    defaultCharacterGameObject.transform.rotation;
                string cpuUserDataJsonString =
                    JsonUtility.ToJson(GameSetting.cpuUserDatas[i]);
                ws.Send(Encoding.UTF8.GetBytes(cpuUserDataJsonString));
            }
        };

        // Add OnMessage event listener
        ws.onMessage += (object sender, MessageEventArgs e) =>
        {
            string message = e.Data;
            UserData userData = JsonUtility.FromJson<UserData>(message);

            for (int i = 0; i < GameSetting.cpuUserDatas.Length; i++)
            {
                if (
                    GameSetting.cpuUserDatas[i].unixTime == userData.unixTime &&
                    GameSetting.cpuUserDatas[i].playerType == userData.playerType
                )
                {
                    GameSetting.cpuUserDatas[i] = userData;
                    GameSetting.cpuUserDatas[i].webSocketStatus = (int)UserData.WebSocketStatus.BATTLE;
                    if (cpuUserDatasDictionary.ContainsKey(userData.id))
                    {
                        // すでにCPUが存在するとき
                        cpuUserDatasDictionary[userData.id] = userData;
                    }
                    else
                    {
                        if (GameSetting.selfUserData.webSocketStatus == (int)UserData.WebSocketStatus.DEAD || GameSetting.selfUserData.webSocketStatus == (int)UserData.WebSocketStatus.CLOSE)
                        {

                        }
                        else
                        {
                            // CPUが存在しないとき
                            cpuUserDatasDictionary.Add(userData.id, userData);
                        }
                    }
                }
            }

            if (
                GameSetting.selfUserData.name == userData.name &&
                GameSetting.selfUserData.unixTime == userData.unixTime
            )
            {
                if (GameSetting.selfUserData.webSocketStatus == (int)UserData.WebSocketStatus.OPEN)
                {
                    GameSetting.selfUserData = userData;
                    GameSetting.selfUserData.webSocketStatus = (int)UserData.WebSocketStatus.BATTLE;
                }
            }
            else
            {
                if (!cpuUserDatasDictionary.ContainsKey(userData.id))
                {
                    if (userDatasDictionary.ContainsKey(userData.id))
                    {
                        // すでにユーザが存在するとき
                        userDatasDictionary[userData.id] = userData;
                    }
                    else
                    {
                        if (userData.webSocketStatus == (int)UserData.WebSocketStatus.DEAD || userData.webSocketStatus == (int)UserData.WebSocketStatus.CLOSE)
                        {

                        }
                        else
                        {
                            // ユーザが存在しないとき
                            userDatasDictionary.Add(userData.id, userData);
                        }
                    }
                }
            }
        };

        // Add OnError event listener
        ws.onError += (object sender, ErrorEventArgs e) =>
        {
            Debug.Log("WS error: " + e.Message);
        };

        // Add OnClose event listener
        ws.onClose += (object sender, CloseEventArgs e) =>
        {
            GameSetting.selfUserData.webSocketStatus = (int)UserData.WebSocketStatus.CLOSE;
            string selfUserDataJsonString =
                JsonUtility.ToJson(GameSetting.selfUserData);
            ws.Send(Encoding.UTF8.GetBytes(selfUserDataJsonString));
        };

        // Connect to the server
        ws.Connect();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameSetting.selfUserData != null && GameSetting.selfUserData.id > 0)
        {
            string selfUserGameObjectName = $"{GameSetting.selfUserData.id}_{GameSetting.selfUserData.name}";

            if (GameObject.Find(selfUserGameObjectName) == null)
            {
                GameObject characterGameObject =
                    GameObject.Find(GameSetting.selfUserData.character);
                selfUserGameObject =
                    Instantiate(characterGameObject,
                    GameSetting.selfUserData.position,
                    GameSetting.selfUserData.rotation);
                selfUserGameObject.name = selfUserGameObjectName;

                // Stage GameObject下のキャラクター一覧にまとめる
                selfUserGameObject.transform.parent =
                    selectedCharactersGameObject.transform;

                if (GameSetting.selfUserData.character == UserData.CharacterType.RobotKyleCharacter.ToString())
                {
                    RobotKyleCharacterScript script = selfUserGameObject.AddComponent<RobotKyleCharacterScript>();
                    script.userData = GameSetting.selfUserData;
                }
            }
            else
            {
                if (GameSetting.selfUserData.character == UserData.CharacterType.RobotKyleCharacter.ToString())
                {
                    RobotKyleCharacterScript script = selfUserGameObject.GetComponent<RobotKyleCharacterScript>();
                    if (script.dead)
                    {
                        GameSetting.selfUserData.webSocketStatus = (int)UserData.WebSocketStatus.DEAD;
                    }
                }

                if (GameSetting.selfUserData.webSocketStatus == (int)UserData.WebSocketStatus.DEAD)
                {
                    gameOverRawImageGameObject.SetActive(true);
                }

                selfUserGameObject.transform.position =
                    new Vector3(selfUserGameObject.transform.position.x,
                        selfUserGameObject.transform.position.y,
                        defaultCharacterGameObject.transform.position.z);

                GameSetting.selfUserData.position = selfUserGameObject.transform.position;
                GameSetting.selfUserData.rotation = selfUserGameObject.transform.rotation;
                if (GameSetting.selfUserData.rotationStatus == (int)UserData.RotationStatus.RIGHT)
                {
                    selfUserGameObject.transform.rotation = Quaternion.Euler(
                        0,
                        (int)UserData.RotationStatus.RIGHT,
                        0
                    );
                }
                else if (GameSetting.selfUserData.rotationStatus == (int)UserData.RotationStatus.LEFT)
                {
                    selfUserGameObject.transform.rotation = Quaternion.Euler(
                        0,
                        (int)UserData.RotationStatus.LEFT,
                        0
                    );
                }
            }

            string selfUserDataJsonString =
                JsonUtility.ToJson(GameSetting.selfUserData);
            ws.Send(Encoding.UTF8.GetBytes(selfUserDataJsonString));

            if (GameSetting.selfUserData.webSocketStatus == (int)UserData.WebSocketStatus.DEAD)
            {
                Destroy(selfUserGameObject);
            }
        }

        List<int> cpuUserIds = new List<int>(cpuUserDatasDictionary.Keys);

        foreach (int id in cpuUserIds)
        {
            UserData cpuUserData = cpuUserDatasDictionary[id];
            string cpuUserGameObjectName = $"{cpuUserData.id}_{cpuUserData.name}";
            GameObject cpuUserGameObject = GameObject.Find(cpuUserGameObjectName);
            if (GameSetting.selfUserData.webSocketStatus == (int)UserData.WebSocketStatus.DEAD)
            {
                cpuUserDatasDictionary[id].webSocketStatus = (int)UserData.WebSocketStatus.DEAD;
                string cpuUserDataJsonString =
                    JsonUtility.ToJson(cpuUserDatasDictionary[id]);
                ws.Send(Encoding.UTF8.GetBytes(cpuUserDataJsonString));
                if (cpuUserGameObject)
                {
                    Destroy(cpuUserGameObject);
                }
            }
            else if (cpuUserGameObject == null && cpuUserData.webSocketStatus != (int)UserData.WebSocketStatus.DEAD)
            {
                GameObject characterGameObject =
                    GameObject.Find(cpuUserData.character);
                cpuUserGameObject =
                    Instantiate(characterGameObject,
                    cpuUserData.position,
                    cpuUserData.rotation);
                cpuUserGameObject.name = cpuUserGameObjectName;

                // Stage GameObject下のキャラクター一覧にまとめる
                cpuUserGameObject.transform.parent = selectedCharactersGameObject.transform;
                if (cpuUserData.character == UserData.CharacterType.RobotKyleCharacter.ToString())
                {
                    RobotKyleCharacterScript script = cpuUserGameObject.AddComponent<RobotKyleCharacterScript>();
                    script.userData = cpuUserData;
                }
            }
            else
            {
                if (cpuUserData.webSocketStatus == (int)UserData.WebSocketStatus.BATTLE)
                {
                    if (cpuUserData.character == UserData.CharacterType.RobotKyleCharacter.ToString())
                    {
                        RobotKyleCharacterScript script = cpuUserGameObject.GetComponent<RobotKyleCharacterScript>();
                        cpuUserData.inputType = script.getCpuInputType();
                        script.userData = cpuUserData;
                        if (script.dead)
                        {
                            cpuUserDatasDictionary[id].webSocketStatus = (int)UserData.WebSocketStatus.DEAD;
                            script.userData.webSocketStatus = (int)UserData.WebSocketStatus.DEAD;
                        }
                    }

                    cpuUserGameObject.transform.position =
                        new Vector3(cpuUserGameObject.transform.position.x,
                            cpuUserGameObject.transform.position.y,
                            defaultCharacterGameObject.transform.position.z);
                    cpuUserDatasDictionary[id].position = cpuUserGameObject.transform.position;
                    cpuUserDatasDictionary[id].rotation = cpuUserGameObject.transform.rotation;

                    if (cpuUserDatasDictionary[id].rotationStatus == (int)UserData.RotationStatus.RIGHT)
                    {
                        cpuUserGameObject.transform.rotation = Quaternion.Euler(
                            0,
                            (int)UserData.RotationStatus.RIGHT,
                            0
                        );
                    }
                    else if (cpuUserDatasDictionary[id].rotationStatus == (int)UserData.RotationStatus.LEFT)
                    {
                        cpuUserGameObject.transform.rotation = Quaternion.Euler(
                            0,
                            (int)UserData.RotationStatus.LEFT,
                            0
                        );
                    }
                    string cpuUserDataJsonString =
                        JsonUtility.ToJson(cpuUserDatasDictionary[id]);
                    ws.Send(Encoding.UTF8.GetBytes(cpuUserDataJsonString));
                }
            }
        }

        List<int> userIds = new List<int>(userDatasDictionary.Keys);

        foreach (int id in userIds)
        {
            UserData userData = userDatasDictionary[id];
            string userGameObjectName = $"{userData.id}_{userData.name}";
            GameObject userGameObject = GameObject.Find(userGameObjectName);
            if (userGameObject == null && userData.webSocketStatus != (int)UserData.WebSocketStatus.DEAD)
            {
                GameObject characterGameObject =
                    GameObject.Find(userData.character);
                userGameObject =
                    Instantiate(characterGameObject,
                    userData.position,
                    userData.rotation);
                userGameObject.name = userGameObjectName;

                // Stage GameObject下のキャラクター一覧にまとめる
                userGameObject.transform.parent = selectedCharactersGameObject.transform;
                if (userData.character == UserData.CharacterType.RobotKyleCharacter.ToString())
                {
                    RobotKyleCharacterScript script = userGameObject.AddComponent<RobotKyleCharacterScript>();
                    script.userData = userData;
                }
            }
            else
            {
                if (userData.webSocketStatus == (int)UserData.WebSocketStatus.BATTLE)
                {
                    if (userData.character == UserData.CharacterType.RobotKyleCharacter.ToString())
                    {
                        RobotKyleCharacterScript script = userGameObject.GetComponent<RobotKyleCharacterScript>();
                        script.userData = userData;
                    }
                    userGameObject.transform.position = userData.position;
                    userGameObject.transform.rotation = userData.rotation;

                    if (userData.rotationStatus == (int)UserData.RotationStatus.RIGHT)
                    {
                        userGameObject.transform.rotation = Quaternion.Euler(
                            0,
                            (int)UserData.RotationStatus.RIGHT,
                            0
                        );
                    }
                    else if (userData.rotationStatus == (int)UserData.RotationStatus.LEFT)
                    {
                        userGameObject.transform.rotation = Quaternion.Euler(
                            0,
                            (int)UserData.RotationStatus.LEFT,
                            0
                        );
                    }
                }

                if (userData.webSocketStatus == (int)UserData.WebSocketStatus.DEAD)
                {
                    userDatasDictionary.Remove(id);
                    Destroy(userGameObject);
                }
            }
        }
    }
}
