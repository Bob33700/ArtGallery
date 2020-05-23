using UnityEngine;
using System.Collections;
using Cinemachine;
using UnityEngine.AI;
using UnityEngine.UI;

public class FlyCam : MonoBehaviour
{

	/*
    wasd : basic movement
    shift : Makes camera accelerate
	*/

	public static FlyCam instance;

	public NavMeshAgent visitor;
	VisitorManager visitorManager;

	float mainSpeed = 1f;           // regular speed
	float shiftAdd = 5.0f;          // multiplied by how long shift is held.  Basically running
	float maxShift = 100.0f;        // Maximum speed when holdin gshift
	private float totalRun = 1.0f;

	public float speedSens = 50f;     //How sensitive it with mouse
	public Image senitivity;
	public Sprite s_plus;
	public Sprite s_minus;

	Vector3 gesture = Vector3.zero;

	float dx, dy;

	Vector3 prevPos;

	private CinemachineVirtualCamera currentCam {
		get {
			if (CameraManager.instance != null)
				return CameraManager.instance.currentVcam;
			else
				return null;
		}
	}

	private void Awake() {
		instance = this;
	}

	private void Start() {
		visitorManager = VisitorManager.instance;

		prevPos = new Vector2(Screen.width / 2f, Screen.height / 2f);
	}



	Vector3 p;
	void Update() {

		if (visitor != null && currentCam != null && VisitorManager.started) {

			//----------------------
			// rotation
			gesture = DetectGesture();
			if (gesture != Vector3.zero) {
				p = new Vector3(gesture.x, 0, gesture.y);
				if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
					totalRun += Time.deltaTime;
					p = p * totalRun * shiftAdd;
					p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
					p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
					p.z = Mathf.Clamp(p.z, -maxShift, maxShift);
				} else {
					totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
					p *= mainSpeed;
				}

				p *= Time.deltaTime;

				Vector3 rot;
				if (visitorManager.visitorCam == currentCam) {
					visitor.transform.Rotate(new Vector3(0, p.x, 0));
					rot = new Vector3(currentCam.transform.eulerAngles.x - p.z, currentCam.transform.eulerAngles.y, 0);
					if (rot.x > 30 && rot.x < 330) rot.x += p.z;
					currentCam.transform.eulerAngles = rot;
				} else {
					rot = new Vector3(currentCam.transform.eulerAngles.x - p.z, currentCam.transform.eulerAngles.y + p.x, 0);
					if (rot.x > 30 && rot.x < 330) rot.x += p.z;
					currentCam.transform.eulerAngles = rot;
				}
			}

			//-------------------------
			// sensibilité
			if (Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.Plus)) {
				StartCoroutine(ISensitivity(+1));
			} else if (Input.GetKeyDown(KeyCode.KeypadMinus) || Input.GetKeyDown(KeyCode.Minus)) {
				StartCoroutine(ISensitivity(-1));
			}
		}
	}

	IEnumerator ISensitivity(int k) {
		if (k < 0) {
			speedSens /= 1.2f;
			senitivity.sprite = s_minus;
		} else {
			speedSens *= 1.2f;
			senitivity.sprite = s_plus;
		}
		senitivity.enabled = true;
		yield return new WaitForSeconds(0.5f);
		senitivity.enabled = false;
	}


	private Vector2 DetectGesture() {
		Vector2 mvt = Vector3.zero;

		// mobile => si plus d'1 point, ne rien faire
		if (Input.touchCount > 1) return mvt;

		// souris ou swioe su écran
		dx = Input.GetAxis("Mouse X") * speedSens;
		dy = Input.GetAxis("Mouse Y") * speedSens;

		mvt = new Vector2(dx, dy);

		prevPos = Input.mousePosition;

		return mvt;
	}


	public void Calibration() {
		StartCoroutine(ICalibration());
	}
	IEnumerator ICalibration() {
		if (speedSens < 0) {
			speedSens = 0;
			var dt = 0f;
			int i = -1;
			while (i++ < 50) {
				dt += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
			Debug.Log(dt);
			speedSens = 20 / dt;
		}
	}
}
