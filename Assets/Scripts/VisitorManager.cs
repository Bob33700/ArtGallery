using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using Cinemachine;
using System.IO;
using System.Security.AccessControl;

public class VisitorManager : MonoBehaviour
{
	public static VisitorManager instance;
	public static bool started = false;


	UIManager uiManager;                                            // gestionnaire d'interface utilisateur

	// navigation
	public NavMeshAgent navAgent { get; private set; }
	private NavUI navUI;
	public CinemachineVirtualCamera visitorCam { get; private set; }
	private float normalSpeed;
	private float shiftSpeedFactor = 2f;

	// souris
	bool isClicOnUI;                                                // le clic en cours a-t-il débuté sur un élément d'interface ?
	bool isClicOnPicture;                                           // le clic en cours a-t-il débuté sur un tableau ?


	// raycast
	int picLayer;                                                   // layer des tableaux à tester pour le Raycast
	public RaycastHit[] m_RaycastHitCache { get; private set; } = new RaycastHit[16];            // cache des résultats de lancer de rayon
	public int hitCount { get; private set; } = 0;
	public int lastHitCount { get; private set; } = 0;

	public GameObject tableaux;
	Selectable[] selectables;

	[HideInInspector]
	public Selectable highlighted;
	[HideInInspector]
	public Selectable target;



	private void Awake() {
		instance = this;
		navAgent = GetComponent<NavMeshAgent>();
	}

	void Start() {
		uiManager = UIManager.instance;                                     // gestionnaire d'interface utilisateur
		navUI = NavUI.instance;

		normalSpeed = navAgent.speed;                                       // vitesse du visiteur

		visitorCam = GetComponentInChildren<CinemachineVirtualCamera>();    // identifier la caméra du visiteur

		picLayer = 1 << LayerMask.NameToLayer("Pictures");                  // layer des tableaux

		selectables = tableaux.GetComponentsInChildren<Selectable>();
	}


