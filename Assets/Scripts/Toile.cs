using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Toile : MonoBehaviour
{
	[Header("Toile")]
	public GameObject toile;
	public Texture oeuvre;
	public Color highlightColor = Color.grey;
	[Header("Etiquette")]
	public GameObject etiquette;
	public Texture label;
	[Header("Audio")]
	public AudioClip audioClip;
	public float volume = 0.8f;

	private CinemachineVirtualCamera canvasCam;
	private float angle;

	[Header("mouvements de caméra")]
	public Transform pivot;
	public Transform orbital;
	public bool rotateAroundPivot = true;
	private float slideDistance;
	public float rotationSpeed = .15f;
	private Vector3 parentScale;
	private Group group;


	public Material mat { get; private set; }

	private void Awake() {
	}

	public virtual void Start() {

		// caméra du tableau
		canvasCam = toile.GetComponentInChildren<CinemachineVirtualCamera>(true);   // identifier la caméra
		canvasCam.gameObject.SetActive(false);                                      // désactiver la caméra
		canvasCam.transform.localScale = Vector3.one;
		SetCamPosition();                                                           // préparer la gestion du mouvement de caméra

		// groupe
		group = GetComponentInParent<Artiste>()?.GetComponentInChildren<Group>();

		mat = toile.GetComponent<MeshRenderer>().material;
		if (oeuvre) {
			mat = Instantiate(Resources.Load<Material>("Toile"));
			mat.SetTexture("_MainTex", oeuvre);
			mat.SetTexture("_EmissionMap", oeuvre);
			mat.SetColor("_Color", Color.white);
			mat.SetColor("_EmissionColor", Color.black);
			toile.GetComponent<MeshRenderer>().material = mat;
		}

		enabled = false;
	}

	private void OnBecameVisible() {
		enabled = true;
	}

	private void OnBecameInvisible() {
		if (CameraManager.instance.currentVcam != canvasCam)
			enabled = false;
	}

	private void Update() {
		// si on est sur le tableau 
		if (CameraManager.instance.currentVcam == canvasCam) {
			if (rotateAroundPivot && !NavUI.instance.crosshair.enabled) {                    // si le mouvement de caméra est activé
				RotateAroundPivot();                    // bouger la caméra
			}

		} else {
			if (Input.GetMouseButtonUp(0) && VisitorManager.instance.HighlightedToile == this)
				Select();
		}
	}

	public void Select() {
		// si on n'est pas déjà sur le tableau
		if (canvasCam != null && !canvasCam.gameObject.activeInHierarchy) {
			CameraManager.instance.currentVcam.gameObject.SetActive(false);     // désactiver la caméra courante
			timer = 0;                                                          // réinitialiser le mouvement de caméra
			canvasCam.m_Lens.FieldOfView = angle;                               // réinitialiser le zoom
			canvasCam.gameObject.SetActive(true);                               // activer la caméra du tableau
			canvasCam.transform.rotation = new Quaternion();
			Highlight(false);
			NavUI.instance.Show(false);

			//UI 
			GroupUI.instance.SetLabel(etiquette);                               // initialiser l'étiquette
			GroupUI.instance.ShowGroupUI(true);                                 // afficher l'UI

			// Audio
			AudioManager.instance.SetToile(audioClip, volume);                  // lancer la musique du tableau

			if (group && group.groupCam) {
				VisitorManager.instance.navAgent.SetDestination(group.groupCam.transform.position);  // rapprocher le visiteur (pour préparer la sortie)
			}
		}
	}

	private void SetCamPosition() {
		parentScale = transform.InverseTransformVector(transform.parent.localScale);    // facteur d'échelle

		// hauteur de la toile
		Bounds bounds = toile.GetComponent<Collider>().bounds;
		float h = bounds.size.y * parentScale.y;
		slideDistance = .5f;
		// pivot
		pivot.transform.localPosition = new Vector3(0f, 0f, -h);
		pivot.transform.LookAt(transform.position);
		// orbital
		orbital.localPosition = Vector3.zero;
		orbital.transform.LookAt(transform.position);
		// caméra (la distance est ajustée pour qu'un angle de vision de 30° couvre la toile en vertical)
		canvasCam.transform.LookAt(transform.position);
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


	public virtual void Highlight(bool on) {
		//mat.EnableKeyword("_EmissionEnabled");
		if (CameraManager.instance.currentVcam != canvasCam && NavUI.instance.showCrosshair && on) {
			if (VisitorManager.instance.HighlightedToile != this) {
				mat.SetColor("_Color", Color.white);
				mat.SetColor("_EmissionColor", highlightColor);
				VisitorManager.instance.HighlightedToile = this;
			}
		} else {
			mat.SetColor("_Color", Color.white);
			mat.SetColor("_EmissionColor", Color.black);
			VisitorManager.instance.HighlightedToile = null;
		}
		if (CameraManager.instance.currentVcam == canvasCam && on) {
			NavUI.instance.Show(false);
		}
	}
}
