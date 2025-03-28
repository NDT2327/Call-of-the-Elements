using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class NewBehaviourScript : MonoBehaviour
{
	public GameObject dialoguePanel;
	public Text dialogueText;
	public GameObject continueButton;
	public float wordSpeed = 0.05f; // Tốc độ gõ chữ
	private List<string> dialogueLines = new List<string>
	{
		"You... have finally arrived... I am one of the last guardians of the Earth Gem. Elementia... this world was once peaceful, sustained by the harmony of four elemental regions: Earth, Fire, Wind, and Water. But now, everything is falling apart...",
		"The Conqueror, a tyrannical warlord driven by greed for power, has invaded our lands, defeated the guardians, and stolen the Earth Gem and Fire Gem. He now sets his sights on Wind and Water as well...",
		"I no longer have the strength to fight... but you, brave knight, you are Elementia's last hope. Before I was defeated, I managed to wrest a fragment of the Earth Gem and Fire Gem from The Conqueror's grasp. I will bestow their power upon you now.",
		"The power of Earth will let you summon stone from the ground, while Fire allows you to scorch your enemies with fierce flames. Use [O] to switch between them, and [U] to unleash them when you have enough mana.",
		"You must journey through this Forest, confront the Golem – one of The Conqueror’s minions. Then, venture into the Inferno to defeat the Fire Demon. Finally, face The Conqueror himself within his Castle.",
		"Along the way, seek out Health Potions to restore your vitality and Mana Potions to replenish your mana. They are scattered throughout or dropped by foes. When you find a Checkpoint, touch it to save your progress.",
		"Now go, knight... Time is running out. I entrust Elementia to you..."
	};
	private int index = 0;
	private bool playerIsClose = false;
	private bool hasStartedDialogue = false;

	void Start()
	{
		dialoguePanel.SetActive(false); // Ẩn panel khi bắt đầu
		continueButton.SetActive(false); // Ẩn nút Continue khi bắt đầu
	}

	void Update()
	{
		// Kiểm tra nếu văn bản đã hiển thị đầy đủ thì hiện nút Continue
		if (dialoguePanel.activeInHierarchy && dialogueText.text == dialogueLines[index])
		{
			continueButton.SetActive(true);
		}

		// Nhấn Enter để chuyển câu tiếp theo
		if (dialoguePanel.activeInHierarchy && Input.GetKeyDown(KeyCode.Return))
		{
			NextLine();
		}
	}

	private void ShowDialogue()
	{
		if (!hasStartedDialogue && dialogueLines.Count > 0)
		{
			dialoguePanel.SetActive(true);
			StartCoroutine(Typing());
			hasStartedDialogue = true;
			GrantElementalPowers(); // Truyền sức mạnh ngay khi bắt đầu hội thoại
		}
		else if (dialogueLines.Count == 0)
		{
			Debug.LogWarning("Dialogue lines are empty!");
		}
	}

	private void ResetDialogue()
	{
		dialogueText.text = "";
		index = 0;
		dialoguePanel.SetActive(false);
		continueButton.SetActive(false);
	}

	IEnumerator Typing()
	{
		dialogueText.text = "";
		foreach (char letter in dialogueLines[index].ToCharArray())
		{
			dialogueText.text += letter;
			yield return new WaitForSeconds(wordSpeed);
		}
	}

	public void NextLine()
	{
		continueButton.SetActive(false);

		if (index < dialogueLines.Count - 1)
		{
			index++;
			StartCoroutine(Typing());
		}
		else
		{
			ResetDialogue();
			SceneManager.LoadScene("Earth"); // Chuyển sang map Earth sau khi hết hội thoại
		}
	}

	public void SkipDialogue()
	{
		StopAllCoroutines(); // Dừng hiệu ứng gõ chữ nếu đang chạy
		ResetDialogue();
		SceneManager.LoadScene("Earth"); // Chuyển ngay sang map Earth
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			playerIsClose = true;
			ShowDialogue();
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			playerIsClose = false;
			SkipDialogue(); // Tự động skip nếu người chơi rời khỏi
		}
	}

	private void GrantElementalPowers()
	{
		Player player = FindObjectOfType<Player>();
		if (player != null)
		{
			Debug.Log("Earth and Fire abilities granted by Earth Guardian Monk!");
			// Không cần mở khóa vì Player đã có sẵn Earth/Fire trong script trước
		}
	}
}
