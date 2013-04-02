#pragma strict

var sources : AudioSource[];

function Play () {
	for (var source : AudioSource in sources) {
		source.Play();
	}
}

function PlayOneShot (clip : AudioClip, volume : float) {
	for (var source : AudioSource in sources) {
		source.PlayOneShot(clip, volume);
	}
}