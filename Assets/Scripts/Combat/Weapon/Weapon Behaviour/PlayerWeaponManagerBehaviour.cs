using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Patrik
{
    public struct WeaponSetupData
    {
        public bool Active;
        public WeaponType WeaponType;
        public int WeaponButtonIndex;
        public WeaponBehaviour WeaponBehaviour;
    }
    
    // TODO: extract PlayerManager, PlayerWeaponBehaviour, PlayerAnimationBehaviour
    public class PlayerWeaponManagerBehaviour : MonoBehaviour
    {
        public static PlayerWeaponManagerBehaviour Instance { get; private set; }

        [Header("Weapon")]
        [SerializeField] private List<WeaponType> startingWeaponTypes = new List<WeaponType>();

        [Header("Animation")]
        [SerializeField] private Animator playerAnimator;
        
        [Header("Weapon Slots")]
        [SerializeField] private Transform activeSlot;
        [SerializeField] private List<Transform> passiveSlots = new ();
        
        [Header("Attack Buffer")]
        [SerializeField] float attackBufferTime = 0.2f;

        [Range(1,3)]
        public int currentlyAllowedWeapons = 1;

        BusyAttackInfo busyAttackInfo;
        public ChargeState chargeState { get; private set; }= ChargeState.None;

        // Weapons
        //This list should not be modified after setup
        private List<WeaponBehaviour> startingWeapons;
        //Use this list instead as it changes at runtime.
        private List<WeaponSetupData> weaponDataList;
        private WeaponBehaviour activeWeapon;
        private WeaponSetupData activeWeaponData;
        public WeaponType CurrentWeaponType => activeWeapon.WeaponType;
        private Dictionary<WeaponBehaviour, Transform> weaponParents = new ();
        private Dictionary<WeaponType, int> weaponIdMap;

        // Attack Data
        private AttackType currentAttackType { get;  set; }
        private int currentCombo = 0;
        
        // Animator parameters
        private string weaponIdParameterName = "weaponID";

        float timeOfLastAttackHold;
        float timeSinceLastAttackHold;

        private bool updateWeaponCountAtRuntime;
        private int previouslyAllowedWeapons;

        private List<WeaponSetupData> previousWeaponDataList;
        private List<WeaponBehaviour> previousWeapons;

        private void Awake()
        {
            Instance = this;
            playerAnimator = gameObject.GetComponent<Animator>();
            
            SetWeaponIds();
            updateWeaponCountAtRuntime = false;
        }

        private void OnValidate()
        {
            SetWeaponIds();
        }

        private void SetWeaponIds()
        {
            weaponIdMap = new Dictionary<WeaponType, int>();

            int nextId = 1;
            
            foreach (var pair in startingWeaponTypes)
            {
                weaponIdMap[pair] = nextId++;
            }
        }

        private void OnEnable()
        {
            EventManager.OnUpdateAttackAnimation += UpdateAttackAnimation;
            EventManager.OnDashInput += Dash;
            EventManager.OnWeaponCountSet += OnNewWeaponCountSet;
        }

        private void OnDisable()
        {
            foreach (var weapon in weaponDataList)
            {
                UnsubscribeFromPassiveEvents(weapon.WeaponBehaviour);
            }
            
            EventManager.OnUpdateAttackAnimation -= UpdateAttackAnimation;
            EventManager.OnDashInput -= Dash;
            EventManager.OnWeaponCountSet -= OnNewWeaponCountSet;
        }

        public void OnNewWeaponCountSet(int newValue)
        {
            previouslyAllowedWeapons = currentlyAllowedWeapons;
            currentlyAllowedWeapons = newValue;
            updateWeaponCountAtRuntime = true;
            previousWeapons = startingWeapons;
            previousWeaponDataList = weaponDataList;
            UpdateWeapons();
            //SetupWeapons();
        }

        private void UpdateAttackAnimation(AttackType attackType, bool shouldAttack)
        {
           if (shouldAttack) currentAttackType = attackType;
           
           playerAnimator.SetBool(GetActiveAttackAnimationName(attackType), shouldAttack);
           
           HandleAttackBuffer(attackType, shouldAttack);
        }

        private void HandleAttackBuffer(AttackType type, bool setBool)
        {
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

        public void Begin(int combo)
        {
            currentCombo = combo;
            EventManager.OnActiveAttackStart?.Invoke(GetActiveAttackData());
        }
        
        public void Stop(int combo)
        {
            currentCombo = combo;
            EventManager.OnActiveAttackStop?.Invoke(GetActiveAttackData());
        }
        
        public void TurnOff()
        {
            string attackParam = GetActiveAttackAnimationName(currentAttackType);
            if (attackParam == "") return;
            
            playerAnimator.SetBool(attackParam, false);
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

        public void UpdateWeapons()
        {
            var foundWeapons = FindObjectsOfType<WeaponBehaviour>().ToList();
            foundWeapons = foundWeapons.OrderBy(weapon => weaponIdMap[weapon.WeaponType]).ToList();
            
            startingWeapons = new List<WeaponBehaviour>();
            weaponDataList = new List<WeaponSetupData>();
            int passiveSlotCounter = 0;

            bool hasSetStarter = false;

            int buttonIndex = 0;
            //for (int i = 0; i < previousWeaponDataList.Count; i++)
            //{
            //buttonIndex++;
            
            //SubscribeToPassiveEvents(foundWeapons[i]);
            
            // Handle active weapon
            for (int i = 0; i < previousWeaponDataList.Count; i++)
            {
                buttonIndex++;
        
                SubscribeToPassiveEvents(foundWeapons[i]);
                
                if (!hasSetStarter)
                {
                    MakeWeaponActive(activeWeaponData);
                    startingWeapons.Add(activeWeapon);
            
                    weaponDataList.Add(activeWeaponData);

                    EventManager.OnSetupWeapon?.Invoke(activeWeaponData);

                    hasSetStarter = true;
                    continue;
                }
                // Handle passive weapon
                Transform passiveParent = passiveSlotCounter <= passiveSlots.Count
                    ? passiveSlots[passiveSlotCounter]
                    : activeSlot;
            
                startingWeapons.Add(previousWeaponDataList[i].WeaponBehaviour);
                weaponDataList.Add(previousWeaponDataList[i]);
                MakeWeaponPassive(previousWeaponDataList[i], passiveParent);
            
                passiveSlotCounter++;
            
                EventManager.OnSetupWeapon?.Invoke(previousWeaponDataList[i]);
                
            }

            for (int i = previousWeaponDataList.Count; i < currentlyAllowedWeapons; i++)
            {
                Transform passiveParent = passiveSlotCounter <= passiveSlots.Count
                    ? passiveSlots[passiveSlotCounter]
                    : activeSlot;
        
                var passiveWeaponData = new WeaponSetupData
                {
                    Active = false,
                    WeaponType = foundWeapons[i].WeaponType,
                    WeaponButtonIndex = buttonIndex,
                    WeaponBehaviour = foundWeapons[i]
                };
                startingWeapons.Add(foundWeapons[i]);
                weaponDataList.Add(passiveWeaponData);
                MakeWeaponPassive(passiveWeaponData, passiveParent);
                
                passiveSlotCounter++;
            
                EventManager.OnSetupWeapon?.Invoke(weaponDataList[i]);
            }
            EventManager.OnAllWeaponsSetup?.Invoke(weaponDataList);

        }
        public void SetupWeapons()
        {
            var foundWeapons = FindObjectsOfType<WeaponBehaviour>().ToList();
            foundWeapons = foundWeapons.OrderBy(weapon => weaponIdMap[weapon.WeaponType]).ToList();
            
            startingWeapons = new List<WeaponBehaviour>();
            weaponDataList = new List<WeaponSetupData>();
            int passiveSlotCounter = 0;

            bool hasSetStarter = false;

            int buttonIndex = 0;
            //for (int i = 0; i < previousWeaponDataList.Count; i++)
            //{
                //buttonIndex++;
                
                //SubscribeToPassiveEvents(foundWeapons[i]);
                
                // Handle active weapon
            for(int i = 0; i < currentlyAllowedWeapons; i++)
            {
                buttonIndex++;
        
                SubscribeToPassiveEvents(foundWeapons[i]);
                if (!hasSetStarter)
                {
                    var activeWeaponData = new WeaponSetupData
                    {
                        Active = true,
                        WeaponType = foundWeapons[i].WeaponType,
                        WeaponButtonIndex = buttonIndex,
                        WeaponBehaviour = foundWeapons[i]
                    };
                    MakeWeaponActive(activeWeaponData);
                    startingWeapons.Add(activeWeapon);
            
                    weaponDataList.Add(activeWeaponData);

                    EventManager.OnSetupWeapon?.Invoke(activeWeaponData);

                    hasSetStarter = true;
                    continue;
                }
        
                // Handle passive weapon
                Transform passiveParent = passiveSlotCounter <= passiveSlots.Count
                    ? passiveSlots[passiveSlotCounter]
                    : activeSlot;
        
                var passiveWeaponData = new WeaponSetupData
                {
                    Active = false,
                    WeaponType = foundWeapons[i].WeaponType,
                    WeaponButtonIndex = buttonIndex,
                    WeaponBehaviour = foundWeapons[i]
                };
                startingWeapons.Add(foundWeapons[i]);
                weaponDataList.Add(passiveWeaponData);
                MakeWeaponPassive(passiveWeaponData, passiveParent);

        
                passiveSlotCounter++;

        
                EventManager.OnSetupWeapon?.Invoke(passiveWeaponData);
            }
            //EventManager.OnSetupWeapon?.Invoke(passiveWeaponData);
            EventManager.OnAllWeaponsSetup?.Invoke(weaponDataList);

            //}
                
            /*foreach (var weapon in foundWeapons)
            {
                buttonIndex++;
                
                SubscribeToPassiveEvents(weapon);
                
                // Handle active weapon
                if (!hasSetStarter)
                {
                    var activeWeaponData = new WeaponSetupData
                    {
                        Active = true,
                        WeaponType = weapon.WeaponType,
                        WeaponButtonIndex = buttonIndex,
                        WeaponBehaviour = weapon
                    };
                    MakeWeaponActive(activeWeaponData);
                    startingWeapons.Add(activeWeapon);
                    
                    weaponDataList.Add(activeWeaponData);

                    EventManager.OnSetupWeapon?.Invoke(activeWeaponData);

                    hasSetStarter = true;
                    continue;
                }
                
                // Handle passive weapon
                Transform passiveParent = passiveSlotCounter <= passiveSlots.Count
                    ? passiveSlots[passiveSlotCounter]
                    : activeSlot;
                
                var passiveWeaponData = new WeaponSetupData
                {
                    Active = false,
                    WeaponType = weapon.WeaponType,
                    WeaponButtonIndex = buttonIndex,
                    WeaponBehaviour = weapon
                };
                startingWeapons.Add(weapon);
                weaponDataList.Add(passiveWeaponData);
                MakeWeaponPassive(passiveWeaponData, passiveParent);

                
                passiveSlotCounter++;

                
                EventManager.OnSetupWeapon?.Invoke(passiveWeaponData);
                //EventManager.OnSetupWeapon?.Invoke(passiveWeaponData);
            }
            
            EventManager.OnAllWeaponsSetup?.Invoke(weaponDataList);*/
        }

       
        
        private void MakeWeaponActive(WeaponSetupData weaponData)
        {
            activeWeapon = weaponData.WeaponBehaviour;
            activeWeaponData = weaponData;
            weaponData.WeaponBehaviour.MakeActive(activeSlot);
            weaponParents[weaponData.WeaponBehaviour] = activeSlot;

            int weaponID = (int) CurrentWeaponType - 1;
            playerAnimator.SetInteger(weaponIdParameterName, weaponID);
            
            //OnWeaponActive?.Invoke(weapon.WeaponType);
            weaponData.Active = true;
            EventManager.OnWeaponActive?.Invoke(weaponData.WeaponType);
        }
        
        private void MakeWeaponPassive(WeaponSetupData weaponData, Transform passiveParent)
        {
            weaponData.WeaponBehaviour.MakePassive(passiveParent);
            weaponParents[weaponData.WeaponBehaviour] = passiveParent;
            
            EventManager.OnWeaponPassive?.Invoke(weaponData.WeaponType);
            weaponData.Active = false;
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
            EventManager.OnPassiveAttackStart?.Invoke(GetPassiveAttackData(weapon));
        }
        
        private void StopPassiveAttack(WeaponBehaviour weapon)
        {
            EventManager.OnPassiveAttackStop?.Invoke(GetPassiveAttackData(weapon));
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

        public void CycleWeaponsRight()
        {
            
            Debug.Log("RightCycle Pressed");

            var last = weaponDataList[currentlyAllowedWeapons-1];
            for (int i = currentlyAllowedWeapons-1; i > 0; i--)
            {
                weaponDataList[i] = weaponDataList[(i - 1) % currentlyAllowedWeapons];
            }
            weaponDataList[0] = last;

            WeaponSetupData newActiveWeaponData = weaponDataList[0];
            WeaponSetupData oldActiveWeaponData = activeWeaponData;
            
            if (newActiveWeaponData.WeaponType == oldActiveWeaponData.WeaponType)
            {
                Debug.LogError("The new weapon was the same as the old weapon!");
                return;
            }

            Transform newPassiveSlot = weaponParents[newActiveWeaponData.WeaponBehaviour];

            // switching passive from active
            MakeWeaponPassive(oldActiveWeaponData, newPassiveSlot);
            MakeWeaponActive(newActiveWeaponData);
            EventManager.OnWeaponSwitch?.Invoke(newActiveWeaponData,weaponDataList);
            /*Debug.Log("RightCycle Pressed");

            var last = weaponDataList[^1];
            for (int i = weaponDataList.Count-1; i > 0; i--)
            {
                weaponDataList[i] = weaponDataList[i - 1 % weaponDataList.Count];
            }
            weaponDataList[0] = last;

            WeaponSetupData newActiveWeaponData = weaponDataList[0];
            WeaponSetupData oldActiveWeaponData = activeWeaponData;
            
            if (newActiveWeaponData.WeaponType == oldActiveWeaponData.WeaponType)
            {
                Debug.LogError("The new weapon was the same as the old weapon!");
                return;
            }

            Transform newPassiveSlot = weaponParents[newActiveWeaponData.WeaponBehaviour];

            // switching passive from active
            MakeWeaponPassive(oldActiveWeaponData, newPassiveSlot);
            MakeWeaponActive(newActiveWeaponData);
            EventManager.OnWeaponSwitch?.Invoke(newActiveWeaponData,weaponDataList);*/
           
        }
        
        public void CycleWeaponsLeft()
        {
            var first = weaponDataList[0];
            for (int i = 0; i < currentlyAllowedWeapons; i++)
            {
                weaponDataList[i] =  weaponDataList[(i + 1) % currentlyAllowedWeapons] ;
            }
            weaponDataList[currentlyAllowedWeapons-1] = first;

            
            WeaponSetupData newActiveWeaponData = weaponDataList[0];
            WeaponSetupData oldActiveWeaponData = activeWeaponData;
        
           
            if (newActiveWeaponData.WeaponType == oldActiveWeaponData.WeaponType)
            {
                Debug.LogError("The new weapon was the same as the old weapon!");
                return;
            }

            Transform newPassiveSlot = weaponParents[newActiveWeaponData.WeaponBehaviour];

            // switching passive from active
            MakeWeaponPassive(oldActiveWeaponData, newPassiveSlot);
            MakeWeaponActive(newActiveWeaponData);
            EventManager.OnWeaponSwitch?.Invoke(newActiveWeaponData,weaponDataList);
            
                /*var first = weaponDataList[0];
                for (int i = 0; i < weaponDataList.Count - 1; i++)
                {
                    weaponDataList[i] =  weaponDataList[i + 1 % weaponDataList.Count] ;
                }
                weaponDataList[^1] = first;

                
                WeaponSetupData newActiveWeaponData = weaponDataList[0];
                WeaponSetupData oldActiveWeaponData = activeWeaponData;
            
               
                if (newActiveWeaponData.WeaponType == oldActiveWeaponData.WeaponType)
                {
                    Debug.LogError("The new weapon was the same as the old weapon!");
                    return;
                }

                Transform newPassiveSlot = weaponParents[newActiveWeaponData.WeaponBehaviour];

                // switching passive from active
                MakeWeaponPassive(oldActiveWeaponData, newPassiveSlot);
                MakeWeaponActive(newActiveWeaponData);
                EventManager.OnWeaponSwitch?.Invoke(newActiveWeaponData,weaponDataList);*/
            
        }
        public void SwitchWeapon(int weaponNumber)
        {
            if (weaponNumber > startingWeapons.Count)
            {
                Debug.LogWarning($"Can't switch to weapon {weaponNumber} because there are only {startingWeapons.Count} weapons.");
                return;
            }

            if (weaponNumber > currentlyAllowedWeapons)
            {
                return;
            }

            
            int numberInList = weaponNumber - 1;

            while (startingWeapons[numberInList].WeaponType !=  weaponDataList[0].WeaponType)
            {
                /*var first = weaponDataList[0];
                for (int i = 0; i < weaponDataList.Count - 1; i++)
                {
                    weaponDataList[i] =  weaponDataList[i + 1 % weaponDataList.Count] ;
                }
                weaponDataList[^1] = first;*/
                CycleWeaponsLeft();
            }
            
            
            /*WeaponSetupData newActiveWeaponData = weaponDataList[numberInList];
            WeaponSetupData oldActiveWeaponData = activeWeaponData;
            if (newActiveWeaponData.WeaponType == oldActiveWeaponData.WeaponType)
            {
                return;
            }

            Transform newPassiveSlot = weaponParents[newActiveWeaponData.WeaponBehaviour];

            // switching passive from active
            MakeWeaponPassive(oldActiveWeaponData, newPassiveSlot);
            MakeWeaponActive(newActiveWeaponData);
            EventManager.OnWeaponSwitch?.Invoke(newActiveWeaponData,weaponDataList);*/
        }

        public void ReleaseSpecial()
        {
            string specialAtk = GetActiveAttackAnimationName(AttackType.Special);
            playerAnimator.SetBool(specialAtk, false);
        }
        
        public void PerformUltimateAttack(WeaponType currentWeapon)
        {
            currentAttackType = AttackType.Ultimate;
            EventManager.OnUltimatePerform?.Invoke(currentWeapon,GetActiveAttackData());
            playerAnimator.SetTrigger("startUltimateTrigger");
            Debug.Log("Ultimate performed with weapon " + currentWeapon);
        }

        public void PrepareUltimateAttack()
        {
            currentAttackType = AttackType.Ultimate;
            EventManager.OnUltimatePrepare?.Invoke(GetActiveAttackData());
            playerAnimator.SetTrigger("startUltimateTrigger");
        }

        public void ReturnActiveWeapon()
        {
            activeWeapon.SetParent(activeSlot);
            playerAnimator.SetTrigger("hammerReturn");
        }

        public void SetCharge(ChargeState state)
        {
            chargeState = state;
        }

        public void SetChargeLevel(int newLevel)
        {
            EventManager.OnChargeLevelChange?.Invoke(newLevel);
        }

        public void ResetUltimatePrepare()
        {
            playerAnimator.SetBool("startUltimate", false);
            playerAnimator.ResetTrigger("startUltimateTrigger");
        }

        public void SetMovementXY(float2 localMovementVector)
        {
            playerAnimator.SetFloat("movementSpeedX", localMovementVector.x);
            playerAnimator.SetFloat("movementSpeed", localMovementVector.y);
        }

        void Dash()
        {
            playerAnimator.SetTrigger("dashTrigger");
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