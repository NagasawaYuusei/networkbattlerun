using UnityEngine;
using UnityEngine.UI;

public class CountDown : MonoBehaviour
{
    [SerializeField] Text _countDownText;
    bool _countDownEnabled;
    [SerializeField] float _countDownTime = 3f;
    float _timer;
    
    public void CountDownStart()
    {
        _countDownEnabled = true;
        _countDownText.gameObject.SetActive(true);
    }

    void Start()
    {
        _timer = _countDownTime + 0.49f;
    }

    void Update()
    {
        if(_countDownEnabled)
        {
            _timer -= Time.deltaTime;
            if (_timer < 0.5f)
            {
                _countDownText.gameObject.SetActive(false);
                GameManager.Instance.CanPlayerMove();
                Destroy(this);
            }
            _countDownText.text = _timer.ToString("F0");
        }
    }
}
