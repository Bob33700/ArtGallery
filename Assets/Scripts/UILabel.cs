using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILabel : MonoBehaviour
{
	public static UILabel instance;
	public Image image;


	private void Awake() {
		instance = this;
	}


	//private void OnMouseEnter() {
	//	image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
	//}



	public void SetLabel(Material mat) {
		var tex = mat.mainTexture as Texture2D;
		var sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
		image.sprite = sprite;
	}

}
