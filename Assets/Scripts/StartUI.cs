using UnityEngine;

public class StartUI : MonoBehaviour
{
	public Transform cible;
	public Toile accueil;
	public float distance = 3f;


	private void Start() {
		gameObject.SetActive(true);
		Screen.fullScreen = true;
	}

	public void Go() {
		VisitorManager.started = true;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		gameObject.SetActive(false);
		VisitorManager.instance.navAgent.SetDestination(cible.position);
		VisitorManager.instance.GoTo(cible.position, new JoinToile(accueil), 0.1f, distance);
    }

	class JoinToile: VisitorManager.Action
	{
		Toile accueil;
		public JoinToile(Toile accueil) {
			this.accueil = accueil;
		}
		override public void Run() {
			accueil.OnMouseUp();
		}
	}
}
