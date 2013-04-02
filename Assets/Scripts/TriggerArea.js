#pragma strict
@script RequireComponent(Collider);

var enterAction : TriggerAction;
var exitAction : TriggerAction;
var stayAction : TriggerAction;
var triggerOnce : boolean = true;
var onlyPlayer : boolean = true;
var triggerConstantly : boolean = false;
var color : Color = Color(.4, .5, 1, .4);

private var enterHasBeenTriggered : boolean;
private var exitHasBeenTriggered : boolean;

function Start () {
	if (!collider.isTrigger) {
		collider.isTrigger = true;
	}
}

function OnTriggerEnter (other : Collider) {
	if (enterAction) {
		if (!triggerOnce || !enterHasBeenTriggered) {
			if (onlyPlayer) {
				if (other.tag == "Player") {
					enterAction.Trigger(other);
				}
			} else {
				enterAction.Trigger(other);
			}
		}
		if (triggerOnce) {
			enterHasBeenTriggered = true;
		}
	}
}

function OnTriggerExit (other : Collider) {
	if (exitAction) {
		if (!triggerOnce || !exitHasBeenTriggered) {
			if (onlyPlayer) {
				if (other.tag == "Player") {
					exitAction.Trigger(other);
				}
			} else {
				exitAction.Trigger(other);
			}
		}
	}
	if (triggerOnce) {
		exitHasBeenTriggered = true;
	}
}

function OnTriggerStay (other : Collider) {
	if (stayAction) {
		if (triggerConstantly) {
			if (onlyPlayer) {
				if (other.tag == "Player") {
					stayAction.Trigger(other);
				}
			} else {
				stayAction.Trigger(other);
			}
		}
	}
}