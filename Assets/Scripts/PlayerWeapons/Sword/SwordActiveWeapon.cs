using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SwordActiveWeapon : Weapon, IMakeDamage, IHasForce, IHasCooldown, IHasSize, IHasSpeed, IHasLifeSteal
{
    public PlayerAudio playerAudio;
    public Animator swordAnimator;
    public SwordAnimationEventHandler swordAnimationEventHandler;
    private PlayerWeaponBehaviour playerWeaponBehaviour;
    public string animatorTrigger = "weaponAttack";
    public int damage;
    public float force;
    public float cooldown;
    //public float size;
    public float speed = 0;
    public float lifesteal;
    
    public float timer = 0;

    [SerializeField] HealthHandler healthHandler;
    
    private bool isAttackReady;
    private bool isAttacking;
    
    //private DamageNumberPool poolHolder;
    //private ObjectPool<DamagePopup> damageNumberPool;

    private int lifestealCounter;

    public int lifestealCap = 1;
    
    //public LayerMaskHandler enemyLayerMaskHandler;
    private LayerMask enemyLayerMask;

    private CapsuleCollider swordCollider;
    private Rigidbody swordRigidbody;
    private List<float> swordAnimationValues = new List<float>();

    private int swordAnimationIndex = 0;
    private float currentSwordAnimationValue;

    private float minAnimationSpeed = 0.1f;
    private float defaultAnimationSpeed = 1f;
    private float currentAnimationSpeed = 1f;

    private float defaultSwingSpeed;
    private float defaultAttackSpeed;

    private void Awake()
    {
        playerWeaponBehaviour = FindObjectOfType<PlayerWeaponBehaviour>();
        
        //poolHolder = FindObjectOfType<DamageNumberPool>();
        //damageNumberPool = poolHolder.Pool;
        
        //enemyLayerMask = enemyLayerMaskHandler.layerMask;
        swordCollider = GetComponent<CapsuleCollider>();
        swordRigidbody = GetComponent<Rigidbody>(); 
        swordCollider.enabled = false;
        swordRigidbody.detectCollisions= false;

        isAttackReady = true;
        isAttacking = false;

        defaultSwingSpeed = swordAnimator.GetFloat("swordSwingSpeed");
        defaultAttackSpeed = swordAnimator.GetFloat("weaponAttackSpeed");
    }

    private void OnEnable()
    {
        weapon = this;
        
        swordAnimationEventHandler.onSwing += RunSwingSequence;
        swordAnimationEventHandler.onImpact += RunImpactSequence;
        swordAnimationEventHandler.onRetract += RunRetractSequence;
        swordAnimationEventHandler.onAnimationEnd += RunEndSequence;
        swordAnimationEventHandler.onAnimationEnd += EndAttack;
        swordAnimationEventHandler.onAnimationInterval += UpdateAnimationIndex;
        swordAnimationEventHandler.onAnimationStart += SetUpAnimation;
        swordAnimationEventHandler.onAnimationStart += CheckDamage;
    }
    
    private void OnDisable()
    {
        swordAnimationEventHandler.onSwing -= RunSwingSequence;
        swordAnimationEventHandler.onImpact -= RunImpactSequence;
        swordAnimationEventHandler.onRetract -= RunRetractSequence;
        swordAnimationEventHandler.onAnimationEnd -= RunEndSequence;
        swordAnimationEventHandler.onAnimationEnd -= EndAttack;
        swordAnimationEventHandler.onAnimationInterval -= UpdateAnimationIndex;
        swordAnimationEventHandler.onAnimationStart -= SetUpAnimation;
        swordAnimationEventHandler.onAnimationStart -= CheckDamage;
    }
    
    public override string GetAnimatorKeywords()
    {
        return animatorTrigger;
    }

    public override BaseWeaponType GetWeaponType()
    {
        return BaseWeaponType.Sword;
    }

    private void Update()
    {
        if (isAttackReady && Input.GetMouseButton(0))
        {
            Attack();
        }

        if (isAttacking)
        {
            UpdateAnimationValues();
        }
    }

    public void SetSize(float amount)
    {
        //this.size += amount;
        float oldZPos = swordCollider.center.z;
        swordCollider.center = new Vector3(0, 0, oldZPos - (amount / 2));
        swordCollider.height += amount;
    }

    public void SetSpeed(float amount)
    {
        this.speed += amount;

        swordAnimator.SetFloat("swordSwingSpeed", (defaultSwingSpeed + speed));
        swordAnimator.SetFloat("weaponAttackSpeed", (defaultAttackSpeed + speed));
        //defaultAnimationSpeed = 1f + this.speed;
    }

    private void ApplyLifesteal()
    {
        lifestealCounter++;
        if (lifestealCounter > lifestealCap) return;
        healthHandler.CurrentHealth = lifesteal;

        if (lifesteal > 0)
        {
            //var damageNumber = damageNumberPool.Get();
            //damageNumber.transform.position = this.transform.position;
            //damageNumber.Setup((int)(lifesteal), true);
        }
    }

    public void SetAnimationValues(List<float> list)
    {
        swordAnimationValues = list;
    }

    private void UpdateAnimationIndex()
    {
        int maxIndex = swordAnimationValues.Count - 1;
        swordAnimationIndex += 1;
        if (swordAnimationIndex > maxIndex) swordAnimationIndex = 0;
        currentSwordAnimationValue = swordAnimationValues[swordAnimationIndex];
    }


    //INFO: This updates every frame, changing the speed of the sword animation. You can change the "formula" for applying the changes below. 
    private void UpdateAnimationValues()
    {
        float aniSpeed = currentAnimationSpeed;

        //Change this line to for example: aniSpeed *= currentSwordAnimationValue or aniSpeed = currentSwordAnimationValue;
        aniSpeed = currentSwordAnimationValue;

        if(aniSpeed < minAnimationSpeed) aniSpeed = minAnimationSpeed;
        swordAnimator.SetFloat("swordSwingSpeed", aniSpeed);
    }

    private void SetUpAnimation()
    {
        //swordAnimator.SetFloat("swordSwingSpeed", defaultAnimationSpeed);
        //swordAnimator.SetFloat("weaponAttackSpeed", defaultAnimationSpeed);
        currentAnimationSpeed = defaultSwingSpeed;

        swordAnimationIndex = -1;
        UpdateAnimationIndex();
    }
    
    private void Attack()
    {
        lifestealCounter = 0;

        swordAnimator.SetBool("weaponAttack", true);
        isAttackReady = false;

        isAttacking = true;
    }

    private void CheckDamage()
    {
        swordCollider.enabled = true;
        swordRigidbody.detectCollisions = true;
    }

    private void EndAttack()
    {
        isAttackReady = true;
        swordCollider.enabled = false;
        swordRigidbody.detectCollisions= false;
        isAttacking = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Vector3 direction = other.transform.position - this.transform.position;
            //other.GetComponent<BaseEnemyHealth>().CheckDamage(direction, force, damage);
            //statCounterHandler.swordDamage += damage;
            ApplyLifesteal();
            RunHitSequence();
        }
        else if (other.CompareTag("Breakable"))
        {
            //BreakableBehaviour breakableBehaviour = other.GetComponent<BreakableBehaviour>();
            Vector3 collisionDir = this.transform.position - other.transform.position;
            //if (breakableBehaviour.isAlive) breakableBehaviour.BreakObject(collisionDir);
        }
    }

    public void UpdateDamage(int deltaDamage)
    {
        damage += deltaDamage;
    }

    public void UpdateForce(float deltaForce)
    {
        force += deltaForce;
    }

    public void UpdateCooldown(float deltaCooldown)
    {
        cooldown -= deltaCooldown;
    }

    public void UpdateSize(float deltaSize)
    {
        SetSize(deltaSize);
    }

    public void UpdateSpeed(float deltaSpeed)
    {
        SetSpeed(deltaSpeed);
    }

    public void UpdateLifeSteal(float deltaLifeSteal)
    {
        lifesteal += deltaLifeSteal;
    }
}
