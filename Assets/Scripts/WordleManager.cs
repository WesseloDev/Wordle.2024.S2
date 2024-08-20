using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public enum GuessType
{
    Correct,
    InWord,
    NotInWord
}

public class WordleManager : MonoBehaviour
{
    public UnityEvent onWin, onLose, onGameEnd, onReset;
    
    private static int _wordLength = 5;
    private static int _guesses = 6;
    
    [SerializeField] private GameObject _guessParent;
    [SerializeField] private GameObject _guessPrefab;
    private GuessCell[] _guessCells;
    
    [SerializeField] private GameObject _button;

    //[SerializeField] private string _wordPath;
    [SerializeField] private TextAsset _wordList;
    [SerializeField] private List<string> _words = new List<string>();
    private string _word;
    private static int _currentGuess = 0;

    private static bool _gameOver = false;
    private static bool _canGuess = false;
    
    public static int WordLength => _wordLength;
    public static bool CanGuess => _canGuess;
    public static bool CanContinue => !_gameOver && _guesses != _currentGuess;

    private static WordleManager _instance;

    public static WordleManager Instance
    {
        get
        {
            return _instance;
        }
        set
        {
            if (_instance)
            {
                Debug.LogWarning("Wordle Manager already exists. Remove second copy.");
                Destroy(value);
                return;
            }

            _instance = value;
        }
    }

    void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        _guessCells = new GuessCell[_guesses];
        
        for (int i = 0; i < _guesses; i++)
        {
            _guessCells[i] = Instantiate(_guessPrefab, _guessParent.transform).GetComponent<GuessCell>();
        }

        Reset();
    }
    
    void Update()
    {
        if (_canGuess && Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(CompareWords(_guessCells[_currentGuess].GetWord()));
        }

        if (_canGuess && Input.GetKeyDown(KeyCode.Backspace))
        {
            _guessCells[_currentGuess].DeleteLetter();
        }
    }

    [ContextMenu ("Get Words")]
    public void GetWords()
    {
        _words.Clear();

        string[] wordArray = _wordList.text.Split(' ', ',', '\n');

        foreach (string word in wordArray)
        {
            string newWord = word.ToLower();
            if (_words.Contains(newWord)) continue;
            if (HasSpecialCharacter(newWord)) continue;
            if (newWord.Length == _wordLength)
                _words.Add(newWord);
        }
    }

    public bool HasSpecialCharacter(string word)
    {
        foreach (char ch in word)
            if (!char.IsLetter(ch))
                return true;

        return false;
    }

    public void Reset()
    {
        onReset.Invoke();
        _word = PickWord();

        _currentGuess = 0;
        _gameOver = false;
        _canGuess = true;
        
        _guessCells[_currentGuess].FocusGuess();
    }

    string PickWord()
    {
        return _words[Random.Range(0, _words.Count)];
    }

    bool IsValidWord(string word)
    {
        if (HasSpecialCharacter(word))
            return false;
        if (word.Length != _wordLength)
            return false;
        
        return true; //_words.Contains(word);
    }

    public IEnumerator CompareWords(string guess)
    {
        if (_gameOver || !_canGuess)
            yield break;

        _canGuess = false;
        
        _guessCells[_currentGuess].ToggleEditing(false);
        
        guess = guess.ToLower();
        
        if (!IsValidWord(guess))
        {
            Debug.Log("Invalid word: " + guess);
            _guessCells[_currentGuess].ToggleEditing(true);
            _guessCells[_currentGuess].SelectAtIndex();
            _canGuess = true;
            yield break;
        }
        
        //Debug.Log("Comparing " + guess + " to " + _word);

        GuessType[] guessInfo = new GuessType[guess.Length];

        for (int i = 0; i < _word.Length; i++)
        {
            if (guess[i] == _word[i])
                guessInfo[i] = GuessType.Correct;
            //Debug.Log(guess[i] + " is in correct spot");
            else if (_word.Contains(guess[i]))
                guessInfo[i] = GuessType.InWord;
            //Debug.LogWarning(guess[i] + " is in the word, in the wrong spot");
            else
                guessInfo[i] = GuessType.NotInWord;
            //Debug.LogError(guess[i] + " is not in word");
        }

        yield return StartCoroutine(_guessCells[_currentGuess].UpdateLetterCells(guessInfo));
        //StartCoroutine(DelayBeforeResults());

        _canGuess = true;
        
        _currentGuess++;

        if (guess != _word && _currentGuess < _guesses)
        {
            _guessCells[_currentGuess].FocusGuess();
            yield break;
        }

        _gameOver = true;
        onGameEnd.Invoke();
    }

}
