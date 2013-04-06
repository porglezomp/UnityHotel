#pragma strict

class CubemapEditor extends EditorWindow {

var useBaker : boolean;
var baker : CubemapBaker;
var bakerObject : GameObject;
private var lastState : boolean;

@MenuItem ("Window/Cubemap Editor")
static function ShowWindow () {
	EditorWindow.GetWindow (CubemapEditor);
}

@DrawGizmo (GizmoType.NotSelected | GizmoType.Selected | GizmoType.Pickable)
static function DrawReflective (node : CubemapNode, gizmoType : GizmoType) {
        Gizmos.DrawIcon(node.transform.position, "CubemapGizmo");
}

@MenuItem ("GameObject/Create Other/CubeMap Bake #%3")
static function AddPoint () {
	var node : GameObject  = new GameObject("cubeMapNode");
	node.transform.position = SceneView.lastActiveSceneView.pivot;
	node.AddComponent.<CubemapNode>();
}

function OnGUI() {
	useBaker = EditorGUILayout.Toggle ("Bake Lightprobes", useBaker);
	if (useBaker && !lastState) {
		if (bakerObject == null) {
			bakerObject = new GameObject("__CubemapBaker__");
			baker = bakerObject.AddComponent.<CubemapBaker>();
		}
	} else if (!useBaker && lastState) {
		DestroyImmediate(bakerObject);
	}
	lastState = useBaker;
}

}