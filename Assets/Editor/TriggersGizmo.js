#pragma strict

@DrawGizmo (GizmoType.NotSelected | GizmoType.Pickable)
static function DrawTriggerArea(trigger : TriggerArea, gizmoType : GizmoType) {
	Gizmos.color = Color (0.4, 0.5, 1, 0.1);
	Gizmos.DrawCube (trigger.transform.position, trigger.collider.bounds.size);
}