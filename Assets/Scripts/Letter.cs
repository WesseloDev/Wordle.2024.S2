using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Letter : MonoBehaviour
{
    [SerializeField] private Color _correctColor, _incorrectColor, _inWordColor;

    private Animator _animator;
    
    [SerializeField] private Image _background;
    private TMP_InputField _input;

    public TMP_InputField Input => _input;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _input = GetComponentInChildren<TMP_InputField>();
        WordleManager.Instance.onReset.AddListener(Reset);
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

    public IEnumerator Flip(GuessType guess)
    {
        _animator.SetTrigger("Flip");

        while (!CanSetColor())
        {
            yield return null;
        }
        
        SetColor(GetColorFromGuess(guess));
    }

    private bool CanSetColor()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).IsName("FlipUp");
    }

    private Color GetColorFromGuess(GuessType guess)
    {
        return guess == GuessType.Correct ? _correctColor : guess == GuessType.InWord ? _inWordColor : _incorrectColor;
    }
    
    public void SetColor(Color color)
    {
        _background.color = color;
    }

    public void Reset()
    {
        ClearText();
        ToggleInteraction(false);
        SetColor(Color.white);
    }
}
