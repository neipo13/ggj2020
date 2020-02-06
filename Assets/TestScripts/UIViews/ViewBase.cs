using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewBase : MonoBehaviour
{
    private Animator _animator;
    public Animator animator {
        get {
            if (_animator == null)
                _animator = GetComponent<Animator>();
            return _animator;
        }
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
}
