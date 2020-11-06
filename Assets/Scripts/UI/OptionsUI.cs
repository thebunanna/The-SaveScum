using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class OptionsUI : MonoBehaviour {
    public InputAction options;
    public Scrollbar music;
    public Scrollbar sfx;
    public AudioSource au;
    public AudioClip boss;
    public AudioClip normal;
    GameObject housing;
    public bool enable = false;
    void Start (){
        enable = false;
        options.Enable();
        housing = transform.GetChild(0).gameObject;
        housing.SetActive(false);

        au.ignoreListenerVolume = true;

        options.started += ctx => OnPause(ctx);      
        au.clip = normal;
        au.Play();  
    }
    void Update () {
        au.volume = music.value;
        AudioListener.volume = sfx.value;
    }

    public void OnPause(InputAction.CallbackContext context) {
        enable = !enable;
        housing.SetActive(enable);
        if (enable) Time.timeScale = 0;
        else Time.timeScale = 1;
    }

    public void NormalMusic () {
        if (au.clip == null || au.clip != normal) {
            au.Stop();
            au.clip = normal;
            au.Play();
        }
    }
    public void BossMusic() {
        if (au.clip != boss) {
            au.Stop();
            au.clip = boss;
            au.Play();
        }
    }
}