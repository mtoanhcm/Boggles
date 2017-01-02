using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

public class DetectEnterKey : MonoBehaviour {

	private Button _myButton;

	void Awake(){
		_myButton = GetComponent<Button> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Return)) {
			if (_myButton.interactable) {
				GameplayController.instance.ClickCheckWordChoosen ();
			}
		}
	}
}
