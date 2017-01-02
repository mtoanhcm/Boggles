using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

public class SpinItem : MonoBehaviour {

	[SerializeField] private AudioClip _stopSpinClip;
	[SerializeField] private AudioSource _audioSource;

	private List<Text> _listItems;
	private Transform _startPos;
	private Transform _endPos;
	private bool _isSpinFirstTime;
	private int _numOfRound;

	void Awake(){
		Initialization ();
	}

	void Initialization(){
		_startPos = transform.FindChild ("Start");
		_endPos = transform.FindChild ("End");
		_isSpinFirstTime = false;
		_listItems = new List<Text> ();
		_numOfRound = 2;
	}

	IEnumerator SpinT(float _timer){
		if (!_isSpinFirstTime) {
			_isSpinFirstTime = true;
			yield return new WaitForSeconds (0.04f);
		}
		for (int i=0; i < _listItems.Count; i++) {
			var _word = _listItems [i];
			_word.transform.DOMoveY (_endPos.position.y, _timer).OnComplete (() => CompleteWord (_listItems[i-1]));
			yield return new WaitForSeconds (_timer/2);
		}

		_numOfRound--;
		if (_numOfRound <= 0) {
			_audioSource.Stop ();
			_audioSource.PlayOneShot (_stopSpinClip);
			foreach (Text _item in _listItems) {
				Destroy (_item.gameObject);
			}
			yield return new WaitForSeconds (0.1f);
			GameplayController.instance.CreateCharPlayable (transform, DiceId());
			yield break;
		} else {
			StartCoroutine (SpinT (_timer));
		}

		_audioSource.Play ();
	}

	void CompleteWord(Text _word){
		var _temp = _word.transform.position;
		_temp.y =_startPos.position.y;
		_word.transform.position = _temp;
	}

	int DiceId(){
		string _myName = gameObject.name;
		return int.Parse (_myName);
	}

	public void RunSpin(float _timer){
		StartCoroutine (SpinT (_timer));
	}

	public void SetListItem(List<Text> _list){
		_listItems.AddRange (_list);

		foreach (Text _item in _listItems) {
			_item.transform.SetParent (transform);
		}
	}

	public Transform GetStartPos(){
		return _startPos;
	}

	public void ClearListItem(){
		_listItems.Clear ();
	}
}