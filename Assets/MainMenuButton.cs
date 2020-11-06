using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuButton : MonoBehaviour
{
    private Button button;
    public int sceneNumber;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("hi there");
        button = this.GetComponent<Button>();
        button.onClick.AddListener(()=>Click());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Click () {
        Debug.Log("hi there");
        SceneManager.LoadScene(sceneNumber);
    }
    public void OnMouseDown() {
        Click();
    }
}
