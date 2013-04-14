#pragma strict
var clip : AudioClip;
var threshhold : float;

function Start () {

}

function Update () {

}

function OnCollisionEnter(collision : Collision){
	if (collision.relativeVelocity.magnitude > threshhold){
		AudioSource.PlayClipAtPoint(clip, transform.position);
	}
}