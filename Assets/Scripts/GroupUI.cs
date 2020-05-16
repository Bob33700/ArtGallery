using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupUI : MonoBehaviour
{
	public static GroupUI instance;
	public Image label;
	public Image back;

	public Sprite backArrow;
	public Sprite rightClic;

	private void Awake() {
		instance = this;
	}

	private void Start() {
		ShowGroupUI(false);
		// adapter l'icone à la plateforme (mobile ou standalone)
		back.sprite = Application.platform == RuntimePlatform.Android ? backArrow : rightClic;
	}

	public void ShowGroupUI(bool on) {
		label.gameObject.SetActive(on);
		back.gameObject.SetActive(on);
	}

	public void ShowLabelUI(bool on) {
		label.gameObject.SetActive(on);
	}
	public void ShowBackUI(bool on) {
		back.gameObject.SetActive(on);
	}

	public void SetLabel(GameObject etiquette) {
		var mat = etiquette.GetComponent<MeshRenderer>().material;
		var tex = mat.mainTexture as Texture2D;
		if (tex != null) {
			var sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
			label.sprite = sprite;
		} else {
			label.sprite = null;
		}
	}

	public void GoBack() {
		VisitorManager.instance.ActivateCam();
		AudioManager.instance.SetAmbiance();
	}
}
