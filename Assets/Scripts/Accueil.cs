using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Accueil : Toile
{

	private FlyCam flycam;
	private Renderer renderer;

	// Start is called before the first frame update
	public override void Start() {
		base.Start();
		// flycam
		flycam = CameraManager.instance.GetComponent<FlyCam>();
		renderer = toile.GetComponent<Renderer>();
	}

	public override void Highlight(bool on) {
		base.Highlight(on);
	}

	private void OnBecameVisible() {
		if (!UIManager.instance.startUI.gameObject.activeInHierarchy)
			flycam.Calibration();
	}

}
