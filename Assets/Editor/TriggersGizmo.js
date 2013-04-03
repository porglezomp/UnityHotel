#pragma strict

static var objectOffset : float = 10;

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

@MenuItem ("GameObject/Create Trigger/Box #%1")
static function CreateBoxTrigger () {
	var triggerBox : GameObject  = new GameObject("BoxTrigger");
	triggerBox.transform.position = SceneView.lastActiveSceneView.pivot;
	triggerBox.AddComponent.<BoxCollider>();
	triggerBox.AddComponent.<TriggerArea>();
}

@MenuItem ("GameObject/Create Trigger/Sphere #%2")
static function CreateSphereTrigger () {
	var triggerSphere : GameObject  = new GameObject("SphereTrigger");
	triggerSphere.transform.position = SceneView.lastActiveSceneView.pivot;
	triggerSphere.AddComponent.<SphereCollider>();
	triggerSphere.AddComponent.<TriggerArea>();
}