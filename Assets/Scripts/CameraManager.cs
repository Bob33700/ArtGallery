using Cinemachine;
using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
	static public CameraManager instance;
	public CinemachineBrain cinemachineBrain { get; private set; }
	public Camera gameplayCamera;

	public CinemachineVirtualCamera currentVcam => GetCurrentVcam();

	/// <summary>
	/// Angle in degree (down compared to horizon) the camera will look at when at the closest of the character
	/// </summary>
	public float MinAngle = 15.0f;
	/// <summary>
	/// Angle in degree (down compared to horizon) the camera will look at when at the farthest of the character
	/// </summary>
	public float MaxAngle = 45.0f;

	[HideInInspector]
	public float m_CurrentDistance = 1.0f;

	void Awake() {
		instance = this;
		cinemachineBrain = GetComponent<CinemachineBrain>();
	}

	private CinemachineVirtualCamera GetCurrentVcam() {
		if (cinemachineBrain.ActiveVirtualCamera != null)
			return cinemachineBrain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
		else
			return null;
	}

	private void Update() {
		float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
		if (!Mathf.Approximately(mouseWheel, 0.0f)) {
			Zoom(-mouseWheel * Time.deltaTime * 10.0f);
		}
	}

	/// <summary>
	/// Zoom of the given distance. Note that distance need to be a param between 0...1,a d the distance is a ratio
	/// </summary>
	/// <param name="distance">The distance to zoom, need to be in range [0..1] (will be clamped) </param>
	public void Zoom(float distance) {
		m_CurrentDistance = Mathf.Clamp01(m_CurrentDistance + distance);
		cinemachineBrain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = MinAngle + (MaxAngle - MinAngle) * m_CurrentDistance;
	}

	public void RestoreFocalDistance(CinemachineVirtualCamera vCam) {
		m_CurrentDistance = (vCam.m_Lens.FieldOfView - MinAngle) / (MaxAngle - MinAngle);
	}
}