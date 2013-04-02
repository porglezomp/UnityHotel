#pragma strict

var message : String;
var callbacks : boolean;

class TriggerAction extends MonoBehaviour{

function Trigger (other : Collider) {
	if (callbacks) {
		Debug.Log(other.name + " " + message);
	}
}

}