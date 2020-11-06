using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ExitButton : MonoBehaviour
{
    private Button button;
    // Start is called before the first frame update
    void Start()
    {
        button = this.GetComponent<Button>();
        button.onClick.AddListener(()=>Click());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Click() {
        Application.Quit();
    }
}
