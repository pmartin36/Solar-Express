using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour {

	AudioSource audio;
	Coroutine volumeAdjustment;

	public float Volume {
		get {
			return audio.volume;
		}
		set {
			audio.volume = value;
		}
	}

	public bool Mute {
		get {
			return audio.mute;
		}
		set {
			audio.mute = value;
		}
	}

	public AudioClip PlayingSong {
		get { return audio.clip; }
	}

	// Use this for initialization
	public MusicManager Init () {
		audio = GetComponent<AudioSource>();
		audio.volume = 0.1f;
		audio.loop = true;
		Mute = GameManager.Instance.PlayerInfo.SoundOn;
		return this;
	}

	public void SetPlayingSong( AudioClip song ) {
		audio.Stop();
		audio.clip = song;
		audio.Play();
	}
	
	public void SetVolumeLevelGradual (float end, float time) {
		if( Volume == end ) return;

		volumeAdjustment = StartCoroutine(SetVolumeGradual(end, time));
	}

	private IEnumerator SetVolumeGradual(float end, float time) {
		float start = Volume;
		float startTime = Time.time;

		while(Time.time - startTime < time + 0.1f) {
			Volume = Mathf.Lerp(start, end, (Time.time - startTime) / time );
			yield return new WaitForSeconds(0.1f);
		}
	}
}
