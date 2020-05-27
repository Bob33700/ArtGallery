using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableMulti : Selectable
{
	public Transform etiquette;
	public AudioClip audioClip;
	public float volume = 0.8f;

	public override void Start() {
		base.Start();
	}

	public override void Update() {
		base.Update();
	}

	public override void Select() {
		base.Select();
		VisitorManager.instance.navAgent.SetDestination(localCam.transform.position);

		//UI
		GroupUI.instance.SetLabel(etiquette.gameObject);
		GroupUI.instance.ShowGroupUI(true);

		// Audio
		AudioManager.instance.SetToile(audioClip, volume);
	}

	public override void SetCamPosition() {
		Collider[] colliders = GetComponentsInChildren<Collider>();
		Bounds bounds = new Bounds(transform.position, Vector3.zero);
		foreach (Collider nextCollider in colliders) {
			bounds.Encapsulate(nextCollider.bounds);
		}
		float h = bounds.size.y * parentScale.y;

		angle = Mathf.Atan(h / 2f / camDistance) * 2f * Mathf.Rad2Deg * 1.1f;
	}

	public override void Highlight(bool on) {
		//base.Highlight(on);
		foreach (SelectableToile toile in GetComponentsInChildren<SelectableToile>()) {
			if (!on) VisitorManager.instance.highlighted = toile;
			toile.Highlight(on);
		}
		VisitorManager.instance.highlighted = null;
		if (on)
			VisitorManager.instance.highlighted = this;
		else
			VisitorManager.instance.highlighted = null;
	}

}
