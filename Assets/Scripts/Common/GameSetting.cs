using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetting
{
    // public static string webSoketServerUri = "ws://localhost:9000";
    public static string webSoketServerUri = "wss://syonet.work/ssb/";
    public static UserData selfUserData = new UserData();
    public static UserData[] cpuUserDatas = new UserData[1] {
        new UserData()
    };
}
