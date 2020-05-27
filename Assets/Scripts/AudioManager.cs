using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
	public static AudioManager instance;

	public AudioMixer audioMixer;
	public float fadeDelay = .5f;

	public GameObject tableaux;

	[Header("Sources")]
	public AudioSource[] sources;

	private AudioMixerSnapshot[] snapshots = new AudioMixerSnapshot[3];
	int currentSnapshot = 0;

	float[][] presets;


	private void Awake() {
		instance = this;
	}


	private void Start() {
		snapshots[0] = audioMixer.FindSnapshot("Ambiance");
		snapshots[1] = audioMixer.FindSnapshot("Toile1");
		snapshots[2] = audioMixer.FindSnapshot("Toile2");

		presets = new float[3][];
		presets[0] = new float[3] { 1, 0, 0 };
		presets[1] = new float[3] { 0, 1, 0 };
		presets[2] = new float[3] { 0, 0, 1 };
	}

	// précharger les audioclips en arrière plan
	private void PreloadAudio() {
		SelectableToile[] toiles = tableaux.GetComponentsInChildren<SelectableToile>();
		List<AudioClip> clips = new List<AudioClip>();
		foreach (SelectableToile toile in toiles) {
			clips.Add(toile.audioClip);
			toile.audioClip.LoadAudioData();
		}
	}

	// passer progressivement en musique d'ambiance
	public void SetAmbiance() {
		StartCoroutine(ISetAmbiance());
	}

	IEnumerator ISetAmbiance() {
		audioMixer.TransitionToSnapshots(snapshots, presets[0], fadeDelay);
		currentSnapshot = 0;
		yield return new WaitForSeconds(fadeDelay);
		sources[1].Stop();
		sources[2].Stop();
	}

	// passer progressivement en musique spécifique à une toile
	public void SetToile(AudioClip audioClip, float volume) {
		if (sources[currentSnapshot].clip != audioClip)
			StartCoroutine(ISetToile(audioClip, volume));
	}

	IEnumerator ISetToile(AudioClip audioClip, float volume) {
		var oldSource = sources[currentSnapshot];
		currentSnapshot = currentSnapshot == 1 ? 2 : 1;
		sources[currentSnapshot].clip = audioClip;
		sources[currentSnapshot].volume = volume;
		sources[currentSnapshot].Play();
		audioMixer.TransitionToSnapshots(snapshots, presets[currentSnapshot], fadeDelay);
		yield return new WaitForSeconds(fadeDelay);
		if (oldSource != sources[0])
			oldSource.Stop();
	}

}
