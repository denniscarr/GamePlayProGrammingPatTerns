using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour {

    Text m_Text { get { return GetComponent<Text>(); } }
    string defaultString;
    [HideInInspector] public int score = 0;

    private void Awake() {
        m_Text.text = "Score:";
        defaultString = m_Text.text;
        SetScoreText(score);
        GameEventManager.instance.Subscribe<GameEvents.EnemyDied>(IncreaseScoreByOne);
    }

    public void IncreaseScoreByOne(GameEvent gameEvent) {
        score++;
        SetScoreText(score);
    }

    public void SetScoreText(int newScore) {
        m_Text.text = defaultString + " " + newScore.ToString();
    }
}
