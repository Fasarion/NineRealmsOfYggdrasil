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
        
        public ChargeState chargeState = ChargeState.None;

        
        // Weapons
        private List<WeaponBehaviour> weapons;
        private WeaponBehaviour activeWeapon;
        public WeaponType CurrentWeaponType => activeWeapon.WeaponType;
        private Dictionary<WeaponBehaviour, Transform> weaponParents = new ();

        // Attack Data
        private AttackType currentAttackType { get;  set; }
        private int currentCombo = 0;

       // public bool isAttacking;
        
        // Animator parameters
      //  private string movingParameterName = "Moving";
        private string movingParameterName = "movementSpeed";
        private string bufferAttackParameterName = "BufferedAttack";
        private string isAttackingParameterName = "IsAttacking";
        private string attackReleasedParameterName = "AttackReleased";
        private string currentWeaponParameterName = "CurrentWeapon";
        private string currentAttackParameterName = "CurrentAttack";
        private string weaponIdParameterName = "weaponID";

        
        // Animation Events
        public UnityAction<AttackData> OnActiveAttackStart;
        public UnityAction<AttackData> OnActiveAttackStop;
        
        public UnityAction<AttackData> OnPassiveAttackStart;
        public UnityAction<AttackData> OnPassiveAttackStop;
        
        public UnityAction<WeaponType> OnWeaponActive;
        public UnityAction<WeaponType> OnWeaponPassive;
        
        public UnityAction<AttackData> OnSpecialCharge;
        public UnityAction<AttackData> OnUltimatePrepare;
        
        public void UpdateAttackAnimation(AttackType type, bool setTrue)
        {
           if (setTrue) currentAttackType = type;
           playerAnimator.SetBool(GetActiveAttackAnimationName(type), setTrue);
        }

        
        // New Events
        public void Begin(int combo)
        {
            currentCombo = combo;
            OnActiveAttackStart?.Invoke(GetActiveAttackData());
        }
        
        public void Stop(int combo)
        {
            currentCombo = combo;
            OnActiveAttackStop?.Invoke(GetActiveAttackData()); 
        }
        
        public void TurnOff()
        {
            string attackParam = GetActiveAttackAnimationName(currentAttackType);
            if (attackParam == "") return;
            
            playerAnimator.SetBool(attackParam, false);
            
          //  isAttacking = false;
          //  StartCoroutine(ResetBufferNextFrame());
        }

        // Events called from animator. NOTE: DO NOT REMOVE BECAUSE THEY ARE GREYED OUT IN EDITOR
        public void StartActiveAttackEvent(int combo = 0)
        {
            currentCombo = combo;
            OnActiveAttackStart?.Invoke(GetActiveAttackData());
        } 
        
        public void StopActiveAttackEvent(int combo = 0)
        {
            currentCombo = combo;
            OnActiveAttackStop?.Invoke(GetActiveAttackData()); 
        }

        public void SetIsAttackingEvent()
        {
           // isAttacking = true;
        }

        public void SetCurrentCombo(int combo)
        {
          //  isAttacking = true;
            currentCombo = combo;
        }

        public void FinishActiveAttackAnimationEvent()
        {
        //    isAttacking = false;
         //   StartCoroutine(ResetBufferNextFrame());
        }

        private IEnumerator ResetBufferNextFrame()
        {
            yield return new WaitForEndOfFrame();
            playerAnimator.SetInteger(bufferAttackParameterName, -1);
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
            { new WeaponAttackPair(WeaponType.Sword, AttackType.Special), "SwordSpecialWindup" },
            { new WeaponAttackPair(WeaponType.Sword, AttackType.Ultimate), "SwordUltimate" },
            
            // Hammer Animations
            { new WeaponAttackPair(WeaponType.Hammer, AttackType.Normal), "HammerNormal" },
            { new WeaponAttackPair(WeaponType.Hammer, AttackType.Special), "HammerSpecialWindUp" },
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
                AttackPoint = activeWeapon.AttackPoint,
                ComboCounter = currentCombo,
            };

            return attackData;
        }

        private void Awake()
        {
            Instance = this;

            playerAudio = gameObject.GetComponent<PlayerAudioBehaviour>();
            playerAnimator = gameObject.GetComponent<Animator>();
        }

        private void OnDisable()
        {
            foreach (var weapon in weapons)
            {
                UnsubscribeFromPassiveEvents(weapon);
            }
        }

        public void SetupWeapons()
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

            int weaponID = (int) CurrentWeaponType - 1;
            playerAnimator.SetInteger(weaponIdParameterName, weaponID);
            
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
        public bool PerformNormalAttack()
        {
            return TryPerformAttack(AttackType.Normal);
        }
        
        /// <summary>
        /// Function to be called when a special attack is about to be performed. Called from DOTS after the correct input
        /// is registered.
        /// </summary>
        public bool StartChargingSpecial()
        {
            bool canAttack = TryPerformAttack(AttackType.Special);

            if (canAttack)
            {
                playerAnimator.SetBool(attackReleasedParameterName, false);
                OnSpecialCharge?.Invoke(GetActiveAttackData());
            }

            return canAttack;
        }
        
        /// <summary>
        /// Function to be called when a special attack is about to be performed. Called from DOTS after the correct input
        /// is registered.
        /// </summary>
        public bool PerformUltimateAttack()
        {
            return TryPerformAttack(AttackType.Ultimate);
        }

        private bool TryPerformAttack(AttackType type)
        {
            if (!activeWeapon)
            {
                Debug.LogWarning("No active weapon, can't perform attack.");
                return false;
            }
            
            currentAttackType = type;
            UpdateAnimatorAttackParameters();
            playerAudio.PlayWeaponSwingAudio((int)CurrentWeaponType, (int)currentAttackType);
            return true;
        }

        private void UpdateAnimatorAttackParameters()
        {
            bool animationNameExists = GetActiveAttackAnimationName(out string name);
            if (animationNameExists)
            {
                string attackAnimationName = GetActiveAttackAnimationName(currentAttackType);
                
                playerAnimator.SetBool(attackAnimationName, true);
            }
            else
            {
               // isAttacking = false;
                Debug.Log($"No animation found for weapon attack pair {CurrentWeaponType}, {currentAttackType}");
            }
        }

        private bool GetActiveAttackAnimationName(out string animationName)
        {
            animationName = "";
            
            WeaponAttackPair pair = new WeaponAttackPair(CurrentWeaponType, currentAttackType);
            if (weaponAttackAnimationNames.ContainsKey(pair))
            {
                animationName = weaponAttackAnimationNames[pair];
                return true;
            }

            return false;
        }
        
        private string GetActiveAttackAnimationName(AttackType attackType)
        {
            switch (attackType)
            {
                case AttackType.Normal:
                    return "attackNormal";
                
                case AttackType.Special:
                    return "attackSpecial";
                
                case AttackType.Ultimate:
                    return "attackUltimate";
            }

            return "";
        }

        public void UpdateMovementParameter(float movementT)
        {
            if (!playerAnimator) return;
            
            //playerAnimator.SetBool(movingParameterName, playerIsMoving);
            playerAnimator.SetFloat(movingParameterName, movementT);
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

        public void ReleaseSpecial()
        {
//            playerAnimator.SetBool(attackReleasedParameterName, true);

            string specialAtk = GetActiveAttackAnimationName(AttackType.Special);
            playerAnimator.SetBool(specialAtk, false);
        }

        public void PrepareUltimateAttack()
        {
            currentAttackType = AttackType.Ultimate;
            OnUltimatePrepare?.Invoke(GetActiveAttackData());

            playerAnimator.SetBool("attackUltimate", true);
        }

        public void ResetActiveWeapon()
        {
            activeWeapon.SetParent(activeSlot);
            playerAnimator.SetTrigger("hammerReturn");
        }

        public void SetCharge(ChargeState state)
        {
            chargeState = state;
        }
    }
}

public enum ChargeState
{
    None,
    Start,
    Ongoing,
    Stop
}