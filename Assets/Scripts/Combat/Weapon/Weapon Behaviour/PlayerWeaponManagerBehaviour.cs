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
        
        [Header("Attack Buffer")]
        [SerializeField] float attackBufferTime = 0.2f;

        public BusyAttackInfo busyAttackInfo;
        public ChargeState chargeState = ChargeState.None;

        // Weapons
        private List<WeaponBehaviour> weapons;
        private WeaponBehaviour activeWeapon;
        public WeaponType CurrentWeaponType => activeWeapon.WeaponType;
        private Dictionary<WeaponBehaviour, Transform> weaponParents = new ();

        // Attack Data
        private AttackType currentAttackType { get;  set; }
        private int currentCombo = 0;
        
        // Animator parameters
        private string movingParameterName = "movementSpeed";
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

        float timeOfLastAttackHold;
        float timeSinceLastAttackHold;
        

        public void UpdateAttackAnimation(AttackType type, bool setBool)
        {
           if (setBool) currentAttackType = type;
           
           playerAnimator.SetBool(GetActiveAttackAnimationName(type), setBool);

           if (setBool)
           {
               timeOfLastAttackHold = Time.time;
           }

           timeSinceLastAttackHold = Time.time - timeOfLastAttackHold;
           if (timeSinceLastAttackHold < attackBufferTime && type == currentAttackType)
           {
               playerAnimator.SetBool(GetActiveAttackAnimationName(type), true);
           }
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
        }

        public void StartActiveAttackEvent(int combo = 0)
        {
            Debug.LogError("OLD METHOD CALLED VIA ANIMATION EVENT, DELETE EVENT.");

            currentCombo = combo;
            OnActiveAttackStart?.Invoke(GetActiveAttackData());
        } 
        
        public void StopActiveAttackEvent(int combo = 0)
        {
            Debug.LogError("OLD METHOD CALLED VIA ANIMATION EVENT, DELETE EVENT.");

            currentCombo = combo;
            OnActiveAttackStop?.Invoke(GetActiveAttackData()); 
        }

        public void SetIsAttackingEvent()
        {
            Debug.LogError("OLD METHOD CALLED VIA ANIMATION EVENT, DELETE EVENT.");
        }

        public void SetCurrentCombo(int combo)
        {
            Debug.LogError("OLD METHOD CALLED VIA ANIMATION EVENT, DELETE EVENT.");

            currentCombo = combo;
        }

        public void FinishActiveAttackAnimationEvent()
        {
            Debug.LogError("OLD METHOD CALLED VIA ANIMATION EVENT, DELETE EVENT.");
        }
        
        
        public void SetNotBusy()
        {
            busyAttackInfo = new BusyAttackInfo(false, WeaponType.None, AttackType.None);
            CallUpdateBusyEvent();
        }
        
        public void SetBusy(AttackType attackType, WeaponType weaponType)
        {
            busyAttackInfo = new BusyAttackInfo(true, weaponType, attackType);
            CallUpdateBusyEvent();
        }

        private void CallUpdateBusyEvent()
        {
            EventManager.OnBusyUpdate?.Invoke(busyAttackInfo);
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
            string attackAnimationName = GetActiveAttackAnimationName(currentAttackType);
            playerAnimator.SetBool(attackAnimationName, true);
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
            string specialAtk = GetActiveAttackAnimationName(AttackType.Special);
            playerAnimator.SetBool(specialAtk, false);
        }

        public void PrepareUltimateAttack()
        {
            currentAttackType = AttackType.Ultimate;
            OnUltimatePrepare?.Invoke(GetActiveAttackData());

            //playerAnimator.SetBool("startUltimate", true);
            playerAnimator.SetTrigger("startUltimateTrigger");
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

        public void ResetUltimatePrepare()
        {
            playerAnimator.SetBool("startUltimate", false);
            playerAnimator.ResetTrigger("startUltimateTrigger");
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