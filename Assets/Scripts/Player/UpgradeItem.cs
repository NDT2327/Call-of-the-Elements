//using UnityEngine;

//public class UpgradeItem : MonoBehaviour
//{
//	[SerializeField] private string gemType; // "EarthGem" hoặc "FireGem"

//	private void OnTriggerEnter2D(Collider2D other)
//	{
//		if (other.CompareTag("Player"))
//		{
//			// Cập nhật trạng thái trong GameManager
//			GameManager.Instance.OnSkillCollected(gemType);
//			AudioManager.instance.PlayItemPickupSound();

//			// Mở khóa kỹ năng cho Player
//			Player player = other.GetComponent<Player>();
//			if (player != null)
//			{
//				if (gemType == "EarthGem")
//				{
//					player.UnlockSpecialAttack(GameManager.Map.Earth);
//				}
//				else if (gemType == "FireGem")
//				{
//					player.UnlockSpecialAttack(GameManager.Map.Lava);
//				}
//			}

//			Destroy(gameObject); // Hủy item sau khi nhặt
//		}
//	}

//	// Phương thức để EnemyHP gán gemType
//	public void SetGemType(string type)
//	{
//		gemType = type;
//	}
//}
