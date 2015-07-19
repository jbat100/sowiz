#pragma strict

public class SceneManager extends MonoBehaviour {

	var layerControllers = Array;
	

	public function Start () {
		this.layerControllers = new Array();
	}

	public function Update () {

	}
	
	public function ProcessMessage (message : LayerControlMessage) {
		// get all objects from the corresponding uninty layer 
	}
	
	public function RegisterLayerController (controller : LayerController) {
	
	}

}
