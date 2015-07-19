#pragma strict

public class LayerControlMessage {
	
	var layer : int;
	var path : String;
	var args : Array;
	
	function LayerControlMessage(l:int, p: String, a:Array) {
		this.layer = l;
		this.path = p;
		this.args = a;
	}
}

public class LayerControlMessageHandler {
	public function ProcessMessage (message : LayerControlMessage, gameObject : GameObject) {
		// do something described by the layer control message to the game object 
	}
}

public class LayerController {
	
	var index : int;
	var controlMessageHandlers : Array = new Array();



	public function ProcessMessage (message : LayerControlMessage) {
		
	}
}

