using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    private long unixTime;

    // Start is called before the first frame update
    void Start()
    {
        gameOverRawImageGameObject.SetActive(false);

        // Create WebSocket instance
        ws = new WebSocket(GameSetting.webSoketServerUri);
        userDatasDictionary = new Dictionary<int, UserData>();
        cpuUserDatasDictionary = new Dictionary<int, UserData>();
        unixTime = TimeUtil.GetUnixTime(DateTime.Now);

        // Add OnOpen event listener
        ws.onOpen += (object sender, EventArgs e) =>
        {
            GameSetting.selfUserData.webSocketStatus = (int)UserData.WebSocketStatus.OPEN;
            GameSetting.selfUserData.playerType = (int)UserData.PlayerType.MAN;
            GameSetting.selfUserData.unixTime = unixTime;
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
                UserData cpuUserData = GameSetting.cpuUserDatas[i];
                if (
                    cpuUserData.unixTime == userData.unixTime &&
                    cpuUserData.playerType == userData.playerType
                )
                {
                    cpuUserData = userData;
                    if (cpuUserData.webSocketStatus == (int)UserData.WebSocketStatus.OPEN)
                    {
                        cpuUserData.webSocketStatus = (int)UserData.WebSocketStatus.BATTLE;
                    }

                    if (cpuUserDatasDictionary.ContainsKey(userData.id))
                    {
                        // すでにCPUが存在するとき
                        cpuUserDatasDictionary[userData.id] = userData;
                    }
                    else if (
                        GameSetting.selfUserData.webSocketStatus == (int)UserData.WebSocketStatus.DIE ||
                        GameSetting.selfUserData.webSocketStatus == (int)UserData.WebSocketStatus.DEAD ||
                        GameSetting.selfUserData.webSocketStatus == (int)UserData.WebSocketStatus.CLOSE)
                    {

                    }
                    else
                    {
                        // CPUが存在しないとき
                        cpuUserDatasDictionary.Add(userData.id, userData);
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
                    else if (
                        userData.webSocketStatus == (int)UserData.WebSocketStatus.DIE ||
                        userData.webSocketStatus == (int)UserData.WebSocketStatus.DEAD ||
                        userData.webSocketStatus == (int)UserData.WebSocketStatus.CLOSE)
                    {

                    }
                    else
                    {
                        // ユーザが存在しないとき
                        userDatasDictionary.Add(userData.id, userData);
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
            // GameSetting.selfUserData.webSocketStatus = (int)UserData.WebSocketStatus.CLOSE;
            // string selfUserDataJsonString =
            //     JsonUtility.ToJson(GameSetting.selfUserData);
            // ws.Send(Encoding.UTF8.GetBytes(selfUserDataJsonString));
        };

        // Connect to the server
        ws.Connect();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameSetting.selfUserData != null && GameSetting.selfUserData.id > 0 && GameSetting.selfUserData.unixTime == unixTime)
        {
            if (Input.GetKeyDown("r") && (GameSetting.selfUserData.webSocketStatus == (int)UserData.WebSocketStatus.DEAD || GameSetting.selfUserData.webSocketStatus == (int)UserData.WebSocketStatus.DIE))
            {
                ws.Close();
                SceneManager.LoadScene("Scenes/VSGameMenuScene");
                return;
            }
            else if (GameSetting.selfUserData.webSocketStatus == (int)UserData.WebSocketStatus.BATTLE)
            {
                string selfUserGameObjectName = $"{GameSetting.selfUserData.id}_{GameSetting.selfUserData.name}";

                if (GameObject.Find(selfUserGameObjectName) == null)
                {
                    // キャラクターのGameObjectを作成
                    selfUserGameObject = CreateUserCharacterGameObject(selfUserGameObject, GameSetting.selfUserData, selfUserGameObjectName);
                    StartCoroutine(WaitCreateGameObject(selfUserGameObject, GameSetting.selfUserData));
                }
                else
                {
                    UpdateUserData(selfUserGameObject, GameSetting.selfUserData);

                    if (
                        GameSetting.selfUserData.webSocketStatus == (int)UserData.WebSocketStatus.DIE ||
                        GameSetting.selfUserData.webSocketStatus == (int)UserData.WebSocketStatus.DEAD)
                    {
                        gameOverRawImageGameObject.SetActive(true);
                    }
                }

                string selfUserDataJsonString =
                    JsonUtility.ToJson(GameSetting.selfUserData);
                ws.Send(Encoding.UTF8.GetBytes(selfUserDataJsonString));

                if (
                    GameSetting.selfUserData.webSocketStatus == (int)UserData.WebSocketStatus.DIE ||
                    GameSetting.selfUserData.webSocketStatus == (int)UserData.WebSocketStatus.DEAD)
                {
                    StartCoroutine(WaitDestroyGameObject(selfUserGameObject, GameSetting.selfUserData));
                }
            }
        }

        List<int> cpuUserIds = new List<int>(cpuUserDatasDictionary.Keys);

        foreach (int id in cpuUserIds)
        {
            UserData cpuUserData = cpuUserDatasDictionary[id];
            string cpuUserGameObjectName = $"{cpuUserData.id}_{cpuUserData.name}";
            GameObject cpuUserGameObject = GameObject.Find(cpuUserGameObjectName);

            if (
                (
                    GameSetting.selfUserData.webSocketStatus == (int)UserData.WebSocketStatus.DIE ||
                    GameSetting.selfUserData.webSocketStatus == (int)UserData.WebSocketStatus.DEAD
                ) ||
                (
                    cpuUserData.webSocketStatus == (int)UserData.WebSocketStatus.DIE ||
                    cpuUserData.webSocketStatus == (int)UserData.WebSocketStatus.DEAD
                )
            )
            {
                cpuUserData.webSocketStatus = (int)UserData.WebSocketStatus.DEAD;
                string cpuUserDataJsonString =
                    JsonUtility.ToJson(cpuUserData);
                ws.Send(Encoding.UTF8.GetBytes(cpuUserDataJsonString));

                if (cpuUserGameObject)
                {
                    StartCoroutine(WaitDestroyGameObject(cpuUserGameObject, cpuUserData));
                }
            }
            else if (cpuUserGameObject == null && cpuUserData.webSocketStatus == (int)UserData.WebSocketStatus.BATTLE)
            {
                // キャラクターのGameObjectを作成
                cpuUserGameObject = CreateUserCharacterGameObject(cpuUserGameObject, cpuUserData, cpuUserGameObjectName);
                StartCoroutine(WaitCreateGameObject(cpuUserGameObject, cpuUserData));
            }
            else
            {
                if (cpuUserData.webSocketStatus == (int)UserData.WebSocketStatus.BATTLE)
                {
                    UpdateUserData(cpuUserGameObject, cpuUserData);

                    string cpuUserDataJsonString =
                        JsonUtility.ToJson(cpuUserData);
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
            if (
                userGameObject == null &&
                (
                    userData.webSocketStatus != (int)UserData.WebSocketStatus.DIE ||
                    userData.webSocketStatus != (int)UserData.WebSocketStatus.DEAD)
                )
            {
                // キャラクターのGameObjectを作成
                userGameObject = CreateUserCharacterGameObject(userGameObject, userData, userGameObjectName);
                StartCoroutine(WaitCreateGameObject(userGameObject, userData));
            }
            else
            {
                if (userData.webSocketStatus == (int)UserData.WebSocketStatus.BATTLE)
                {
                    UpdateUserData(userGameObject, userData);
                }

                if (
                    userData.webSocketStatus == (int)UserData.WebSocketStatus.DIE ||
                    userData.webSocketStatus == (int)UserData.WebSocketStatus.DEAD)
                {
                    userDatasDictionary.Remove(id);
                    StartCoroutine(WaitDestroyGameObject(userGameObject, userData));
                }
            }
        }
    }

    GameObject CreateUserCharacterGameObject(GameObject userGameObject, UserData userData, string userGameObjectName)
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
        if (userData.character == UserData.CharacterType.UnityChanCharacter.ToString())
        {
            UnityChanCharacterScript script = userGameObject.AddComponent<UnityChanCharacterScript>();
            script.userData = userData;
        }
        return userGameObject;
    }

    void UpdateUserData(GameObject userGameObject, UserData savedUserData)
    {
        if (savedUserData.character == UserData.CharacterType.RobotKyleCharacter.ToString())
        {
            RobotKyleCharacterScript script = userGameObject.GetComponent<RobotKyleCharacterScript>();
            if (savedUserData.playerType == (int)UserData.PlayerType.CPU)
            {
                savedUserData.inputType = script.GetCpuInputType();
            }
            script.userData = savedUserData;
            if (script.dead && savedUserData.webSocketStatus == (int)UserData.WebSocketStatus.BATTLE)
            {
                savedUserData.webSocketStatus = (int)UserData.WebSocketStatus.DIE;
                script.userData.webSocketStatus = (int)UserData.WebSocketStatus.DIE;
            }
            else if (script.dead && savedUserData.webSocketStatus == (int)UserData.WebSocketStatus.DIE)
            {
                savedUserData.webSocketStatus = (int)UserData.WebSocketStatus.DEAD;
                script.userData.webSocketStatus = (int)UserData.WebSocketStatus.DEAD;
            }
        }

        if (savedUserData.character == UserData.CharacterType.UnityChanCharacter.ToString())
        {
            UnityChanCharacterScript script = userGameObject.GetComponent<UnityChanCharacterScript>();
            if (savedUserData.playerType == (int)UserData.PlayerType.CPU)
            {
                savedUserData.inputType = script.GetCpuInputType();
            }
            script.userData = savedUserData;
            if (script.dead && savedUserData.webSocketStatus == (int)UserData.WebSocketStatus.BATTLE)
            {
                savedUserData.webSocketStatus = (int)UserData.WebSocketStatus.DIE;
                script.userData.webSocketStatus = (int)UserData.WebSocketStatus.DIE;
            }
            else if (script.dead && savedUserData.webSocketStatus == (int)UserData.WebSocketStatus.DIE)
            {
                savedUserData.webSocketStatus = (int)UserData.WebSocketStatus.DEAD;
                script.userData.webSocketStatus = (int)UserData.WebSocketStatus.DEAD;
            }
        }

        userGameObject.transform.position = new Vector3(
            userGameObject.transform.position.x,
            userGameObject.transform.position.y,
            defaultCharacterGameObject.transform.position.z
        );
        savedUserData.position = userGameObject.transform.position;
        savedUserData.rotation = userGameObject.transform.rotation;

        if (savedUserData.rotationStatus == (int)UserData.RotationStatus.RIGHT)
        {
            userGameObject.transform.rotation = Quaternion.Euler(
                0,
                (int)UserData.RotationStatus.RIGHT,
                0
            );
        }
        else if (savedUserData.rotationStatus == (int)UserData.RotationStatus.LEFT)
        {
            userGameObject.transform.rotation = Quaternion.Euler(
                0,
                (int)UserData.RotationStatus.LEFT,
                0
            );
        }
    }

    private IEnumerator WaitCreateGameObject(GameObject userGameObject, UserData savedUserData)
    {
        yield return new WaitForSeconds(0.5f);
        if (savedUserData.character == UserData.CharacterType.UnityChanCharacter.ToString())
        {
            UnityChanCharacterScript script = userGameObject.GetComponent<UnityChanCharacterScript>();
            script.DoOpenMessage();
        }
    }

    private IEnumerator WaitDestroyGameObject(GameObject userGameObject, UserData savedUserData)
    {
        if (savedUserData.character == UserData.CharacterType.UnityChanCharacter.ToString())
        {
            UnityChanCharacterScript script = userGameObject.GetComponent<UnityChanCharacterScript>();
            script.DoDeadMessage();
        }
        yield return new WaitForSeconds(5);
        Destroy(userGameObject);
    }
}
