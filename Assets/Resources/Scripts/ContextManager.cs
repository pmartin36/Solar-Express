using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextManager : MonoBehaviour {

	private List<AudioSource> audioSources;

	// Use this for initialization
	public virtual void Awake(){
		audioSources = new List<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void AddAudioSource(AudioSource s) {
		audioSources.Add(s);
	}

	public void RemoveAudioSource(AudioSource s) {
		audioSources.Remove(s);
	}

	public void MuteAudioSources() {
		foreach (AudioSource s in audioSources) {
			s.mute = true;
		}
	}

	public void UnmuteAudioSources() {
		foreach (AudioSource s in audioSources) {
			s.mute = false;
		}
	}
}
