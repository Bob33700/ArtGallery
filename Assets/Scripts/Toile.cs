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


	private bool isOnPicture;
	RaycastHit[] m_RaycastHitCache = new RaycastHit[16];            // cache des résultats de lancer de rayon
	int hitCount = 0;
	int picLayer;                                                   // layer des tableaux à tester pour le Raycast

	[Header("mouvements de caméra")]
	public Transform pivot;
	public Transform orbital;
	public bool rotateAroundPivot = true;
	private float slideDistance;
	public float rotationSpeed = .15f;
	private Vector3 parentScale;
	private Group group;


	Material mat;

	private void Awake() {
	}

	private void Start() {
		picLayer = 1 << LayerMask.NameToLayer("Pictures");                          // layer des tableaux (pour gérer le pointeur de souris)

		// caméra du tableau
		canvasCam = toile.GetComponentInChildren<CinemachineVirtualCamera>(true);   // identifier la caméra
		canvasCam.gameObject.SetActive(false);                                      // désactiver la caméra
		canvasCam.transform.localScale = Vector3.one;
		SetCamPosition();                                                           // préparer la gestion du mouvement de caméra

		// groupe
		group = GetComponentInParent<Artiste>().GetComponentInChildren<Group>();

		mat = toile.GetComponent<MeshRenderer>().material;
		if (oeuvre) {
			mat = Instantiate(Resources.Load<Material>("Toile"));
			mat.SetTexture("_MainTex", oeuvre);
			mat.SetTexture("_EmissionMap", oeuvre);
			mat.SetColor("_Color", Color.white);
			mat.SetColor("_EmissionColor", Color.black);
			toile.GetComponent<MeshRenderer>().material = mat;
		}
		//if (label) {
		//	mat = Instantiate(Resources.Load<Material>("Toile"));
		//	mat.SetTexture("_MainTex", label);
		//	mat.SetTexture("_EmissionMap", label);
		//	mat.SetColor("_Color", Color.white);
		//	mat.SetColor("_EmissionColor", Color.black);
		//	etiquette.GetComponent<MeshRenderer>().material = mat;
		//}
	}


	private void Update() {
		// si on est sur le tableau 
		if (CameraManager.instance.currentVcam == canvasCam) {

			if (rotateAroundPivot) {                    // si le mouvement de caméra est activé
				RotateAroundPivot();                    // bouger la caméra
			}

		}


		if (CameraManager.instance.currentVcam == VisitorManager.instance.visitorCam) {

		}

	}

	private void OnMouseDown() {
		//if (canvasCam != null && !canvasCam.gameObject.activeInHierarchy) {
		//	CameraManager.instance.CenterCursor();                              // centrer le curseur (pour éviter de décentrer l'affichage
		//}
	}

	// Gestion du clic sur le tableau ou son étiquette
	public void OnMouseUp() {
		// si on n'est pas déjà sur le tableau
		if (canvasCam != null && !canvasCam.gameObject.activeInHierarchy) {
			CameraManager.instance.currentVcam.gameObject.SetActive(false);     // désactiver la caméra courante
			timer = 0;                                                          // réinitialiser le mouvement de caméra
			canvasCam.m_Lens.FieldOfView = angle;                               // réinitialiser le zoom
			canvasCam.gameObject.SetActive(true);                               // activer la caméra du tableau
			canvasCam.transform.rotation = new Quaternion();

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


	public void Highlight(bool on) {
		//mat.EnableKeyword("_EmissionEnabled");
		if (CameraManager.instance.currentVcam != canvasCam && NavUI.instance.showCrosshair && on) {
			mat.SetColor("_Color", Color.white);
			mat.SetColor("_EmissionColor", highlightColor);
		} else {
			mat.SetColor("_Color", Color.white);
			mat.SetColor("_EmissionColor", Color.black);

		}
		if (CameraManager.instance.currentVcam == canvasCam && on) {
			NavUI.instance.Show(false);
		}
	}
}
