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
    }

    public void OnMouseExitBackRawImage()
    {
        backRawImage.SetActive(true);
    }

    public void OnMouseDownBackRawImage()
    {
        SceneManager.LoadScene("Scenes/MenuScene");
    }
}
