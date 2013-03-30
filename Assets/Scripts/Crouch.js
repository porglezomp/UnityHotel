#pragma strict

private var scale : float = 1;
private var targetHeight : float = .5;
function Start () {

}

function Update () {
	if (Input.GetAxis("Crouch")) {
		scale = Mathf.Lerp(scale, targetHeight, .4);
	} else if (scale != 1) {
		scale = Mathf.Lerp(scale, 1, .4);
		transform.Translate(0, .1, 0);
	}
	if (scale > .95) {
		scale = 1;
	} else if (scale < targetHeight + .02) {
		scale = targetHeight;
	}
	transform.localScale.y = scale;
}