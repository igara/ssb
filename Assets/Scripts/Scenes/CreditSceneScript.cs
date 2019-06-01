using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditSceneScript : MonoBehaviour
{
    [SerializeField]
    UnityEngine.UI.Button closeButton;

    // Start is called before the first frame update
    void Start()
    {
        closeButton.onClick.AddListener(OnClickCloseButton);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnClickCloseButton()
    {
        SceneManager.LoadScene("Scenes/MenuScene");
    }
}
