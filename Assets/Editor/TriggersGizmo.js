#pragma strict

@DrawGizmo (GizmoType.NotSelected | GizmoType.Pickable)
static function DrawTriggerArea(trigger : TriggerArea, gizmoType : GizmoType) {
	Gizmos.color = trigger.color;
	if (trigger.collider.GetType() == BoxCollider) {
		Gizmos.matrix = trigger.transform.localToWorldMatrix;
		Gizmos.DrawCube (Vector3.zero, Vector3.one);
	}
	if (trigger.collider.GetType() == SphereCollider) {
		var radius : float = trigger.GetComponent.<SphereCollider>().radius * 
				Mathf.Max(trigger.transform.localScale.x, trigger.transform.localScale.y,
				trigger.transform.localScale.z);
		
		Gizmos.DrawSphere(trigger.transform.position, radius);
	}
}

@MenuItem ("GameObject/Create Trigger/Box")
static function CreateBoxTrigger () {
	
}

@MenuItem ("GameObject/Create Trigger/Sphere")
static function CreateSphereTrigger () {
	
}