#pragma strict

@DrawGizmo (GizmoType.NotSelected | GizmoType.Pickable)
static function DrawTriggerArea(trigger : TriggerArea, gizmoType : GizmoType) {
	Gizmos.matrix = trigger.transform.localToWorldMatrix;
	Gizmos.color = trigger.color;
	Gizmos.DrawCube (Vector3.zero, Vector3.one);
}