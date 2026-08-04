using UnityEngine;
using TMPro;

namespace TechTest.UI
{
    public class EnemyUI : MonoBehaviour
    {
        public TechTest.Combat.Unit myUnit;
        public TextMeshProUGUI hpText;
        public TextMeshProUGUI blockText;
        public TextMeshProUGUI intentText;

        public void Setup(TechTest.Combat.Unit unit)
        {
            myUnit = unit;
        }

        private void Update()
        {
            if (myUnit != null && !myUnit.isDead)
            {
                if (hpText != null) hpText.text = $"{myUnit.unitData.unitName}: {myUnit.currentHP} / {myUnit.unitData.maxHP}";
                if (blockText != null) blockText.text = $"Block: {myUnit.currentBlock}";
                // Tampilkan intent damage yang sebenarnya
                if (intentText != null) intentText.text = $"Intent: Atk {myUnit.nextTurnIntentDamage}";
            }
            else
            {
                // Sembunyikan atau hancurkan UI jika musuh mati
                Destroy(gameObject);
            }
        }
    }
}
