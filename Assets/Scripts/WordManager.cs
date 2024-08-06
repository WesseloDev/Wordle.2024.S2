using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum GuessType
{
    Correct,
    InWord,
    NotInWord
}

public class WordleManager : MonoBehaviour
{
    private static int _wordLength = 5;
    [SerializeField] private int _guesses = 6;
    
    [SerializeField] private GameObject _guessParent;
    [SerializeField] private GameObject _guessPrefab;
    private GuessCell[] _guessCells;

    private TMP_InputField _inputField;
    [SerializeField] private GameObject _button;

    private List<string> _words = new List<string>();
    private string _word;
    private int _currentGuess = 0;

    private bool _gameOver = false;

    public static int WordLength => _wordLength;
    
    void Start()
    {
        _inputField = FindObjectOfType<TMP_InputField>();
        _inputField.characterLimit = _wordLength;
        
        _guessCells = new GuessCell[_guesses];
        
        for (int i = 0; i < _guesses; i++)
        {
            _guessCells[i] = Instantiate(_guessPrefab, _guessParent.transform).GetComponent<GuessCell>();
        }
        
        string[] wordArray = System.IO.File.ReadAllLines(@"Assets\words_alpha.txt");

        foreach (string word in wordArray)
        {
            if (word.Length == _wordLength)
                _words.Add(word);
        }   
        
        Restart();
    }
    
    /*void Start()
    {
        
    }*/

    public void Restart()
    {
        _word = PickWord();
        
        foreach (GuessCell cell in _guessCells)
        {
            cell.Reset();
        }

        _currentGuess = 0;
        _gameOver = false;
        
        _guessCells[_currentGuess].FocusGuess();
    }

    string PickWord()
    {
        return _words[Random.Range(0, _words.Count)];
    }

    bool IsValidWord(string word)
    {
        return _words.Contains(word);
    }

    public void CompareWords(string guess)
    {
        if (_gameOver)
            return;

        guess = guess.ToLower();
        
        if (!IsValidWord(guess))
        {
            Debug.Log("Invalid word: " + guess);
            return;
        }

        Debug.Log("Running comparison");
        Debug.Log("Comparing " + guess + " to " + _word);

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

        ShowGuess(guess, guessInfo);

        _inputField.text = "";
        _currentGuess++;

        if (guess != _word && _currentGuess <= _guesses) 
            return;
        
        _gameOver = true;
        _button.gameObject.SetActive(true);
        _inputField.gameObject.SetActive(false);
    }

    void ShowGuess(string word, GuessType[] guess)
    {
        _guessCells[_currentGuess].UpdateLetterCells(word, guess);
    }
}
