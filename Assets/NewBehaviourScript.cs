using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject dialoguePanel;
    public Text dialogueText;
    public string[] dialogue;
    private int index;

    public GameObject contButton;
    public float wordSpeed;
    public bool playerIsClose;

    void Update()
    {
        if (playerIsClose && !dialoguePanel.activeInHierarchy)
        {
            ShowDialogue(); // Tự động hiện hội thoại khi player đến gần
        }

        if (dialogueText.text == dialogue[index])
        {
            contButton.SetActive(true);
        }

        // Kiểm tra nếu nhấn Enter thì chuyển câu tiếp theo
        if (dialoguePanel.activeInHierarchy && Input.GetKeyDown(KeyCode.Return))
        {
            NextLine();
        }

    }

    private void ShowDialogue()
    {
        if (dialogue.Length > 0)
        {
            dialoguePanel.SetActive(true);
            StartCoroutine(Typing());
        }
        else
        {
            Debug.LogWarning("Dialogue array is empty!");
        }
    }

    private void ZeroText()
    {
        dialogueText.text = "";
        index = 0;
        dialoguePanel.SetActive(false);
        contButton.SetActive(false);
    }

    IEnumerator Typing()
    {
        dialogueText.text = "";
        foreach (char letter in dialogue[index].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }
    }

    public void NextLine()
    {
        contButton.SetActive(false);

        if (index < dialogue.Length - 1)
        {
            index++;
            StartCoroutine(Typing());
        }
        else
        {
            ZeroText();
            SceneManager.LoadScene("Earth"); // Chuyển màn hình sau hội thoại
        }
    }


    public void SkipDialogue()
    {
        ZeroText();
        SceneManager.LoadScene("Earth"); // Chuyển màn khi bấm Skip
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = true;
            ShowDialogue(); // Hiện hội thoại ngay khi vào vùng trigger
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;
            ZeroText();
        }
    }
}