	void Update() {
		// quitter le jeu par la touche escape
		if (Input.GetKeyDown(KeyCode.Escape)) {
			uiManager.ShowQuitUi(true);
		}

		// Lancer de rayon de la caméra vers le pointeur de souris
		Ray screenRay = CameraManager.instance.gameplayCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
		hitCount = Physics.SphereCastNonAlloc(screenRay, .01f, m_RaycastHitCache, 1000.0f, picLayer);
		//Debug.DrawLine(CameraManager.instance.gameplayCamera.transform.position, CameraManager.instance.currentVcam.transform.position + CameraManager.instance.currentVcam.transform.forward);

		// Le pointeur est-il sur une toile ou un groupe?
		//navUI.Show(true);

		if (hitCount != lastHitCount) {
			if (hitCount > 0) {
				for (int i = 0; i < hitCount; i++) {
					Rigidbody rigidbody = m_RaycastHitCache[i].collider.attachedRigidbody;
					if (rigidbody) {
						Selectable selectable = rigidbody.GetComponentInChildren<Selectable>();
						if (selectable && selectable.enabled) {
							selectable.Highlight(true);
							navUI.crosshair.color = Color.yellow;
							break;
						} else {
							Debug.Log("unknown");
							navUI.Show(true);
						}


						//Group artiste = rigidbody.GetComponentInChildren<Group>();
						//if (artiste && artiste.enabled) {
						//	artiste.Highlight(true);
						//	navUI.crosshair.color = Color.yellow;
						//	break;
						//} else {
						//	Multiptyque multi = rigidbody.GetComponent<Multiptyque>();              // sur un tableau multiple
						//	if (multi && multi.enabled) {
						//		multi.Highlight(true);
						//		navUI.crosshair.color = Color.yellow;
						//		break;
						//	} else {
						//		Toile toile = rigidbody.GetComponentInChildren<Toile>();
						//		if (toile && toile.enabled) {                                                // sur une toile
						//			toile.Highlight(true);
						//			navUI.crosshair.color = Color.yellow;
						//			break;
						//		} else {
						//			Debug.Log("unknown");
						//			navUI.Show(true);
						//		}
						//	}
						//}
					} else {
						Debug.Log("no rigidbody");
						navUI.Show(true);
					}
				}
			} else {
				navUI.crosshair.color = Color.white;
				highlighted?.Highlight(false);
				//foreach (Selectable selectable in selectables) {
				//	selectable.Highlight(false);
				//}
				navUI.Show(true);

			}
			lastHitCount = hitCount;
		}



		// vitesse de déplacement
		navAgent.speed = normalSpeed * ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) ? shiftSpeedFactor : 1f);

		// Gestion du clic droit pour quitter un groupe ou un tableau
		if (Input.GetMouseButtonDown(1)) {
			ActivateCam();                              // activer la caméra du visiteur
		}

		//if (Application.platform != RuntimePlatform.Android) {
		// déplacements au clavier
		if (CameraManager.instance.currentVcam == visitorCam) {
			if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow)) {
				MoveForward();
			}
			if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
				MoveBackward();
			}
			if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow)) {
				MoveLeft();
			}
			if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
				MoveRight();
			}
		}

		//} else {
		//	// déplacements avec 2 doigts
		//	if (Input.touchCount == 2 && Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved) {
		//		Vector2 move = (Input.GetTouch(0).deltaPosition + Input.GetTouch(1).deltaPosition) / 2f;
		//		if (Mathf.Abs(move.x) > Mathf.Abs(move.y)) { // mouvemnt horizontal => déplacement latéral
		//			if (move.x < 0) {
		//				MoveLeft();
		//			} else {
		//				MoveRight();
		//			}
		//		} else {                                    // mouvemnt vertical => déplacement avant/arrière
		//			if (move.y < 0) {
		//				MoveForward();
		//			} else {
		//				MoveBackward();
		//			}
		//		}
		//	}
		//}
	}

	private void MoveForward() {
		NavMeshPath path = new NavMeshPath();
		navAgent.CalculatePath(transform.position + transform.forward * 1f, path);
		if (path.corners.Length <= 4)
			navAgent.SetDestination(transform.position + transform.forward * 1f);
	}
	private void MoveBackward() {
		NavMeshPath path = new NavMeshPath();
		navAgent.CalculatePath(transform.position + transform.forward * -.5f, path);
		if (path.corners.Length <= 4)
			navAgent.updateRotation = false;
		GoTo(transform.position + transform.forward * -.5f, new ReleaseRotation(navAgent), 0.6f, 0.01f);
	}
	private void MoveLeft() {
		NavMeshPath path = new NavMeshPath();
		navAgent.CalculatePath(transform.position + transform.right * -.5f, path);
		if (path.corners.Length <= 4)
			navAgent.updateRotation = false;
		GoTo(transform.position + transform.right * -.5f, new ReleaseRotation(navAgent), 0.6f, 0.01f);
	}
	private void MoveRight() {
		NavMeshPath path = new NavMeshPath();
		navAgent.CalculatePath(transform.position + transform.right * .5f, path);
		if (path.corners.Length <= 4)
			navAgent.updateRotation = false;
		GoTo(transform.position + transform.right * .5f, new ReleaseRotation(navAgent), 0.1f, 0.01f);
	}

	class ReleaseRotation : Action
	{
		NavMeshAgent agent;
		public ReleaseRotation(NavMeshAgent agent) {
			this.agent = agent;
		}
		override public void Run() {
			agent.updateRotation = true;
		}

	}

	/// <summary>
	/// Activer la caméra du visiteur
	/// </summary>
	public void ActivateCam() {
		//CameraManager.instance.CenterCursor();
		CameraManager.instance.RestoreFocalDistance(visitorCam);        // restaurer le facteur de zoom de la caméra du visiteur
		CameraManager.instance.currentVcam.gameObject.SetActive(false); // désactiver la caméra courante
		visitorCam.gameObject.SetActive(true);                          // activer la vaméra du visiteur
		GroupUI.instance.ShowGroupUI(false);                            // masquer l'UI de groupe & tableau
																		//Cursor.visible = true;                                          // afficher le pointeur
		AudioManager.instance.SetAmbiance();                            // mettre la musique d'ambiance
	}

	/// <summary>
	/// Aller à un point donné
	/// A l'approche, déclencher une action
	/// </summary>
	/// <param name="destination">le point à atteindre</param>
	/// <param name="callback">l'action à déclencehr</param>
	/// <param name="distance">la distance de déclenchement</param>
	public void GoTo(Vector3 destination, Action callback = null, float delay = 0f, float distance = 0.5f) {
		navAgent.SetDestination(destination);
		if (callback != null)
			StartCoroutine(CallBack(destination, callback, delay, distance));
	}
	/// <summary>
	/// Attendre que le visiteur soit arrivé à proximité de sa destination pour déclencher une action
	/// </summary>
	/// <param name="destination">le point à atteindre</param>
	/// <param name="callback">l'action à déclencehr</param>
	/// <param name="distance">la distance de déclenchement</param>
	/// <returns></returns>
	IEnumerator CallBack(Vector3 destination, Action callback, float delay, float distance) {
		//while (Vector3.Distance(navAgent.transform.position, destination) > distance) {
		//while (Vector3.Distance(navAgent.transform.position, destination) > distance || navAgent.remainingDistance > distance) {
		while (Vector3.Distance(navAgent.transform.position, destination) > distance || navAgent.velocity.magnitude > .01f) {
			yield return new WaitForSeconds(0.1f);
		}
		yield return new WaitForSeconds(delay);
		callback.Run();
	}

	/// <summary>
	/// Action à déclencher : surcharger la méthode Run
	/// </summary>
	public class Action
	{
		public virtual void Run() { }
	}
}
