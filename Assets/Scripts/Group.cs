using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.AI;

public class Group : MonoBehaviour
{

	public float camDistance = 200f;
	public CinemachineVirtualCamera groupCam { get; private set; }

	public List<CinemachineVirtualCamera> localCams { get; private set; }
	private Vector3 parentScale;
	private Quaternion rot;
	private Vector3 pos;
	private float angle;

	VisitorManager visitorManager;
	NavMeshAgent visitor;

	SimpleHelvetica nom;
	Material mat;
	private Color baseColor = Color.black;
	private Color highlightColor = Color.grey;


	private void Awake() {
		groupCam = GetComponentInChildren<CinemachineVirtualCamera>();

	}

	private void Start() {
		parentScale = transform.InverseTransformVector(transform.parent.localScale);

		// visiteur
		visitorManager = VisitorManager.instance;
		visitor = visitorManager.navAgent;

		// positionner la caméra de groupe
		if (groupCam)
			SetCamPosition();


		// désactiver toutes les caméras du groupe
		localCams = new List<CinemachineVirtualCamera>(GetComponentsInChildren<CinemachineVirtualCamera>(true));
		foreach (CinemachineVirtualCamera cam in localCams) {
			cam.gameObject.SetActive(false);
		}
		localCams.Remove(groupCam);

		nom = GetComponentInChildren<SimpleHelvetica>();
		mat = nom.GetComponent<MeshRenderer>().material;
		Tableaux tableaux = GetComponentInParent<Tableaux>();
		if (tableaux) {
			baseColor = tableaux.nameBaseColor;
			highlightColor = tableaux.nameHighlightColor;
		}

		enabled = false;
	}

	private void OnBecameVisible() {
		enabled = true;
	}

	private void OnBecameInvisible() {
		enabled = false;
	}

	private void Update() {
		if (CameraManager.instance.currentVcam != groupCam && Input.GetMouseButtonUp(0) && VisitorManager.instance.HighlightedArtist == this)
			Select();
	}

	public void Select() {
		GameObject activeCam = CameraManager.instance.cinemachineBrain.ActiveVirtualCamera.VirtualCameraGameObject;

		if (groupCam != null && !groupCam.gameObject.activeInHierarchy && !localCams.Contains(activeCam.GetComponent<CinemachineVirtualCamera>())) {
			SetActive(true);
		}
	}

	//public void OnMouseUp() {
	//	GameObject activeCam = CameraManager.instance.cinemachineBrain.ActiveVirtualCamera.VirtualCameraGameObject;

	//	if (groupCam != null && !groupCam.gameObject.activeInHierarchy && !localCams.Contains(activeCam.GetComponent<CinemachineVirtualCamera>())) {
	//		SetActive(true);
	//	}
	//}

	public void SetActive(bool on) {
		if (on) {
			groupCam.transform.rotation = rot;
			groupCam.transform.position = pos;
			groupCam.m_Lens.FieldOfView = angle;
			CameraManager.instance.RestoreFocalDistance(groupCam);  // restaurer facteur de zoom

			visitor.SetDestination(groupCam.transform.position);
			//StartCoroutine(IsetCamHeight());

			CameraManager.instance.currentVcam.gameObject.SetActive(false);
			groupCam.gameObject.SetActive(true);

			GroupUI.instance.ShowLabelUI(false);
			GroupUI.instance.ShowBackUI(true);

		} else {

		}
	}

	private void SetCamPosition() {

		Collider[] colliders = transform.parent.GetComponentsInChildren<Collider>();
		Bounds bounds = new Bounds(transform.position, Vector3.zero);
		foreach (Collider nextCollider in colliders) {
			bounds.Encapsulate(nextCollider.bounds);
		}
		var y = (bounds.center.y - transform.parent.position.y) * parentScale.y;
		var vertical = Mathf.Abs(Mathf.Atan(bounds.size.y * parentScale.y / 2f / camDistance) * 2f * Mathf.Rad2Deg);
		var horizontal = Mathf.Abs(Mathf.Atan(bounds.size.z * parentScale.z / 2f / camDistance) * Mathf.Rad2Deg);

		groupCam.transform.localPosition = new Vector3(0f, y, -camDistance);

		rot = new Quaternion();//groupCam.transform.rotation;
		pos = groupCam.transform.position;
		angle = Mathf.Max(vertical, horizontal) * 1.3f;

	}

	public void Highlight(bool on) {
		if (mat) {
			if (CameraManager.instance.currentVcam != groupCam && NavUI.instance.showCrosshair) {
				if (on && mat.GetColor("_Color") != highlightColor) {
					mat.SetColor("_Color", highlightColor);
					nom.ApplyMeshRenderer();
					VisitorManager.instance.HighlightedArtist = this;
					//mat.SetColor("_EmissionColor", highlightColor);
				} else if (!on && mat.GetColor("_Color") != baseColor) {
					mat.SetColor("_Color", baseColor);
					nom.ApplyMeshRenderer();
					VisitorManager.instance.HighlightedArtist = null;
					//mat.SetColor("_EmissionColor", Color.black);
				}
			}
		}
	}

}
