#pragma strict

var nodeName : String;
var res : Resolution;

private var directions : Vector3[] = [Vector3.forward, -Vector3.forward, Vector3.right, -Vector3.right, Vector3.up, -Vector3.up];
private var names : String[] = ["Forward", "Backward", "Right", "Left", "Up", "Down"];
private var frame = -1;
private var bakeComplete : boolean = false;

function Update () {
	if (frame >= 0 && frame < 6) {
		transform.rotation = Quaternion.LookRotation(directions[frame], Vector3.up);
		camera.Render();
		var screenshotFilename = nodeName + names[frame];
		print(screenshotFilename);
		Application.CaptureScreenshot(screenshotFilename);
		frame++;
	} else if (frame == 6) {
		bakeComplete = true;
	}
}

function Bake () {
	res = Screen.currentResolution;
	gameObject.AddComponent.<Camera>();
	camera.fieldOfView = 90;
	frame = 0;
}

function DoneBaking () : boolean {
	return bakeComplete;
}