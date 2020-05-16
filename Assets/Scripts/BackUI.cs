using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackUI : MonoBehaviour
{
    void Start()
    {
        
    }

    public void GoBack() {
        VisitorManager.instance.ActivateCam();
        AudioManager.instance.SetAmbiance();
    }
}
