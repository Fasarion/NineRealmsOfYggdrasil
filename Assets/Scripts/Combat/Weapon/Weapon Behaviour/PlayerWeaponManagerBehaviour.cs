using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Patrik
{
    // TODO: extract PlayerManager, PlayerWeaponBehaviour, PlayerAnimationBehaviour
    public class PlayerWeaponManagerBehaviour : MonoBehaviour
    {
        public static PlayerWeaponManagerBehaviour Instance { get; private set; }

        [Header("Weapon")]
        [SerializeField] private WeaponType startWeaponType = WeaponType.Sword;

        [Header("Animation")]
        [SerializeField] private Animator playerAnimator;
        
        [Header("Audio")]
        [SerializeField] private PlayerAudioBehaviour playerAudio;

        [Header("Weapon Slots")]
        [SerializeField] private Transform activeSlot;
        [SerializeField] private List<Transform> passiveSlots = new ();
        
        // Weapons
        private List<WeaponBehaviour> weapons;
        private WeaponBehaviour activeWeapon;
        private WeaponType currentWeaponType => activeWeapon.WeaponType;
        int CurrentWeaponTypeInt => (int)currentWeaponType;
        private Dictionary<WeaponBehaviour, Transform> weaponParents = new ();

        // Attack Data
        private AttackType currentAttackType { get;  set; }
        int CurrentAttackTypeInt => (int)currentAttackType;
        private bool isAttacking;
        private bool isResettingAttackFlag;
        
        // Animator parameters
        // private string attackAnimationName = "Attack";
        // private string activeWeaponParameterName = "ActiveWeapon";
        // private string currentAttackParameterName = "CurrentAttack";

        private string movingParameterName = "Moving";
        private string bufferAttackParamterName = "AttackBuffered";
        private string isAttackingParameterName = "IsAttacking";
        
        // Animation Events
        public UnityAction<AttackData> OnActiveAttackStart;
        public UnityAction<AttackData> OnActiveAttackStop;
        
        public UnityAction<AttackData> OnPassiveAttackStart;
        public UnityAction<AttackData> OnPassiveAttackStop;
        
        public UnityAction<WeaponType> OnWeaponActive;
        public UnityAction<WeaponType> OnWeaponPassive;


        // Events called from animator. NOTE: DO NOT REMOVE BECAUSE THEY ARE GREYED OUT IN EDITOR
        public void StartActiveAttackEvent()
        {
            OnActiveAttackStart?.Invoke(GetActiveAttackData());
        } 
        
        public void StopActiveAttackEvent()
        {
            OnActiveAttackStop?.Invoke(GetActiveAttackData()); 
        }

        public void SetIsAttackingEvent()
        {
            isAttacking = true;
        }

        public void FinishActiveAttackAnimationEvent()
        {
            Debug.Log("Attack Finished.");
            
            isAttacking = false;

            StartCoroutine(ResetBufferNextFrame());
        }

        private IEnumerator ResetBufferNextFrame()
        {
            yield return new WaitForEndOfFrame();
            playerAnimator.SetBool(bufferAttackParamterName, false);
        }

        private void Update()
        {
            playerAnimator.SetBool(isAttackingParameterName, isAttacking);
        }

        struct WeaponAttackPair
        {
            public WeaponAttackPair(WeaponType weapon, AttackType attack)
            {
                Weapon = weapon;
                Attack = attack;
            }
            
            WeaponType Weapon;
            AttackType Attack;
        }
        
        /// <summary>
        /// Dictionary containing the names for the attack animations. Retrieved using weapon name and attack name.
        /// </summary>
        private static Dictionary<WeaponAttackPair, string> weaponAttackAnimationNames = new ()
        {
            // Sword Animations
            { new WeaponAttackPair(WeaponType.Sword, AttackType.Normal), "SwordNormal" },
            { new WeaponAttackPair(WeaponType.Sword, AttackType.Special), "SwordSpecial" },
            { new WeaponAttackPair(WeaponType.Sword, AttackType.Ultimate), "SwordUltimate" },
            
            // Hammer Animations
            { new WeaponAttackPair(WeaponType.Hammer, AttackType.Normal), "HammerNormal" },
            { new WeaponAttackPair(WeaponType.Hammer, AttackType.Special), "HammerSpecial" },
            { new WeaponAttackPair(WeaponType.Hammer, AttackType.Ultimate), "HammerUltimate" },
            
            // TODO: Add animation names for other attacks
        };

        /// <summary>
        /// Attack Data from current attack. Informs DOTS which weapon was attacking, which attack type was used and
        /// at which point the attack occured.
        /// </summary>
        /// <returns></returns>
        private AttackData GetActiveAttackData() 
        {
            var attackData = new AttackData
            {
                AttackType = currentAttackType,
                WeaponType = activeWeapon.WeaponType,
                AttackPoint = activeWeapon.AttackPoint
            };

            return attackData;
        }

        private void Awake()
        {
            Instance = this;

            playerAudio = gameObject.GetComponent<PlayerAudioBehaviour>();
            playerAnimator = gameObject.GetComponent<Animator>();
        }

        private void OnEnable()
        {
            // wait a few frames to setup weapons to make sure they have spawned from the DOTS side
            Invoke(nameof(SetupWeapons), 0.1f);
        }

        private void OnDisable()
        {
            foreach (var weapon in weapons)
            {
                UnsubscribeFromPassiveEvents(weapon);
            }
        }

        private void SetupWeapons()
        {
            var foundWeapons = FindObjectsOfType<WeaponBehaviour>().ToList();
            weapons = new List<WeaponBehaviour>();

            int passiveSlotCounter = 0;
            
            foreach (var weapon in foundWeapons)
            {
                SubscribeToPassiveEvents(weapon);
                
                // Handle active weapon
                if (weapon.WeaponType == startWeaponType)
                {
                    MakeWeaponActive(weapon);
                    weapons.Add(activeWeapon);
                    
                    EventManager.OnSetupWeapon?.Invoke(weapon, true);
                    
                    continue;
                }
                
                // Handle passive weapon
                Transform passiveParent = passiveSlotCounter <= passiveSlots.Count
                    ? passiveSlots[passiveSlotCounter]
                    : activeSlot;
                MakeWeaponPassive(weapon, passiveParent);

                weapons.Add(weapon);
                passiveSlotCounter++;
                EventManager.OnSetupWeapon?.Invoke(weapon, false);
            }
        }
        
        private void MakeWeaponActive(WeaponBehaviour weapon)
        {
            activeWeapon = weapon;
            weapon.MakeActive(activeSlot);
            weaponParents[weapon] = activeSlot;
            
            OnWeaponActive?.Invoke(weapon.WeaponType);
        }
        
        private void MakeWeaponPassive(WeaponBehaviour weapon, Transform passiveParent)
        {
            weapon.MakePassive(passiveParent);
            weaponParents[weapon] = passiveParent;
            
            OnWeaponPassive?.Invoke(weapon.WeaponType);
        }
        
        private void SubscribeToPassiveEvents(WeaponBehaviour weapon)
        {
            weapon.OnPassiveAttackStart += StartPassiveAttack;
            weapon.OnPassiveAttackStop += StopPassiveAttack;
        }
        
        private void UnsubscribeFromPassiveEvents(WeaponBehaviour weapon)
        {
            weapon.OnPassiveAttackStart -= StartPassiveAttack;
            weapon.OnPassiveAttackStop -= StopPassiveAttack;
        }

        private void StartPassiveAttack(WeaponBehaviour weapon)
        {
            OnPassiveAttackStart?.Invoke(GetPassiveAttackData(weapon));
        }
        
        private void StopPassiveAttack(WeaponBehaviour weapon)
        {
            OnPassiveAttackStop?.Invoke(GetPassiveAttackData(weapon));
        }

        private static AttackData GetPassiveAttackData(WeaponBehaviour weapon)
        {
            var attackData = new AttackData
            {
                AttackType = AttackType.Passive,
                WeaponType = weapon.WeaponType,
                AttackPoint = weapon.AttackPoint
            };

            return attackData;
        }

        /// <summary>
        /// Function to be called when a normal attack is about to be performed. Called from DOTS after the correct input
        /// is registered.
        /// </summary>
        public void PerformNormalAttack()
        {
            TryPerformAttack(AttackType.Normal);
        }
        
        /// <summary>
        /// Function to be called when a special attack is about to be performed. Called from DOTS after the correct input
        /// is registered.
        /// </summary>
        public void PerformSpecialAttack()
        {
            TryPerformAttack(AttackType.Special);
        }
        
        /// <summary>
        /// Function to be called when a special attack is about to be performed. Called from DOTS after the correct input
        /// is registered.
        /// </summary>
        public void PerformUltimateAttack()
        {
            TryPerformAttack(AttackType.Ultimate);
        }

        private void TryPerformAttack(AttackType type)
        {
            if (!activeWeapon)
            {
                Debug.LogWarning("No active weapon, can't perform attack.");
                return;
            }
            
            if (isAttacking)
            {
                // set attack buffer
                playerAnimator.SetBool(bufferAttackParamterName, true);

                if (!isResettingAttackFlag) StartCoroutine(ResetAttackFlag(1f));
                return;
            }

            isAttacking = true;
            currentAttackType = type;
            UpdateAnimatorAttackParameters();
            playerAudio.PlayWeaponSwingAudio(CurrentWeaponTypeInt, CurrentAttackTypeInt);
        }

        private IEnumerator ResetAttackFlag(float time)
        {
            isResettingAttackFlag = true;
            yield return new WaitForSeconds(time);
            isAttacking = false;
            isResettingAttackFlag = false;
        }

        private void UpdateAnimatorAttackParameters()
        {
            bool animationNameExists = GetActiveAttackAnimationName(out string name);
            if (animationNameExists)
            {
                playerAnimator.Play(name);
            }
            else
            {
                isAttacking = false;
                Debug.Log($"No animation found for weapon attack pair {currentWeaponType}, {currentAttackType}");
            }
            
        }

        private bool GetActiveAttackAnimationName(out string animationName)
        {
            animationName = "";
            
            WeaponAttackPair pair = new WeaponAttackPair(currentWeaponType, currentAttackType);
            if (weaponAttackAnimationNames.ContainsKey(pair))
            {
                animationName = weaponAttackAnimationNames[pair];
                return true;
            }

            return false;
        }

        public void UpdateMovementParameter(bool playerIsMoving)
        {
            if (!playerAnimator) return;
            
            playerAnimator.SetBool(movingParameterName, playerIsMoving);
        }

        public void SwitchWeapon(int weaponNumber)
        {
            if (weaponNumber > weapons.Count)
            {
                Debug.LogWarning($"Can't switch to weapon {weaponNumber} because there are only {weapons.Count} weapons.");
                return;
            }

            int numberInList = weaponNumber - 1;
            WeaponBehaviour newActiveWeapon = weapons[numberInList];
            WeaponBehaviour oldActiveWeapon = activeWeapon;
            if (newActiveWeapon == oldActiveWeapon)
            {
                return;
            }

            Transform newPassiveSlot = weaponParents[newActiveWeapon];

            // switching passive from active
            MakeWeaponPassive(oldActiveWeapon, newPassiveSlot);
            MakeWeaponActive(newActiveWeapon);
            EventManager.OnWeaponSwitch?.Invoke(newActiveWeapon);
        }
    }
}