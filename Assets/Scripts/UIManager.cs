using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public static UIManager instance;
    public QuitUI quitUi;                       // interface Quit
    public GroupUI groupUI;                     // interface Groupe et toiles
    public StartUI startUI;                     // interface Start
    public NavUI navUI;                         // interface navigation

    void Awake() {
        instance = this;
    }


    public void ShowQuitUi(bool on) {
        quitUi.Show(on);
    }
}
