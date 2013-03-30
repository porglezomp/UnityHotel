#pragma strict
var size : int = 5;

function OnGUI () {
	GUI.Label(new Rect(Screen.width/2-size, Screen.height/2-size, size*2, size*2), "+");
}