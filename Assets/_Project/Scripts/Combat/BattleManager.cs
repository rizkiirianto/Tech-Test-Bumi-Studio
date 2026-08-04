using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TechTest.Data;

namespace TechTest.Combat
{
    public enum BattleState
    {
        Start,
        PlayerTurn,
        ChoosingTarget,
        EnemyTurn,
        ExecutingCard, // Mencegah spam klik saat animasi jalan
        Won,
        Lost
    }

    public class BattleManager : MonoBehaviour
    {
        public BattleState state;

        public Unit playerUnit;
        public List<Unit> activeEnemies = new List<Unit>();

        [Header("Enemy Spawning")]
        public Transform[] enemySpawnPoints;
        public GameObject enemyPrefab;
        public GameObject enemyUIPrefab;
        public Transform[] enemyUIContainers;

        public DeckManager deckManager;

        [Header("Energy System")]
        public int currentEnergy;
        public int maxEnergy;

        // Pending card for targeting
        private CardData pendingCard;

        [Header("Audio & Game Feel")]
        public AudioSource bgmSource;
        public AudioSource sfxSource;

        public AudioClip battleBGM;
        public AudioClip winBGM;
        public AudioClip loseBGM;
        public AudioClip sfxLunge;
        public AudioClip sfxHit;
        public AudioClip sfxHeal;
        public AudioClip sfxButtonClick;

        private void Update()
        {
            // Cancel targeting with right click
            if (state == BattleState.ChoosingTarget && Input.GetMouseButtonDown(1))
            {
                CancelTargeting();
            }
        }

        public void StartBattle(UnitData pData, List<UnitData> eDataList)
        {
            state = BattleState.Start;

            if (bgmSource != null && battleBGM != null)
            {
                bgmSource.clip = battleBGM;
                bgmSource.loop = true;
                bgmSource.Play();
            }

            // Initialize Player
            playerUnit.Initialize(pData);

            // Clear old enemies & old UI
            foreach (var e in activeEnemies)
            {
                if (e != null) Destroy(e.gameObject);
            }
            activeEnemies.Clear();

            if (enemyUIContainers != null)
            {
                foreach (var container in enemyUIContainers)
                {
                    if (container != null)
                    {
                        foreach (Transform child in container)
                        {
                            Destroy(child.gameObject);
                        }
                    }
                }
            }

            // Spawn Enemies
            for (int i = 0; i < eDataList.Count; i++)
            {
                if (i >= enemySpawnPoints.Length) break;

                // Spawn Enemy Unit
                GameObject eGo = Instantiate(enemyPrefab, enemySpawnPoints[i]);
                Unit eUnit = eGo.GetComponent<Unit>();
                eUnit.Initialize(eDataList[i]);
                activeEnemies.Add(eUnit);

                // Spawn Enemy UI
                if (enemyUIPrefab != null && enemyUIContainers != null && i < enemyUIContainers.Length)
                {
                    // Parameter false pada Instantiate memaksa Unity menjaga posisi lokal (tidak berusaha menyesuaikan posisi World)
                    GameObject uiGo = Instantiate(enemyUIPrefab, enemyUIContainers[i], false);

                    // Pastikan RectTransform berada tepat di 0
                    RectTransform rect = uiGo.GetComponent<RectTransform>();
                    if (rect != null)
                    {
                        rect.anchoredPosition = Vector2.zero;
                        rect.localScale = Vector3.one;
                    }
                    else
                    {
                        uiGo.transform.localPosition = Vector3.zero;
                    }

                    TechTest.UI.EnemyUI eUI = uiGo.GetComponent<TechTest.UI.EnemyUI>();
                    if (eUI != null) eUI.Setup(eUnit);
                }
            }

            maxEnergy = pData.maxEnergy;

            // Setup Deck
            deckManager.InitializeBattle();

            StartCoroutine(PlayerTurn());
        }

        private IEnumerator PlayerTurn()
        {
            state = BattleState.PlayerTurn;

            playerUnit.ClearBlock();
            currentEnergy = maxEnergy;

            // Determine Enemy Intents (Random for all)
            foreach (var enemy in activeEnemies)
            {
                // Future expansion: randomize enemy intent per enemy
            }

            deckManager.DrawCards(playerUnit.unitData.drawPerTurn);

            Debug.Log("Player Turn Started. Energy: " + currentEnergy);
            yield return null;
        }

