using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class GuessCell : MonoBehaviour
{
    [SerializeField] private GameObject _letterPrefab;
    [SerializeField] private Color _correctColor, _incorrectColor, _inWordColor;
    
    private TMP_InputField[] _letters;

    private int _index = 0;
    private bool _canSelectNew = true;
    
    void Awake()
    {
        _letters = new TMP_InputField[WordleManager.WordLength];
        
        for (int i = 0; i < WordleManager.WordLength; i++)
        {
            _letters[i] = Instantiate(_letterPrefab, transform).GetComponentInChildren<TMP_InputField>();
            _letters[i].onValueChanged.AddListener(SelectNextLetter);
        }
    }

    public void UpdateLetterCells(string word, GuessType[] guess)
    {
        for (int i = 0; i < WordleManager.WordLength; i++)
        {
            _letters[i].text = word[i].ToString();
            /*_letters[i].color = guess[i] == GuessType.Correct ? _correctColor :
                guess[i] == GuessType.InWord ? _inWordColor : _incorrectColor;*/
        }
    }

    public void Reset()
    {
        for (int i = 0; i < WordleManager.WordLength; i++)
        {
            _letters[i].text = "";
            _letters[i].interactable = false;
        }
    }

    public void FocusGuess()
    {
        for (int i = 0; i < WordleManager.WordLength; i++)
        {
            _letters[i].interactable = true;
        }
        
        _index = 0;
        _letters[_index].Select();
    }

    public void DisableEditing()
    {
        for (int i = 0; i < WordleManager.WordLength; i++)
        {
            _letters[i].interactable = false;
        }
    }

    void SelectNextLetter(string letter)
    {
        if (!_canSelectNew)
            return;
        
        _canSelectNew = false;

        if (letter == "")
        {
            _index--;
            _index = Mathf.Clamp(_index, 0, WordleManager.WordLength);
            _letters[_index].text = "";
        }
        else
        {
            _index++;
            _index = Mathf.Clamp(_index, 0, WordleManager.WordLength);
        }

        _canSelectNew = true;

        if (_index == WordleManager.WordLength)
            Guess();
        //EventSystem.current.SetSelectedGameObject(null);
        else
            _letters[_index].Select();
    }

    void Guess(string letter = "")
    {
        DisableEditing();
        
        string word = "";
        
        for (int i = 0; i < WordleManager.WordLength; i++)
        {
            word += _letters[i].text;
        }
        
        Debug.Log(word);
    }
}
