using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Multiptyque : MonoBehaviour
{

	public float camDistance = 200f;
	public Transform etiquette;
	public AudioClip audioClip;
	public float volume = 0.8f;

	public CinemachineVirtualCamera groupCam;
	public List<CinemachineVirtualCamera> localCams { get; private set; }
	private Quaternion rot;
	private Vector3 pos;
	private float angle;

	VisitorManager visitorManager;
	NavMeshAgent visitor;


	private void Start() {
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
	}

	public void OnMouseUp() {
		GameObject activeCam = CameraManager.instance.cinemachineBrain.ActiveVirtualCamera.VirtualCameraGameObject;

		if (groupCam != null && !groupCam.gameObject.activeInHierarchy && !localCams.Contains(activeCam.GetComponent<CinemachineVirtualCamera>())) {
			groupCam.transform.rotation = rot;
			groupCam.transform.position = pos;
			groupCam.m_Lens.FieldOfView = angle;
			CameraManager.instance.RestoreFocalDistance(groupCam);  // restaurer facteur de zoom

			visitor.SetDestination(groupCam.transform.position);

			CameraManager.instance.currentVcam.gameObject.SetActive(false);
			groupCam.gameObject.SetActive(true);

			GroupUI.instance.ShowLabelUI(false);
			GroupUI.instance.ShowBackUI(true);

			//UI
			GroupUI.instance.SetLabel(etiquette.gameObject);
			GroupUI.instance.ShowGroupUI(true);

			// Audio
			AudioManager.instance.SetToile(audioClip, volume);
		}
	}


	private void SetCamPosition() {
		Vector3 parentScale = transform.InverseTransformVector(transform.parent.localScale);

		Collider[] colliders = GetComponentsInChildren<Collider>();
		Bounds bounds = new Bounds(transform.position, Vector3.zero);
		foreach (Collider nextCollider in colliders) {
			bounds.Encapsulate(nextCollider.bounds);
		}
		float h = bounds.size.y * parentScale.y;
		//var y = (bounds.center.y - transform.parent.position.y) * parentScale.y;
		//var y = 0f;
		//var pos = GetComponentInParent<Multiptyque>().transform.position;
		//pos = new Vector3(pos.x, pos.y, -200);
		//groupCam.transform.localPosition = new Vector3(0f, y, -camDistance);

		rot = groupCam.transform.rotation;
		pos = groupCam.transform.position;
		angle = Mathf.Atan(h / 2f / camDistance) * 2f * Mathf.Rad2Deg * 1.1f;

	}

	public void Highlight(bool on) {
		foreach(Toile toile in GetComponentsInChildren<Toile>()) {
			toile.Highlight(on);
		}
	}

}
