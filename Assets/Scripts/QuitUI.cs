using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitUI : MonoBehaviour
{


	private void Start() {
		Show(false);
	}

	public void YesButton() {
		Application.Quit();
	}

	public void NoButton() {
		Show(false);
	}

	public void Show(bool on) {
		gameObject.SetActive(on);
		if (on) {
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		} else {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}

}
