#pragma strict

var nodes : CubemapNode[];
var nodeIndex : int = 0;
var res : Resolution;

function Start () {
	Application.CaptureScreenshot("Cubemap/USELESS");
	Camera.main.gameObject.GetComponent.<Crosshair>().enabled = false;
	Camera.main.enabled = false;
	Screen.SetResolution (512, 512, false);
	nodes = FindObjectsOfType(CubemapNode) as CubemapNode[];
	nodes[nodeIndex].Bake();
	res = nodes[nodeIndex].res;
}

function Update () {
	if (nodeIndex >= 0) {
		if (nodes[nodeIndex].DoneBaking()) {
			nodeIndex++;
			if (nodeIndex >= nodes.length) {
				nodeIndex = -1;
			} else {
				nodes[nodeIndex].Bake();
			}
		}
	} else if (nodeIndex == -1) {
		Screen.SetResolution (res.width, res.height, false);
		print("done");
		GameObject.FindWithTag("MainCamera").camera.enabled = true;
		nodeIndex = -2;
	}
}