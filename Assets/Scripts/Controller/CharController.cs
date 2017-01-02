using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharController : MonoBehaviour, IPointerClickHandler {

	public void OnPointerClick(PointerEventData eventData){
		if (transform.childCount > 2) {
			Text _char = transform.GetChild (transform.childCount - 1).GetComponent<Text> ();
			GameplayController.instance.ClickOnCharacter (_char);
		}
	}

}
