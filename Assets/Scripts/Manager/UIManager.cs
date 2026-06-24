using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] GameObject go_timer;
    [SerializeField] TextMeshProUGUI text_hud_time;
    [Header("PlayerStas")]
    [SerializeField] TextMeshProUGUI text_playerStats_currentGun;
    [SerializeField] TextMeshProUGUI text_playerStats_fireType;
    [Header("Wave")]
    [SerializeField] TextMeshProUGUI text_waveCount;
    [Header("Upgrade Screen")]
    [SerializeField] GameObject upgradePanel;           
    [SerializeField] Button[] upgradeButtons;           
    [SerializeField] Animator upgradeAnimator;  
    

    private List<UpgradeType> currentUpgrades = new List<UpgradeType>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        go_timer.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #region Timer
    public void ActivateTimer()
    {
        go_timer.SetActive(true);
    }
    public void DeactivateTimer()
    {
        go_timer.SetActive(false);
    }
    public void SetTextTimer(int timer)
    {
        text_hud_time.text = timer.ToString();
    }
    #endregion
    #region Gun
    public void playerGunAmmoStats(int value)
    {
        text_playerStats_currentGun.text = value.ToString();
    }
    public void PlayerGunFireType(string nameT)
    {
        text_playerStats_fireType.text = nameT.ToString();
    }
    #endregion
    public void SetWaveText(int value)
    {
        text_waveCount.text = value.ToString();
    }
    #region Upgrade Screen (Roguelike)

    /// <summary>
    /// Mostra a tela de upgrades com 3 opções aleatórias
    /// </summary>
    public void ShowUpgradeScreen()
    {
        if (UpgradeManager.Instance == null) return;

        currentUpgrades = UpgradeManager.Instance.GetRandomUpgrades(3);

        // Atribui os upgrades aos botões
        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            if (i < currentUpgrades.Count)
            {
                int index = i;
                upgradeButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentUpgrades[i].ToString();
                upgradeButtons[i].onClick.RemoveAllListeners();
                upgradeButtons[i].onClick.AddListener(() => ChooseUpgrade(index));
                upgradeButtons[i].gameObject.SetActive(true);
            }
            else
            {
                upgradeButtons[i].gameObject.SetActive(false);
            }
        }

        upgradePanel.SetActive(true);

        // === Chama a animação (se tiver Animator) ===
        if (upgradeAnimator != null)
        {
            //upgradeAnimator.SetBool("Open", true);   // Crie uma trigger chamada "Show" na animação
        }
        else
        {
            Debug.Log("Upgrade Panel apareceu (adicione Animator depois se quiser animação)");
        }
    }

    /// <summary>
    /// Jogador escolheu um upgrade
    /// </summary>
    public void ChooseUpgrade(int index)
    {
        if (index >= currentUpgrades.Count) return;

        // Aplica o upgrade
        UpgradeManager.Instance.ApplyUpgrade(currentUpgrades[index]);

        //upgradeAnimator.SetBool("Open", false); 
        // Esconde a tela
        upgradePanel.SetActive(false);

        // Despausa o jogo
        Time.timeScale = 1f;

        // Inicia o timer de 7 segundos para a próxima onda
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartWaveTimer();   // Vamos criar esse método no próximo passo
        }
    }

    public void HideUpgradeScreen()
    {
        if (upgradePanel != null)
            upgradePanel.SetActive(false);
    }

    #endregion
}