        public void OnEndTurnButton()
        {
            if (state != BattleState.PlayerTurn && state != BattleState.ChoosingTarget) return;

            if (state == BattleState.ChoosingTarget) CancelTargeting();

            if (sfxSource != null && sfxButtonClick != null) sfxSource.PlayOneShot(sfxButtonClick);

            deckManager.DiscardHand();
            StartCoroutine(EnemyTurn());
        }

        private IEnumerator EnemyTurn()
        {
            state = BattleState.EnemyTurn;
            Debug.Log("Enemy Turn Started.");

            yield return new WaitForSeconds(0.5f);

            // Enemies attack one by one
            foreach (var enemy in activeEnemies)
            {
                if (enemy.isDead) continue;

                enemy.ClearBlock();

                // Lunge Animation for Enemy
                int dmg = Random.Range(4, 9);
                Debug.Log($"{enemy.unitData.unitName} attacks for {dmg} damage!");

                bool actionFinished = false;
                StartCoroutine(Routine_ExecuteAction(enemy, playerUnit, false, dmg, 0, () =>
                {
                    actionFinished = true;
                }));

                while (!actionFinished)
                {
                    yield return null;
                }

                if (playerUnit.isDead)
                {
                    state = BattleState.Lost;
                    EndBattle(false);
                    yield break; // Stop loop if player is dead
                }
            }

            StartCoroutine(PlayerTurn());
        }

        public bool CanPlayCard(CardData card)
        {
            return currentEnergy >= card.energyCost;
        }

        public void BeginTargeting(CardData card)
        {
            if (!CanPlayCard(card) || state != BattleState.PlayerTurn)
            {
                Debug.Log("Cannot play card! Not enough energy or wrong phase.");
                return;
            }

            if (sfxSource != null && sfxButtonClick != null) sfxSource.PlayOneShot(sfxButtonClick);

            pendingCard = card;
            state = BattleState.ChoosingTarget;
            Debug.Log($"Choosing target for {card.cardName}. Right click to cancel.");
        }

        public void CancelTargeting()
        {
            if (sfxSource != null && sfxButtonClick != null) sfxSource.PlayOneShot(sfxButtonClick);
            pendingCard = null;
            state = BattleState.PlayerTurn;
            Debug.Log("Targeting cancelled.");
        }

        public bool IsTargetEligible(Unit target)
        {
            if (state != BattleState.ChoosingTarget || pendingCard == null) return false;

            if (pendingCard.targetType == TargetType.Self)
                return target.unitData.isPlayer;
            else
                return !target.unitData.isPlayer; // SingleEnemy atau AllEnemies harus klik musuh
        }

        public void HoverTarget(Unit target)
        {
            if (state != BattleState.ChoosingTarget || pendingCard == null) return;
            if (!IsTargetEligible(target)) return;

            if (pendingCard.targetType == TargetType.AllEnemies)
            {
                foreach (var enemy in activeEnemies)
                {
                    if (enemy != null && !enemy.isDead) enemy.Flash(true);
                }
            }
            else
            {
                target.Flash(true);
            }
        }

        public void ClearHover(Unit target)
        {
            if (target != null) target.Flash(false);

            if (state == BattleState.ChoosingTarget && pendingCard != null && pendingCard.targetType == TargetType.AllEnemies)
            {
                foreach (var enemy in activeEnemies)
                {
                    if (enemy != null) enemy.Flash(false);
                }
            }
        }

