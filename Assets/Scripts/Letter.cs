using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Letter : MonoBehaviour
{
    [SerializeField] private Color _correctColor, _incorrectColor, _inWordColor;
    
    [SerializeField] private Image _background;
    private TMP_InputField _input;

    public TMP_InputField Input => _input;

    void Awake()
    {
        _input = GetComponentInChildren<TMP_InputField>();
    }
    
    public void Select()
    {
        _input.Select();
    }

    public void ToggleInteraction(bool canInteract = true)
    {
        _input.interactable = canInteract;
        if (!canInteract)
            _input.DeactivateInputField();
        else
            _input.ActivateInputField();
    }

    public string GetLetter()
    {
        return _input.text;
    }

    public void ClearText()
    {
        _input.text = "";
    }

    public void SetColor(GuessType guess)
    {
        _background.color = guess == GuessType.Correct ? _correctColor :
            guess == GuessType.InWord ? _inWordColor : _incorrectColor;
    }

    public void ResetColor()
    {
        _background.color = Color.white;
    }
}
