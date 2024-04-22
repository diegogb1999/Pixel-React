using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LayoutPlayer : MonoBehaviour
{
    private float maxHp = 10;
    public float currentHp;
    public Image fill;
    public PauseMenuScript pauseMenuScript;

    void Start()
    {
        currentHp = maxHp;
        UpdateHealthBar();
        
    }

    public void updateHp(int amount)
    {
        currentHp -= amount;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);
        UpdateHealthBar();
        if (currentHp <= 0)
        {
            pauseMenuScript.Restart();
        }
    }

    private void UpdateHealthBar()
    {
        float targetFillAmount = currentHp / maxHp;
        fill.fillAmount = targetFillAmount;
    }
}
