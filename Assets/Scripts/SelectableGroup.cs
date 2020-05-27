using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableGroup : Selectable
{
	SimpleHelvetica nom;

	public override void Start() {
		base.Start();
		nom = highlightable.GetComponent<SimpleHelvetica>();
	}

	public override void SetCamPosition() {
		Collider[] colliders = transform.parent.GetComponentsInChildren<Collider>();
		Bounds bounds = new Bounds(transform.position, Vector3.zero);
		foreach (Collider nextCollider in colliders) {
			bounds.Encapsulate(nextCollider.bounds);
		}
		var y = (bounds.center.y - transform.parent.position.y) * parentScale.y;
		localCam.transform.localPosition = new Vector3(0f, y, -camDistance);

		var vertical = Mathf.Abs(Mathf.Atan(bounds.size.y * parentScale.y / 2f / camDistance) * 2f * Mathf.Rad2Deg);
		var horizontal = Mathf.Abs(Mathf.Atan(bounds.size.z * parentScale.z / 2f / camDistance) * Mathf.Rad2Deg);
		angle = Mathf.Max(vertical, horizontal) * 1.3f;

	}

	public override void Select() {
		base.Select();

		GroupUI.instance.ShowLabelUI(false);      // masquer l'étiquette
		GroupUI.instance.ShowBackUI(true);        // afficher l'icône 'clic droit' 

	}

	public override void Highlight(bool on) {
		base.Highlight(on);
		if (nom)
			nom.ApplyMeshRenderer();
	}

}
