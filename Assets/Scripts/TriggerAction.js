#pragma strict

var message : String;
var callbacks : boolean;

function Start () {

}

function Update () {

}

function Trigger (other : Collider) {
	if (callbacks) {
		Debug.Log(other.name + " " + message);
	}
}