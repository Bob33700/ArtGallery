using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavUI : MonoBehaviour
{

    public static NavUI instance;
    public Image crosshair;
    Text text;

    public bool showCrosshair { get; private set; } = true;

    private void Awake() {
        instance = this;
        text = GetComponentInChildren<Text>();
    }

    private void Update() {
        // afficher / masquer le viseur
        if (Input.GetKeyDown(KeyCode.Space)) {
            showCrosshair = !showCrosshair;
            Show(showCrosshair);
        }
        if (text)
            text.text = CameraManager.instance.GetComponent<FlyCam>().speedSens.ToString();
    }

    public void Show(bool on) {
        crosshair.enabled = on && showCrosshair;
    }

}
