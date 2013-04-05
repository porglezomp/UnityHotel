#pragma strict

private var nodes : CubemapNode[];
private var nodeIndex : int = 0;
private var res : Resolution;

function Start () {
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
			nodeIndex ++;
		}
	} else if (nodeIndex == -1) {
		Screen.SetResolution (res.width, res.height, false);
		print("done");
		GameObject.FindWithTag("MainCamera").camera.enabled = true;
		nodeIndex = -2;
	}
}