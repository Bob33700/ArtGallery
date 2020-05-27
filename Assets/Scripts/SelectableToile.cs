using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableToile : Selectable
{

	[Header("Toile")]
	public GameObject toile;
	public GameObject etiquette;

	[Header("Audio")]
	public AudioClip audioClip;
	public float volume = 0.8f;

	[Header("mouvements de caméra")]
	public float slideDistance = .5f;
	public Transform pivot;
	public Transform orbital;
	public float rotationSpeed = .15f;
	public bool rotateAroundPivot = true;

	private SelectableGroup group;

	public override void Start() {
		base.Start();
		// groupe
		group = gameObject.GetComponentInParent<Artiste>()?.GetComponentInChildren<SelectableGroup>();
	}

	public override void Update() {
		base.Update();
		// si on est sur le tableau 
		if (CameraManager.instance.currentVcam == localCam) {
			if (rotateAroundPivot && !NavUI.instance.crosshair.enabled) {                    // si le mouvement de caméra est activé
				RotateAroundPivot();                    // bouger la caméra
			}

		}
	}

	public override void Select() {
		// préparation de la sortie
		if (group)
			exitPosition = group.exitPosition;
		base.Select();

		timer = 0;                                                          // réinitialiser le mouvement de caméra

		//UI 
		NavUI.instance.Show(false);
		GroupUI.instance.SetLabel(etiquette);                               // initialiser l'étiquette
		GroupUI.instance.ShowGroupUI(true);                                 // afficher l'UI

		// Audio
		AudioManager.instance.SetToile(audioClip, volume);                  // lancer la musique du tableau
	}

	public override void SetCamPosition() {
		// hauteur de la toile
		Bounds bounds = toile.GetComponent<Collider>().bounds;
		float h = bounds.size.y * parentScale.y;
		// pivot
		pivot.transform.localPosition = new Vector3(0f, 0f, -h);
		pivot.transform.LookAt(transform.position);
		// orbital
		orbital.localPosition = Vector3.zero;
		orbital.transform.LookAt(transform.position);
		// caméra (la distance est ajustée pour qu'un angle de vision de 30° couvre la toile en vertical)
		localCam.transform.LookAt(transform.position);
		angle = 30f;
	}

	float timer = 0f;
	float dx, dy, dz;
	/// <summary>
	/// mouvement lent de caméra autour du point pivot 
	/// (pour dynamiser la présentation)
	/// </summary>
	private void RotateAroundPivot() {
		timer += Time.deltaTime;
		dx = slideDistance * Mathf.Sin(timer / 1f * rotationSpeed);
		dy = slideDistance / 2f * Mathf.Cos(timer / 1f * rotationSpeed);
		dz = -slideDistance * (toile.transform.localScale.x / toile.transform.localScale.z) / 2f * Mathf.Cos(timer / 2f * rotationSpeed);
		orbital.transform.localPosition = new Vector3(dx, dy, dz);
		orbital.transform.LookAt(transform.position);
	}


}
