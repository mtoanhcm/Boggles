using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Assets.Helpers;
using System.Linq;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameplayController : MonoBehaviour {

	public static GameplayController instance;

	//Use for Word Board
	[SerializeField] private Transform _myCanvas;
	[SerializeField] private GameObject _textErrorWordContains;
	[SerializeField] private Transform _wordListBoard;
	[SerializeField] private Text _wordOnListBoard;
	[SerializeField] private Text _textTimer;
	[SerializeField] private TextAsset _wordDic;
	[SerializeField] private TextAsset _wordDefinition;
	[SerializeField] private TextAsset _dices;
	[SerializeField] private Text[] _textAnswers;
	[SerializeField] private Transform[] _penguinList;
	[SerializeField] private Transform _definitionBoard;
	[SerializeField] private Transform _wordPannel;
	[SerializeField] private Transform _definitionPannel;
	[SerializeField] private GameObject _resultPannel;
	[SerializeField] private Text _textScore;
	[SerializeField] private TextMesh _textScorePlus;
	[SerializeField] private AudioClip _winClip;
	[SerializeField] private AudioClip _wrongClip;
	[SerializeField] private AudioClip _rightClip;
	[SerializeField] private Text _textBoardDefinitionTitle;
	[SerializeField] private Button _buttonSubmit;
	[SerializeField] private Transform[] _penguinWithText;
	[SerializeField] private Text _characterOfPenguin;
	[SerializeField] private GameObject _startGamePannel;
	[SerializeField] private Text _wordNeedToDefine;
	[SerializeField] private Text _typeOfWord;
	[SerializeField] private GameObject _stopSpinEffect;
	[SerializeField] private Transform _sorryBear;
	[SerializeField] private Text _scoreTextOfEndPannel;
	[SerializeField] private GameObject _clock;
	public AudioSource audioSource;

	private DictionaryHelper _dictionary;
	private DictionaryHelper _mainDictionaryKey;
	private List<Text> _listWordDefine;
	private List<Text> _listCharClick;
	private List<string> _listStringCharPlayble;
	private List<Vector3> _penguinPos;
	private List<Transform> _listDice;
	private List<bool> _isDoneAllSpin;
	private Dictionary<string,string> _mainDictionary;
	private Text _wordChoose;
	private int _numWords;
	private int _wordPosX;
	private int _wordPosY;
	private string _answers;
	private bool _escapeCoroutine;
	private bool _isEndTime;
	private int _score;
	private float _sorryBearOldPos;

	private

	void MakeInstance(){
		if (instance == null) {
			instance = this;
		}
	}

	// Use this for initialization
	void Awake(){
		MakeInstance ();
		_dictionary = new DictionaryHelper ("EnDic");
		_mainDictionaryKey = new DictionaryHelper ("words");
		_mainDictionary = new Dictionary<string, string> ();
		_listDice = new List<Transform> ();
		_listWordDefine = new List<Text> ();
		_listStringCharPlayble = new List<string> ();
		_listCharClick = new List<Text> ();
		_isDoneAllSpin = new List<bool> ();
		_score = 0;
		_sorryBearOldPos = _sorryBear.position.x;
	}

	void Start () {
		CreateDictionary ();
		CreateListDice ();
	}

	void CreateDictionary(){
		List<string> _tempWordDic = new List<string> (_wordDic.text.Split('\n'));
		List<string> _tempWordDefi = new List<string> (_wordDefinition.text.Split ('\n'));

		for (int i = 0; i < _tempWordDic.Count; i++) {
			_tempWordDic [i] = _tempWordDic [i].Trim ();
		}

		for (int i = 0; i < _tempWordDic.Count; i++) {
			string _tempDic = _tempWordDic [i];
			string _tempDicDefi = _tempWordDefi [i];
			_mainDictionary.Add (_tempDic, _tempDicDefi);

		}

		//Debug.Log (_mainDictionary [a]);

	}

	#region Choose word area

	void CreateListDice(){
		for (int i = 0; i < 2; i++) {
			foreach (Transform _child in _penguinWithText[i]) {
				_listDice.Add (_child);
			}
		}

		CreateCharactersForEachDice ();
		RunSpinEffect ();
	}

	void CreateCharactersForEachDice(){
		var _listCharactersOfDice = new List<string> (_dices.text.Split ('\n'));
		List<string> _vowelList = new List<string> (){ "u", "e", "o", "a", "i" };
		bool _isHaveListSpin = false;
		int _vowelsNum = 0;

		for (int i = 0; i < _listCharactersOfDice.Count; i++) {
			_listCharactersOfDice [i] = _listCharactersOfDice [i].Trim ();
		}

		while (_vowelsNum < 8) {

			if (_listDice.Any (x => x.GetChild(x.childCount-1).tag == "Spin Char")) {
				_isHaveListSpin = true;
			}


			for (int i = 0; i < _listCharactersOfDice.Count; i++) {
				string _tempString = _listCharactersOfDice [i];
				char[] _listChar = _tempString.ToCharArray ();
				var _diceScript = _listDice [i].GetComponent<SpinItem> ();
				var _startPos = _diceScript.GetStartPos ();
				List<Text> _tempListChar = new List<Text> ();
					
				if (!_isHaveListSpin) {
					foreach (char _char in _listChar) {
						Text _charObject = Instantiate (_characterOfPenguin, Vector3.zero, Quaternion.identity) as Text;
						_charObject.transform.SetParent (_listDice [i], false);
						_charObject.transform.position = _startPos.position;
						_charObject.text = _char.ToString ();

						_tempListChar.Add (_charObject);

					}
					_diceScript.SetListItem (_tempListChar);
				}
					

					CreateListCharPlayable (_tempString);
				
			}

			foreach (string _char in _listStringCharPlayble) {
				if (_vowelList.Contains (_char.ToLower ())) {
					_vowelsNum++;
				}
			}

			if (_vowelsNum < 8) {
				_vowelsNum = 0;
				_listStringCharPlayble.Clear ();
			}
		}

	}

	void RunSpinEffect(){
		foreach (Transform _dice in _listDice) {
			float _random = Random.Range (0.2f, 0.6f);
			var _tempScript = _dice.GetComponent<SpinItem> ();
			_tempScript.RunSpin (_random);
		}
	}
		
	void CreateListCharPlayable(string _listCharactersOfDice){
		char[] _listChar = _listCharactersOfDice.ToCharArray ();
		int _random = Random.Range (0, _listChar.Length);
		string _randomChar = _listChar [_random].ToString ();

		_listStringCharPlayble.Add (_randomChar);

	}

	/// <summary>
	/// Creates the list char playable.
	/// Use when dice complete its spin
	/// </summary>
	/// <param name="_listCharactersOfDice">List characters of dice.</param>
	public void CreateCharPlayable(Transform _myItemListInDice,int _diceID){
		bool _isDoneSpin = true;

		foreach (Transform _item in _myItemListInDice) {
			if (_item.tag == "Spin Char") {
				_isDoneSpin = false;
			}
		}

		if (_isDoneSpin) {
			Text _textChoosen = Instantiate (_characterOfPenguin, Vector3.zero, Quaternion.identity) as Text;
			_textChoosen.transform.SetParent (_myItemListInDice,false);
			_textChoosen.transform.position = _myItemListInDice.position;
			_textChoosen.text = _listStringCharPlayble [_diceID-1];

			_isDoneAllSpin.Add (_isDoneSpin);

			var _effect = Instantiate (_stopSpinEffect, Vector3.zero, Quaternion.identity) as GameObject;
			_effect.transform.position = _textChoosen.transform.position;
			Destroy (_effect, 2f);

			if (_isDoneAllSpin.Count > 15) {
				_startGamePannel.SetActive (false);
				audioSource.PlayOneShot (_rightClip);
				_clock.SetActive (true);

				////Change when change level
				StartCoroutine (CountdownClock (3, 0));
			}
		}
	}

	/// <summary>
	/// Clicks the submit word button
	/// </summary>
	public void ClickCheckWordChoosen(){
		string _tempWordNeedToDefine = _wordNeedToDefine.text.ToLower();
		bool _isContain = false;

		if (_listWordDefine.Count > 0) {
			foreach (Text _word in _listWordDefine) {
				if (_word.text.ToLower () == _tempWordNeedToDefine) {
					_isContain = true;
				}
			}
		}

		if (_dictionary.CheckText (_tempWordNeedToDefine)) {
			if (_listWordDefine.Count > 0) {
				if (!_isContain) {
					AddWordToListBoard (_tempWordNeedToDefine);
				} else {
					_wordNeedToDefine.color = Color.red;
					audioSource.PlayOneShot (_wrongClip);
					_textErrorWordContains.SetActive (true);
					_textErrorWordContains.transform.DOScaleZ (0.9f, 2f).OnComplete (() => _textErrorWordContains.SetActive (false));
					_textErrorWordContains.transform.localScale = Vector3.one;
				}
			} else {
				AddWordToListBoard (_tempWordNeedToDefine);
			}
		} else {
			audioSource.PlayOneShot (_wrongClip);
			ShowSorryBear ();
		}

	}

	void ShowSorryBear(){
		float _oldPos = _sorryBearOldPos;
		float _lastPos = _oldPos + 7.5f;

		if (_sorryBear.position.x != _lastPos) {
			_sorryBear.DOMoveX (7.5f, 1f).OnComplete (() => StartCoroutine (HideSorryBear (_oldPos)));
		}
	}

	IEnumerator HideSorryBear(float _oldPos){
		yield return new WaitForSeconds (2f);
		_sorryBear.DOMoveX (_oldPos, 1f);
	}

	void AddWordToListBoard(string _myWord){
		audioSource.PlayOneShot (_rightClip);
		Text _myText = Instantiate (_wordOnListBoard, Vector3.zero, Quaternion.identity) as Text;
		_myText.text = _myWord;
		_myText.color = Color.white;
		_myText.transform.SetParent (_wordListBoard, false);
		_listWordDefine.Add (_myText);
		_wordNeedToDefine.text = "";

		foreach (Text _char in _listCharClick) {
			Color _myColor = _char.color;
			_myColor.a = 1f;
			_char.color = _myColor;

			_char.transform.parent.GetComponent<Image> ().raycastTarget = true;
		}

		_listCharClick.Clear ();
		CheckButtonSubmit ("");

		if (_listWordDefine.Count > 10) {
			_score++;
		} else if (_listWordDefine.Count == 10) {
			_score += 5;
		}

		_textScore.text = "" + _score;
	}

	/// <summary>
	/// Clicks the on character.
	/// </summary>
	/// <param name="_charClick">Char click.</param>
	public void ClickOnCharacter(Text _charClick){
		_wordNeedToDefine.text += _charClick.text;
		string _tempWord = _wordNeedToDefine.text;
		_charClick.transform.parent.GetComponent<Image> ().raycastTarget = false;

		Color _myColor = _charClick.color;
		_myColor.a = 0.3f;
		_charClick.color = _myColor; 

		_listCharClick.Add (_charClick);

		CheckButtonSubmit (_tempWord);

		if (_wordNeedToDefine.color == Color.red) {
			_wordNeedToDefine.color = Color.blue;
		}
	}

	public void ClickDeleteChar(){
		_wordNeedToDefine.text = "";

		foreach (Text _char in _listCharClick) {
			Color _myColor = _char.color;
			_myColor.a = 1f;
			_char.color = _myColor;
			_char.transform.parent.GetComponent<Image> ().raycastTarget = true;
		}

		_listCharClick.Clear ();
		CheckButtonSubmit (_wordNeedToDefine.text);
	}

	void CheckButtonSubmit(string _word){
		int _numOfCheck = 3;
		if (_word.Length > _numOfCheck) {
			_buttonSubmit.interactable = true;
		} else {
			_buttonSubmit.interactable = false;
		}

	}

	void ChangeSector(){
		foreach (Text _word in _listWordDefine) {
			_word.color = Color.white;
		}

		_buttonSubmit.gameObject.SetActive (false);
		_escapeCoroutine = true;
		StartCoroutine (AutoChangeWord (0.5f,true));
		_wordPannel.DOMoveY (13f,2f);
		_definitionPannel.DOMoveY (0f,2f).OnComplete(()=>RunWhenCompleteMovePannel());
	}

	/// <summary>
	/// Runs the when complete move pannel.
	/// </summary>
	void RunWhenCompleteMovePannel(){
		GetPenguinPos ();
		_wordPannel.gameObject.SetActive (false);
		_escapeCoroutine = false;

		foreach (Text _word in _listWordDefine) {
			_word.raycastTarget = true;
		}
		_isEndTime = false;
	}

	/// <summary>
	/// Countdowns the clock.
	/// </summary>
	/// <returns>The clock.</returns>
	/// <param name="_minute">Minute.</param>
	/// <param name="_second">Second.</param>
	IEnumerator CountdownClock(int _minute, int _second){
		if (_escapeCoroutine) {
			yield break;
		}
		yield return new WaitForSeconds (1f);
		if (_second <= 0) {
			if (_minute <= 0) {
				_minute = 0;
				_second = 0;
				yield break;
			} else {
				_minute--;
				_second = 60;
			}
		}

		_second--;
		_textTimer.text = _minute + ":" + _second;

		if (_minute <= 0 && _second <= 0) {
			if (!_wordPannel.gameObject.activeSelf) {//end time at part 2

				FillRedAllWordHaveNotDefine ();
				MoveWrongPenguins ("all");

				foreach (Transform _penguin in _penguinList) {
					_penguin.gameObject.SetActive (false);
				}
			} else {//end time at part 1
				if (_listWordDefine.Count > 0) {

					//Use func to remove word not in 3k words database
					CheckWordDoNotHaveDefineInDatabase ();

					yield return new WaitForSeconds (0.5f);

					ChangeSector ();
				} else {
					CheckResult ();
				}
			}

			_isEndTime = true;
			yield break;
		}

		StartCoroutine (CountdownClock (_minute, _second));
	}

	/// <summary>
	/// Checks the word do not have define in database.
	/// Need update database later
	/// </summary>
	void CheckWordDoNotHaveDefineInDatabase(){
		List<Text> _temp = new List<Text>();

		foreach(Text _word in _listWordDefine){
			if (!_mainDictionary.ContainsKey (_word.text)) {Debug.Log (_word.text);
				_word.color = Color.green;
				_word.raycastTarget = false;
				_temp.Add (_word);

				_score++;
			}
		}

		_textScore.text = "" + _score;

		foreach (Text _word in _temp) {
			_listWordDefine.Remove (_word);
		}
	}

	#endregion

	#region word definition area

	/// <summary>
	/// Checks the word was answered.
	/// </summary>
	/// <returns><c>true</c>, if word was answered was checked, <c>false</c> otherwise.</returns>
	/// <param name="_word">Word.</param>
	bool CheckWordWasAnswered(Text _word){
		if (_word.color != Color.white) {
			return true;
		} else {
			return false;
		}
	}

	/// <summary>
	/// Sets the word need to dedinition.
	/// Use when click on word in board
	/// </summary>
	/// <param name="_word">Word.</param>
	public void SetWordNeedToDedinition(Text _word){
		string _tempWord = _word.text;
		string _rightAnswer = "";
		string _wordType = "";
		List<string> _allAnswer = new List<string>();
		_textBoardDefinitionTitle.text = _tempWord;
		_wordChoose = _word;

		if (_mainDictionary.ContainsKey (_tempWord)) {
			var _desContent = JsonHelper.WordInfoCreateFromJson (_mainDictionary [_tempWord]);
			int _random = 0;

			if (_desContent.verb != null) {
				_random = Random.Range (0, _desContent.verb.Length);
				_rightAnswer = _desContent.verb [_random];
				_wordType = "verb";
			} else if (_desContent.noun != null) {
				_random = Random.Range (0, _desContent.noun.Length);
				_rightAnswer = _desContent.noun [_random];
				_wordType = "noun";
			} else if (_desContent.adjective != null) {
				_random = Random.Range (0, _desContent.adjective.Length);
				_rightAnswer = _desContent.adjective [_random];
				_wordType = "adjective";
			} else if (_desContent.adverb != null) {
				_random = Random.Range (0, _desContent.adverb.Length);
				_rightAnswer = _desContent.adverb [_random];
				_wordType = "adverb";
			} else if (_desContent.pronoun != null) {
				_random = Random.Range (0, _desContent.pronoun.Length);
				_rightAnswer = _desContent.pronoun [_random];
				_wordType = "pronoun";
			} else if (_desContent.preposition != null) {
				_random = Random.Range (0, _desContent.preposition.Length);
				_rightAnswer = _desContent.preposition [_random];
				_wordType = "preposition";
			} else if (_desContent.exclamation != null) {
				_random = Random.Range (0, _desContent.exclamation.Length);
				_rightAnswer = _desContent.exclamation [_random];
				_wordType = "exclamation";
			} else if (_desContent.possessive_determiner != null) {
				_random = Random.Range (0, _desContent.possessive_determiner.Length);
				_rightAnswer = _desContent.possessive_determiner [_random];
				_wordType = "possessive_determiner";
			} else if (_desContent.plural_noun != null) {
				_random = Random.Range (0, _desContent.plural_noun.Length);
				_rightAnswer = _desContent.plural_noun [_random];
				_wordType = "plural_noun";
			} else if (_desContent.proper_noun != null) {
				_random = Random.Range (0, _desContent.proper_noun.Length);
				_rightAnswer = _desContent.proper_noun [_random];
				_wordType = "proper_noun";
			} else if (_desContent.ordinal_number != null) {
				_random = Random.Range (0, _desContent.ordinal_number.Length);
				_rightAnswer = _desContent.ordinal_number [_random];
				_wordType = "ordinal_number";
			} else if (_desContent.cardinal_number != null) {
				_random = Random.Range (0, _desContent.cardinal_number.Length);
				_rightAnswer = _desContent.cardinal_number [_random];
				_wordType = "cardinal_number";
			} else if (_desContent.phrase != null) {
				_random = Random.Range (0, _desContent.phrase.Length);
				_rightAnswer = _desContent.phrase [_random];
				_wordType = "phrase";
			} else if (_desContent.determiner != null) {
				_random = Random.Range (0, _desContent.determiner.Length);
				_rightAnswer = _desContent.determiner [_random];
				_wordType = "determiner";
			}
		}

		_typeOfWord.text = _wordType;
		_allAnswer.AddRange(CreateAnswersList (_wordType,_rightAnswer));
		_allAnswer.Add (_rightAnswer);
		_answers = _rightAnswer;

		SuffleWord (_allAnswer);

		for (int i = 0; i < _allAnswer.Count; i++) {
			if (_allAnswer [i] == "") {
				_allAnswer [i] = "Undefine.";
			}

			_textAnswers [i].text = _allAnswer [i];
		}

		if (CheckWordWasAnswered (_word)) {
			ToggleChecker (true);
			MoveWrongPenguins ("all");
		} else {
			ToggleChecker (false);
			if (_penguinPos != null) {
				ResetPenguinPos ();
			}
		}
	}

	/// <summary>
	/// Creates the answers list.
	/// </summary>
	/// <returns>The answers list.</returns>
	/// <param name="_typeOfWord">Type of word.</param>
	List<string> CreateAnswersList(string _typeOfWord,string _rightAnswer){
		List<string> _answerList = new List<string> ();
		int _numAnswers = 3;////Change when change level
		int _escape = 50;

		while (_numAnswers > 0) {
			string _desOfWord = _mainDictionaryKey.RandomWord ();
			//Debug.Log (_desOfWord);
			var _desContent = JsonHelper.WordInfoCreateFromJson (_mainDictionary [_desOfWord]);

			if (_typeOfWord == "verb") {
				if (_desContent.verb != null) {
					int _random = Random.Range (0, _desContent.verb.Length);
					string _des = _desContent.verb [_random];
					_answerList.Add (_des);
					_numAnswers--;
				}
			} else if (_typeOfWord == "noun") {
				if (_desContent.noun != null) {
					int _random = Random.Range (0, _desContent.noun.Length);
					string _des = _desContent.noun [_random];
					_answerList.Add (_des);
					_numAnswers--;
				}
			} else if (_typeOfWord == "adjective") {
				if (_desContent.adjective != null) {
					int _random = Random.Range (0, _desContent.adjective.Length);
					string _des = _desContent.adjective [_random];
					_answerList.Add (_des);
					_numAnswers--;
				}
			} else if (_typeOfWord == "adverb") {
				if (_desContent.adverb != null) {
					int _random = Random.Range (0, _desContent.adverb.Length);
					string _des = _desContent.adverb [_random];
					_answerList.Add (_des);
					_numAnswers--;
				}
			} else if (_typeOfWord == "pronoun") {
				if (_desContent.pronoun != null) {
					int _random = Random.Range (0, _desContent.pronoun.Length);
					string _des = _desContent.pronoun [_random];
					_answerList.Add (_des);
					_numAnswers--;
				}
			} else if (_typeOfWord == "preposition") {
				if (_desContent.preposition != null) {
					int _random = Random.Range (0, _desContent.preposition.Length);
					string _des = _desContent.preposition [_random];
					_answerList.Add (_des);
					_numAnswers--;
				}
			} else if (_typeOfWord == "exclamation") {
				if (_desContent.exclamation != null) {
					int _random = Random.Range (0, _desContent.exclamation.Length);
					string _des = _desContent.exclamation [_random];
					_answerList.Add (_des);
					_numAnswers--;
				}
			} else if (_typeOfWord == "possessive_determiner") {
				if (_desContent.possessive_determiner != null) {
					int _random = Random.Range (0, _desContent.possessive_determiner.Length);
					string _des = _desContent.possessive_determiner [_random];
					_answerList.Add (_des);
					_numAnswers--;
				}
			} else if (_typeOfWord == "plural_noun") {
				if (_desContent.plural_noun != null) {
					int _random = Random.Range (0, _desContent.plural_noun.Length);
					string _des = _desContent.plural_noun [_random];
					_answerList.Add (_des);
					_numAnswers--;
				}
			} else if (_typeOfWord == "proper_noun") {
				if (_desContent.proper_noun != null) {
					int _random = Random.Range (0, _desContent.proper_noun.Length);
					string _des = _desContent.proper_noun [_random];
					_answerList.Add (_des);
					_numAnswers--;
				}
			} else if (_typeOfWord == "ordinal_number") {
				if (_desContent.ordinal_number != null) {
					int _random = Random.Range (0, _desContent.ordinal_number.Length);
					string _des = _desContent.ordinal_number [_random];
					_answerList.Add (_des);
					_numAnswers--;
				}
			} else if (_typeOfWord == "cardinal_number") {
				if (_desContent.cardinal_number != null) {
					int _random = Random.Range (0, _desContent.cardinal_number.Length);
					string _des = _desContent.cardinal_number [_random];
					_answerList.Add (_des);
					_numAnswers--;
				}
			} else if (_typeOfWord == "phrase") {
				if (_desContent.phrase != null) {
					int _random = Random.Range (0, _desContent.phrase.Length);
					string _des = _desContent.phrase [_random];
					_answerList.Add (_des);
					_numAnswers--;
				}
			} else if (_typeOfWord == "determiner") {
				if (_desContent.determiner != null) {
					int _random = Random.Range (0, _desContent.determiner.Length);
					string _des = _desContent.determiner [_random];
					_answerList.Add (_des);
					_numAnswers--;
				}
			}

			_escape--;
			if (_escape < 0) {
				_typeOfWord = "noun";
			}
		}

		//Add dot at the end of sentence
		for (int i = 0; i< _answerList.Count;i++) {
			var _answer = _answerList [i];
			if (_answer [_answer.Length - 1].ToString () != ".") {
				_answer += ".";
			}
		}

		return _answerList;
	}

	void SuffleWord(List<string> _deck){
		for (int i = _deck.Count - 1; i > 0; i--) {
			int _random = Random.Range(0,i+1);
			var _temp = _deck[i];
			_deck[i] = _deck[_random];
			_deck[_random] = _temp;
		}
	}

	/// <summary>
	/// Clicks the choose answer.
	/// </summary>
	/// <param name="_myAnswer">My answer.</param>
	public void ClickChooseAnswer(string _myAnswer){
		for (int i = 1; i < _definitionBoard.childCount; i++) {
			var _posOfAnswer = _definitionBoard.GetChild (i).GetChild (0).name;

			//Choose right letter answer (A || B || C || D)
			if (_posOfAnswer == _myAnswer) {
				var _answerContent = _definitionBoard.GetChild (i).GetChild(1).GetComponent<Text>().text;
				if (_answerContent == _answers) {
					ChooseRightAnswer (_myAnswer);
				} else {
					ChooseWrongAnswer ();
				}
			}
		}

		CheckResult ();
	}

	/// <summary>
	/// Chooses the right answer.
	/// </summary>
	/// <param name="_myAnswer">My answer.</param>
	void ChooseRightAnswer(string _myAnswer){
		if (_wordChoose.color != Color.red) {
			var _tempText = Instantiate(_textScorePlus,new Vector3(1.20f,-4f,0),Quaternion.identity) as TextMesh;
			_tempText.GetComponent<MeshRenderer> ().sortingOrder = 5;
			_tempText.transform.DOMoveY (-1.40f, 0.5f);
			Destroy (_tempText, 0.5f);

			if (!_isEndTime) {
				_score++;
			}
			_textScore.text = "" + _score;
			audioSource.PlayOneShot (_rightClip);
		}

		_wordChoose.color = Color.green;
		_listWordDefine.RemoveAll (x => x.text == _wordChoose.text);

		MoveWrongPenguins (_myAnswer);
		ToggleChecker (true);
		StartCoroutine (AutoChangeWord (2f,false));
	}

	/// <summary>
	/// Chooses the wrong answer.
	/// </summary>
	void ChooseWrongAnswer(){
		_listWordDefine.RemoveAll (x => x.text == _wordChoose.text);
		audioSource.PlayOneShot (_wrongClip);
		_wordChoose.color = Color.red;
		MoveWrongPenguins ("all");
		ToggleChecker (true);
		StartCoroutine (AutoChangeWord (2f,false));
	}

	/// <summary>
	/// Moves the wrong penguin.
	/// </summary>
	/// <param name="_rightPenguin">Right penguin.</param>
	void MoveWrongPenguins(string _rightPenguin){
		foreach (Transform _penguin in _penguinList) {
			_penguin.GetComponent<Button> ().interactable = false;
			if (_penguin.name != _rightPenguin) {
				float _posMove = _definitionBoard.transform.position.y;
				_penguin.DOMoveY (_posMove, 2f); 
			}
		}
	}

	/// <summary>
	/// Gets the penguin position.
	/// </summary>
	void GetPenguinPos(){
		_penguinPos = new List<Vector3> ();
		foreach (Transform _penguin in _penguinList) {
			_penguinPos.Add (_penguin.position);
		}
	}

	/// <summary>
	/// Resets the penguin position.
	/// </summary>
	void ResetPenguinPos(){
		for (int i = 0; i < _penguinList.Length; i++) {
			var _tempPenguin = _penguinList [i];
			var _tempPos = _penguinPos;
			_tempPenguin.DOMove (_tempPos[i], 1f);
			_tempPenguin.GetComponent<Button> ().interactable = true;
		}
	}

	/// <summary>
	/// Autos the change word.
	/// </summary>
	/// <returns>The change word.</returns>
	/// <param name="_time">Time.</param>
	/// <param name="_isFirstme">If set to <c>true</c> is firstme.</param>
	IEnumerator AutoChangeWord(float _time,bool _isFirstme){
		yield return new WaitForSeconds (_time);
		if (_listWordDefine.Count > 0) {
			int _random = Random.Range (0, _listWordDefine.Count);
			SetWordNeedToDedinition (_listWordDefine [_random]);
		}

		if (!_isFirstme) {
			ResetPenguinPos ();
		}
	}

	/// <summary>
	/// Toggles the checker.
	/// </summary>
	void ToggleChecker(bool _isCheck){
		List<Transform> _listAnswer = new List<Transform> ();
		for (int i = 0; i < 4; i++) {
			var _tempHeadText = _textAnswers [i].transform.parent;
			_listAnswer.Add (_tempHeadText);
		}

		//Use _isCheck to disable all right or wrong check --> use with word haven't defined
		if (_isCheck) {
			foreach (Transform _ans in _listAnswer) {
				var _answerContent = _ans.GetChild (1).GetComponent<Text> ().text;
				if (_answerContent == _answers) {
					_ans.GetChild (0).GetChild (0).gameObject.SetActive (true);
					_ans.GetChild (0).GetChild (1).gameObject.SetActive (false);
				} else {
					_ans.GetChild (0).GetChild (0).gameObject.SetActive (false);
					_ans.GetChild (0).GetChild (1).gameObject.SetActive (true);
				}
			}
		} else {
			foreach (Transform _ans in _listAnswer) {
				_ans.GetChild (0).GetChild (0).gameObject.SetActive (false);
				_ans.GetChild (0).GetChild (1).gameObject.SetActive (false);
			}
		}
	}

	void FillRedAllWordHaveNotDefine(){
		foreach (Text _word in _listWordDefine) {
			if (_word.color != Color.green) {
				_word.color = Color.red;
			}
		}
	}

	#endregion

	void CheckResult(){
		if (CheckWinCondition()) {
			_escapeCoroutine = true;
			_resultPannel.SetActive (true);
			_resultPannel.transform.GetChild (0).DOMoveY (2f, 1.5f);
			_scoreTextOfEndPannel.text = "" + _score;
			audioSource.PlayOneShot (_winClip);
		}
	}

	bool CheckWinCondition(){
		if (_listWordDefine.Count <= 0) {
			return true;
		} else {
			return false;
		}
	}

	public void ClickTutButton(){
		SystemController.instance.ClickTutorialButton ();
		Time.timeScale = 0;
	}

	public void ClickFinishedButton(){
		
	}

	public void ClickHomeButton(){
		SceneManager.LoadScene (0);
	}
}