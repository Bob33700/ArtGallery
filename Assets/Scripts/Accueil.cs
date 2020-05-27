using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Accueil : SelectableToile
{

	public override void Highlight(bool on) {
		base.Highlight(on);
	}

	//private void OnBecameVisible() {
	//	if (!UIManager.instance.startUI.gameObject.activeInHierarchy)
	//		flycam.Calibration();
	//}

}
