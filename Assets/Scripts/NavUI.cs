using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavUI : MonoBehaviour
{

    public static NavUI instance;
    public Image crosshair;

    public bool showCrosshair { get; private set; } = true;

    private void Awake() {
        instance = this;
    }

    private void Update() {
        // afficher / masquer le viseur
        if (Input.GetKeyDown(KeyCode.Space)) {
            showCrosshair = !showCrosshair;
        }
    }

    public void Show(bool on) {
        crosshair.enabled = on && showCrosshair;
    }

}
