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
	public Vector3 parentScale;
	private Quaternion rot;
	private Vector3 pos;
	private float angle;

	VisitorManager visitorManager;
	NavMeshAgent visitor;


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

		//CreateNameCollider();
	}


	public void OnMouseUp() {
		GameObject activeCam = CameraManager.instance.cinemachineBrain.ActiveVirtualCamera.VirtualCameraGameObject;

		if (groupCam != null && !groupCam.gameObject.activeInHierarchy && !localCams.Contains(activeCam.GetComponent<CinemachineVirtualCamera>())) {
			SetActive(true);
		}
	}

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

			//StartCoroutine(CenterCursor());

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
		var vertical = Mathf.Abs(Mathf.Atan(bounds.size.y * parentScale.y / 2f / camDistance) * 2f * Mathf.Rad2Deg );
		var horizontal = Mathf.Abs(Mathf.Atan(bounds.size.z * parentScale.z / 2f / camDistance) * Mathf.Rad2Deg);

		groupCam.transform.localPosition = new Vector3(0f, y, -camDistance);

		rot = new Quaternion();//groupCam.transform.rotation;
		pos = groupCam.transform.position;
		angle = Mathf.Max(vertical, horizontal) * 1.3f;

	}

	private void CreateNameCollider() {
		SimpleHelvetica name = GetComponentInChildren<SimpleHelvetica>();
		if (name) {
			BoxCollider bc = name.gameObject.AddComponent<BoxCollider>();
			bc.center = name.transform.position;
			bc.size = Vector3.zero;
			Bounds bounds = new Bounds() { center = name.transform.position, size = Vector3.zero };
			foreach (Collider c in name.GetComponentsInChildren<Collider>(false)) {
				bounds.Encapsulate(c.bounds);
			}
			bc.size = new Vector3(bounds.size.x * parentScale.x, bounds.size.y * parentScale.y, bounds.size.z * parentScale.z);
			bc.center = new Vector3(Mathf.Abs(bc.size.x / 2f), -4f, 0);
		}
	}
}
