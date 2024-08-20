using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class GuessCell : MonoBehaviour
{
    [SerializeField] private GameObject _letterPrefab;
    
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
        }

        ToggleEditing(false);
        
        WordleManager.Instance.onReset.AddListener(Reset);
    }

    void Update()
    {
        if (!_active)
            return;
        
        if (!EventSystem.current.currentSelectedGameObject)
            _letters[_index].Select();
    }

    public IEnumerator UpdateLetterCells(GuessType[] guess)
    {
        for (int i = 0; i < WordleManager.WordLength; i++)
        {
            StartCoroutine(_letters[i].Flip(guess[i]));
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void Reset()
    {
        _active = false;
    }

    public void FocusGuess()
    {
        _active = true;
        _index = 0;
        _letters[_index].ToggleInteraction();
        _letters[_index].Select();
    }

    public void ToggleEditing(bool editingOn = true)
    {
        if (_index != WordleManager.WordLength - 1)
            _letters[_index].ToggleInteraction(editingOn);
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

    public void SelectAtIndex()
    {
        if (_index != WordleManager.WordLength - 1)        
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
    }
}
