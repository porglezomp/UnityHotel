#pragma strict

private var activeObject : GameObject;
private var oldParent : Transform;
private var oldUp : Vector3;
private var oldForward : Vector3;

function Start () {
	activeObject = null;
}

function Update () {	
	
	if (activeObject) {
		if (Input.GetButtonDown("Use")) { //drop object
			activeObject.rigidbody.isKinematic = false;
			activeObject.transform.parent = oldParent;
			activeObject = null;
		}
	} 
	
	else if (Input.GetButtonDown("Use")) { //set object picked up
		var info = new RaycastHit();
		if(Physics.Raycast(transform.position, transform.forward, info, 3)) {
			if(info.collider.gameObject.GetComponent("Rigidbody") != null) {
				print("hit");
				activeObject = info.collider.gameObject;
				activeObject.rigidbody.isKinematic = true;
				oldUp = activeObject.transform.up;
				oldForward = activeObject.transform.forward;
				var pos = transform.position + transform.forward * 1.2;
				activeObject.transform.position = pos;
				oldParent = activeObject.transform.parent;
				activeObject.transform.parent = transform;
			}
		}
	}
	
	if(activeObject != null) { //move the object to your hand
		activeObject.transform.rotation = Quaternion.LookRotation(oldForward, oldUp);
	}
}

function FixedUpdate () {
	
}
