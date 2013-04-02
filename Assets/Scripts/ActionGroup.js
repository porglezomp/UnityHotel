#pragma strict

var actions : TriggerAction[];

class ActionGroup extends TriggerAction {

function Trigger (other : Collider) {
	for (var action : TriggerAction in actions) {
		action.Trigger(other);
	}
}

}