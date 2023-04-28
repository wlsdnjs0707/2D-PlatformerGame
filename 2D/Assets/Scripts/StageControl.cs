using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageControl : MonoBehaviour
{
    // UI
    public TMP_Text stageText;
    public GameObject winText;
    public Button restartButton; // ����� ��ư
    public GameObject gameoverUI;
    public GameObject winUI;

    // ���� ��������
    private int currentStage = 0;

    // Enemy List
    public GameObject[] Enemy;
    private GameObject currentEnemy;

    // �� ���� ��ġ
    private Vector3 spawnPosition = new Vector3(12, 0, -1);

    // Start is called before the first frame update
    void Start()
    {
        restartButton.onClick.AddListener(Restart);
        SetNextStage();
        StartCoroutine(UpdateText());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    void Restart()
    {
        gameoverUI.SetActive(false);
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SetNextStage()
    {
        // ������ ������ Ȯ��
        if (currentStage == Enemy.Length)
        {
            Time.timeScale = 0;
            winUI.SetActive(true);
            return;
        }

        if (currentStage > 0)
        {
            StartCoroutine(PrintWinText()); // �¸� �ؽ�Ʈ ���
        }
        currentStage++;
        currentEnemy = Enemy[currentStage-1]; // ���� �� ����
        Instantiate(currentEnemy, spawnPosition, Quaternion.identity); // �� ����
        StartCoroutine(UpdateText());
    }

    IEnumerator PrintWinText()
    {
        winText.SetActive(true);
        yield return new WaitForSeconds(1);
        winText.SetActive(false);
    }

    IEnumerator UpdateText()
    {
        yield return new WaitForSeconds(1);
        stageText.color = new Color(stageText.color.r, stageText.color.g, stageText.color.b, 1);
        stageText.text = string.Format($"STAGE {currentStage}");
        StartCoroutine(TextFadeOut());
    }

    IEnumerator TextFadeOut()
    {
        yield return new WaitForSeconds(2.0f);

        while (stageText.color.a > 0.0f)
        {
            stageText.color = new Color(stageText.color.r, stageText.color.g, stageText.color.b, stageText.color.a - (Time.deltaTime / 1.0f));

            yield return null;
        }
    }
}
