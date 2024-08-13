using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class GuessCell : MonoBehaviour
{
    [SerializeField] private GameObject _letterPrefab;
    [SerializeField] private Color _correctColor, _incorrectColor, _inWordColor;
    
    private Letter[] _letters;

    private int _index = 0;
    private bool _canSelectNew = true;
    private bool _active = false;
    
    void Awake()
    {
        _letters = new Letter[WordleManager.WordLength];
        
        for (int i = 0; i < WordleManager.WordLength; i++)
        {
            _letters[i] = Instantiate(_letterPrefab, transform).GetComponent<Letter>();
            _letters[i].Input.onValueChanged.AddListener(SelectNextLetter);
            //_letters[i].Input.onSelect.AddListener(OnLetterSelected);
        }

        ToggleEditing(false);
    }

    void Update()
    {
        if (!_active)
            return;
        
        if (!EventSystem.current.currentSelectedGameObject)
            _letters[_index].Select();
    }

    public void UpdateLetterCells(GuessType[] guess)
    {
        for (int i = 0; i < WordleManager.WordLength; i++)
        {
            _letters[i].SetColor(guess[i]);
        }
    }

    public void Reset()
    {
        _active = false;
        
        for (int i = 0; i < WordleManager.WordLength; i++)
        {
            _letters[i].ClearText();
            _letters[i].ToggleInteraction(false);
            _letters[i].ResetColor();
        }
    }

    public void FocusGuess()
    {
        /*for (int i = 0; i < WordleManager.WordLength; i++)
        {
            _letters[i].ToggleInteraction(true);
        }*/

        _active = true;
        _index = 0;
        _letters[_index].ToggleInteraction();
        _letters[_index].Select();
    }

    public void ToggleEditing(bool editingOn = true)
    {
        _letters[_index].ToggleInteraction(editingOn);
        
        /*for (int i = 0; i < WordleManager.WordLength; i++)
        {
            _letters[i].ToggleInteraction(editingOn);
        }*/
    }

    public void DeleteLetter()
    {
        if (!_canSelectNew)
            return;
        
        _canSelectNew = false;

        if (_letters[_index].GetLetter() == "")
        {
            _letters[_index].ToggleInteraction(false);
            _index--;
            _index = Mathf.Clamp(_index, 0, WordleManager.WordLength - 1);
        }

        _letters[_index].ClearText();
        _letters[_index].ToggleInteraction();
        _letters[_index].Select();

        _canSelectNew = true;
    }
    
    void SelectNextLetter(string letter)
    {
        if (!_canSelectNew || letter == "")
            return;

        _canSelectNew = false;
        
        _letters[_index].ToggleInteraction(false);
        
        _index++;
        _index = Mathf.Clamp(_index, 0, WordleManager.WordLength - 1);

        if (_index != WordleManager.WordLength - 1 || _letters[_index].GetLetter() == "")
            _letters[_index].ToggleInteraction();

        _letters[_index].Select();
        
        _canSelectNew = true;
    }

    void OnLetterSelected(string _)
    {
        //print(_index);
        for (int i = 0; i < _letters.Length; i++)
        {
            
        }
        
        _letters[_index].Select();
    }

    public void SelectAtIndex()
    {
        _letters[_index].Select();
    }
    
    public string GetWord(string letter = "")
    {
        string word = "";
        
        for (int i = 0; i < WordleManager.WordLength; i++)
        {
            word += _letters[i].GetLetter();
        }

        return word;

        //bool validGuess = WordleManager.Instance.CompareWords(word);

        /*if (validGuess && WordleManager.CanContinue)
            Debug.Log("Valid guess");
        else if (WordleManager.CanContinue)
        {
            ToggleEditing();
            _letters[_index].Select();
            //_letters[_letters.Length - 1].Select();
        }
        else
            Debug.Log("No more guesses");*/
    }
}
