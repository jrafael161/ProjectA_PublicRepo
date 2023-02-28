using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBarUI : MonoBehaviour
{
    [SerializeField]
    Slider _hpBarSlider;

    [SerializeField]
    float _timeUntilBarIsHidden = 0;

    private void Start()
    {
        _hpBarSlider = GetComponentInChildren<Slider>();
    }

    private void Update()
    {
        _timeUntilBarIsHidden = _timeUntilBarIsHidden - Time.deltaTime;

        if (_hpBarSlider != null)
        {
            if (_timeUntilBarIsHidden <= 0)
            {
                _timeUntilBarIsHidden = 0;
                _hpBarSlider.gameObject.SetActive(false);
            }
            else
            {
                if (!_hpBarSlider.gameObject.activeInHierarchy)
                {
                    _hpBarSlider.gameObject.SetActive(true);
                }
            }

            if (_hpBarSlider.value <= 0)
            {
                Destroy(_hpBarSlider.gameObject);
            }
        }
    }

    public void SetHealth(int newHealthValue)
    {
        _hpBarSlider.value = newHealthValue;
        _timeUntilBarIsHidden = 3;
    }

    public void SetMaxHealth(int maxHealth)
    {
        _hpBarSlider.maxValue = maxHealth;
        _hpBarSlider.value = maxHealth;
    }
}
