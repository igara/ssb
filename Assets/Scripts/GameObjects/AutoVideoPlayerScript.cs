using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoVideoPlayerScript : MonoBehaviour
{
#if UNITY_WEBGL
    // Start is called before the first frame update
    void Start()
    {
        var videoPlayer =
            GetComponentInChildren<UnityEngine.Video.VideoPlayer>();
        videoPlayer.url =
            System
                .IO
                .Path
                .Combine(Application.streamingAssetsPath, "Title.mp4");
        videoPlayer.Play();
    }

    // Update is called once per frame
    void Update()
    {
    }
#endif
}