        public void ExecuteTargetedCard(Unit target)
        {
            if (state != BattleState.ChoosingTarget || pendingCard == null) return;
            if (!IsTargetEligible(target)) return;

            state = BattleState.ExecutingCard; // Kunci state agar animasi jalan dulu
            currentEnergy -= pendingCard.energyCost;

            // Simpan referensi kartu untuk dikirim ke coroutine
            CardData cardToPlay = pendingCard;
            pendingCard = null;

            bool isHeal = false;
            int totalDamage = 0;
            int totalBlock = 0;
            int totalDraw = 0;

            foreach (var effect in cardToPlay.effects)
            {
                if (effect.effectType == EffectType.Damage) totalDamage += effect.value;
                if (effect.effectType == EffectType.Block) totalBlock += effect.value;
                if (effect.effectType == EffectType.Heal) isHeal = true;
                if (effect.effectType == EffectType.DrawCard) totalDraw += effect.value;
            }

            StartCoroutine(Routine_ExecuteAction(playerUnit, target, isHeal, totalDamage, totalBlock, () => {
                // Apply remaining card logic after lunge impact
                if (cardToPlay.targetType == TargetType.AllEnemies && totalDamage > 0)
                {
                    foreach(var enemy in activeEnemies)
                    {
                        if (enemy != target && enemy != null && !enemy.isDead)
                        {
                            enemy.TakeDamage(totalDamage);
                            enemy.Flash(true); // Short flash for other enemies
                            StartCoroutine(ClearFlash(enemy, 0.1f));
                        }
                    }
                }

                if (totalDraw > 0) deckManager.DrawCards(totalDraw);
                deckManager.PlayCard(cardToPlay);

                CheckWinCondition();

                if (state != BattleState.Won)
                {
                    state = BattleState.PlayerTurn;
                }
            }));
        }

        private IEnumerator ClearFlash(Unit unit, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (unit != null) unit.Flash(false);
        }

        public IEnumerator Routine_ExecuteAction(Unit caster, Unit target, bool isHeal, int damage, int block, System.Action onComplete)
        {
            if (caster == null || target == null)
            {
                onComplete?.Invoke();
                yield break;
            }

            Vector3 originalPos = caster.transform.position;
            Vector3 targetPos = target.transform.position;
            Vector3 direction = (targetPos - originalPos).normalized;
            Vector3 lungePos = originalPos + direction * 1f;

            // 1. Lunge Forward
            if (sfxSource != null && sfxLunge != null) sfxSource.PlayOneShot(sfxLunge);

            float t = 0;
            while (t < 0.15f)
            {
                t += Time.deltaTime;
                caster.transform.position = Vector3.Lerp(originalPos, lungePos, t / 0.15f);
                yield return null;
            }

            // 2. Impact Effects
            if (sfxSource != null)
            {
                if (isHeal || block > 0) sfxSource.PlayOneShot(sfxHeal);
                else if (damage > 0) sfxSource.PlayOneShot(sfxHit);
            }

            StartCoroutine(CameraShakeRoutine(0.15f, 0.3f));

            if (target != caster && target != null)
            {
                target.Flash(true);
            }

            // 3. Apply Actual Logic
            if (damage > 0) target.TakeDamage(damage);
            if (block > 0) target.AddBlock(block); // Normally cast on self, but allowed here

            yield return new WaitForSeconds(0.1f);
            if (target != caster && target != null) target.Flash(false);

            // 4. Return to Position
            t = 0;
            while (t < 0.15f)
            {
                t += Time.deltaTime;
                if (caster != null) caster.transform.position = Vector3.Lerp(lungePos, originalPos, t / 0.15f);
                yield return null;
            }

            if (caster != null) caster.transform.position = originalPos;
            yield return new WaitForSeconds(0.1f);

            onComplete?.Invoke();
        }

        public IEnumerator CameraShakeRoutine(float duration, float magnitude)
        {
            Camera mainCam = Camera.main;
            if (mainCam == null) yield break;

            Vector3 originalPos = mainCam.transform.localPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;

                mainCam.transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

                elapsed += Time.deltaTime;
                yield return null;
            }

            mainCam.transform.localPosition = originalPos;
        }

        private void CheckWinCondition()
        {
            // Remove dead enemies
            for (int i = activeEnemies.Count - 1; i >= 0; i--)
            {
                if (activeEnemies[i].isDead)
                {
                    Destroy(activeEnemies[i].gameObject);
                    activeEnemies.RemoveAt(i);
                }
            }

            if (activeEnemies.Count == 0)
            {
                state = BattleState.Won;
                EndBattle(true);
            }
        }

        private void EndBattle(bool isWin)
        {
            if (bgmSource != null) bgmSource.Stop();

            if (isWin)
            {
                Debug.Log("Battle Won!");
                if (bgmSource != null && winBGM != null) bgmSource.PlayOneShot(winBGM);
                TechTest.Core.RunManager.Instance.EndBattle(true, playerUnit.currentHP);
            }
            else
            {
                Debug.Log("Game Over! Player died.");
                if (bgmSource != null && loseBGM != null) bgmSource.PlayOneShot(loseBGM);
                TechTest.Core.RunManager.Instance.EndBattle(false, 0);
            }
        }
    }
}
