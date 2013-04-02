#pragma strict

var clip : AudioClip;
var sources : AudioSourceGroup;

class AudioAction extends TriggerAction {

function Trigger (other : Collider) {
	if (sources) {
		sources.PlayOneShot(clip, 1);
	} else {
		AudioSource.PlayClipAtPoint(clip, transform.position);
	}
}

}