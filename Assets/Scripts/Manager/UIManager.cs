using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject go_timer;
    [SerializeField] TextMeshProUGUI text_hud_time;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        go_timer.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
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
}
