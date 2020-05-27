using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Selectable : MonoBehaviour
{

	public Renderer highlightable;
	public Color colorOn = Color.white;
	public Color colorOff = Color.black;
	public Color emissionOn = Color.grey;
	public Color emissionOff = Color.black;

	public CinemachineVirtualCamera localCam { get; private set; }
	public float camDistance = 200f;
	public Material mat { get; private set; }

	[HideInInspector]
	public float angle;

	public Vector3 parentScale { get; private set; }
	[HideInInspector]
	public Vector3 exitPosition;

	private void Awake() {
		localCam = GetComponentInChildren<CinemachineVirtualCamera>();
	}

	virtual public void Start() {
		mat = highlightable.material;
		parentScale = transform.InverseTransformVector(transform.parent.localScale);

		// positionner la caméra locale
		localCam.gameObject.SetActive(false);
		localCam.transform.localScale = Vector3.one;
		if (localCam)
			SetCamPosition();

		// préparation de la sortie
		exitPosition = localCam.transform.position;
	}

	virtual public void Update() {
		if (CameraManager.instance.currentVcam != localCam && Input.GetMouseButtonUp(0) && VisitorManager.instance.highlighted == this)
			Select();
	}

	private void OnBecameVisible() {
		enabled = true;
	}

	private void OnBecameInvisible() {
		if (CameraManager.instance.currentVcam != localCam)
			enabled = false;
	}

	public virtual void Select() {
		localCam.transform.rotation = new Quaternion();                                     // orientation par défaut
		localCam.m_Lens.FieldOfView = angle;                                                // réinitialiser le zoom

		CameraManager.instance.currentVcam.gameObject.SetActive(false);                     // désactiver la caméra courante
		localCam.gameObject.SetActive(true);                                                // activer la caméra locale
		VisitorManager.instance.navAgent.SetDestination(exitPosition);                      // déplacer le visiteur

		Highlight(false);
	}

	public abstract void SetCamPosition();

	public virtual void Highlight(bool on) {
		if (CameraManager.instance.currentVcam != localCam && NavUI.instance.showCrosshair) {
			VisitorManager.instance.target = this;
			if (on && VisitorManager.instance.highlighted != this) {
				mat.SetColor("_Color", colorOn);
				mat.SetColor("_EmissionColor", emissionOn);
				VisitorManager.instance.highlighted = this;
			} else {
				if (!on && VisitorManager.instance.highlighted == this) {
					mat.SetColor("_Color", colorOff);
					mat.SetColor("_EmissionColor", emissionOff);
					VisitorManager.instance.highlighted = null;
				}

			}
		}
		if (CameraManager.instance.currentVcam == localCam && on) {
			NavUI.instance.Show(false);
		}
	}


}
