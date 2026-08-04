using UnityEngine;

namespace TechTest.Combat
{
    // Membutuhkan Collider (misal: BoxCollider2D) agar mouse bisa mendeteksinya
    [RequireComponent(typeof(Collider2D))]
    public class Targetable : MonoBehaviour
    {
        private Unit unit;
        private BattleManager battleManager;

        private void Start()
        {
            unit = GetComponent<Unit>();
            battleManager = FindFirstObjectByType<BattleManager>();
        }

        private void OnMouseEnter()
        {
            if (battleManager != null)
            {
                battleManager.HoverTarget(unit);
            }
        }

        private void OnMouseExit()
        {
            if (battleManager != null && unit != null)
            {
                battleManager.ClearHover(unit);
            }
        }

        private void OnMouseDown()
        {
            if (battleManager != null && battleManager.state == BattleState.ChoosingTarget)
            {
                unit.Flash(false);
                battleManager.ExecuteTargetedCard(unit);
            }
        }
    }
}
