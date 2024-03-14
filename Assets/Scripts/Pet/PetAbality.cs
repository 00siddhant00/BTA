using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class PetAbality : MonoBehaviour
{
    [Header("Prefabs & Objects")]
    [SerializeField] private Transform spawnPoint;
    public GameObject petPrefab;
    public GameObject crosshairPrefab;
    private GameObject petInstance;
    private GameObject crosshair;

    [Header("Abality Value")]
    public bool aquiredPet;
    [SerializeField] private float healRate;
    private float nextChunkHealTime;
    bool onceInitialHeal;

    [Header("Pet Physics")]
    public bool isThrown = false;
    public bool isThrowing = false;
    public bool isHeal = false;
    private Rigidbody2D petRigidbody;
    [SerializeField] private float PetThrowVelo = 40f;

    [SerializeField] public float callBackDelay = 0.1f;

    [Header("Gizmos Values")]
    [SerializeField] private bool isDrawingGizmo = false;
    public float maxGizmoLength = 10f;
    private Vector3 endPoint;

    //Other Variables
    [HideInInspector] public bool isButtonUp = true;
    private PlayerMovement playerMovement;
    private PlayerCombat combatScript;
    private PlayerController playerController;

    public bool isRight = false;

    public bool dontMove;

    private void Awake()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        playerController = GetComponent<PlayerController>();
        combatScript = GetComponent<PlayerCombat>();
        playerMovement = GetComponent<PlayerMovement>();
        if (petInstance == null)
        {
            petInstance = Instantiate(petPrefab, spawnPoint.position, Quaternion.identity);
            petRigidbody = petInstance.GetComponent<Rigidbody2D>();
            petInstance.SetActive(false);
        }
    }

    private void Start()
    {
        Cursor.visible = false;
    }

    private void Update()
    {
        Cursor.visible = false;
        dontMove = isHeal || isThrowing ? true : false;

        if (aquiredPet)
        {
            InputChecks();
            PetStateCheck();
            CrosshairState();
        }
        else
        {
            combatScript.meleAllowed = false;
            playerMovement.Data = playerController.playerDataNoPet;
        }
    }

    //public void OnPetCallBack(InputAction.CallbackContext ctx)
    //{
    //    if (!aquiredPet) return;

    //    if (ctx.performed)
    //    {
    //        if (isButtonUp)
    //        {
    //            isThrowing = true;
    //            DrawGizmo();
    //        }
    //        else
    //        {
    //            Invoke(nameof(CallBackPet), callBackDelay);
    //            StopHealing();
    //        }
    //    }
    //}

    //public void OnHealThrow(InputAction.CallbackContext ctx)
    //{
    //    if (!aquiredPet) return;

    //    if (ctx.canceled && isThrowing)
    //    {
    //        playerMovement.IsFacingRight = Mathf.Sign(transform.localScale.x) == 1;
    //    }
    //    else if (ctx.canceled)
    //    {
    //        isThrowing = false;
    //        if (isButtonUp)
    //        {
    //            ThrowPet();
    //            PetStateCheck();
    //        }

    //        isButtonUp = !isButtonUp;
    //    }
    //}

    //public void OnHeal(InputAction.CallbackContext ctx)
    //{
    //    if (!aquiredPet) return;

    //    if (ctx.canceled && isThrown)
    //    {
    //        StopHealing();
    //    }
    //}

    void InputChecks()
    {
        //if (Input.GetMouseButtonDown(1))
        if (PlayerInputHandler.Instance.IsPetThrow == 1)
        {
            PlayerInputHandler.Instance.IsPetThrow = 0;
            if (isButtonUp)
            {
                isThrowing = true;
                DrawGizmo();
            }
            else
            {
                Invoke(nameof(CallBackPet), callBackDelay);
                StopHealing();
            }
        }

        //if (Input.GetMouseButtonUp(1) && isThrowing)
        if (PlayerInputHandler.Instance.IsPetThrow == 2 && isThrowing)
        {
            playerMovement.IsFacingRight = Mathf.Sign(transform.localScale.x) == 1;
        }

        if (isThrowing)
        {
            LookAtCrosshair();
        }

        if (isDrawingGizmo)
        {
            DrawGizmo();
        }

        //if (Input.GetMouseButtonUp(1))
        if (PlayerInputHandler.Instance.IsPetThrow == 2)
        {
            PlayerInputHandler.Instance.IsPetThrow = 0;
            isThrowing = false;
            if (isButtonUp)
            {
                ThrowPet();
                PetStateCheck();
            }

            isButtonUp = !isButtonUp;
        }

        //if (Input.GetMouseButton(0) && isThrown && petInstance.GetComponent<PetHealMode>().foundEnemy)
        if (PlayerInputHandler.Instance.IsHealing && isThrown && petInstance.GetComponent<PetHealMode>().foundEnemy)
        {
            if (!onceInitialHeal)
            {
                nextChunkHealTime = Time.time + healRate;
                onceInitialHeal = true;
            }
            Heal();
        }
        //else if (Input.GetMouseButtonUp(0) && isThrown)
        else if (PlayerInputHandler.Instance.IsPetCallBack && isThrown)
        {
            StopHealing();
        }
    }

    void PetStateCheck()
    {
        if (combatScript.slashing)
        {
            playerMovement.Data = playerController.playerDataNoPet;
            return;
        }

        if (!isThrowing)
        {
            if (isThrown)
            {
                combatScript.meleAllowed = false;
                playerMovement.Data = playerController.playerDataNoPet;
            }
            if (!isThrown)
            {
                combatScript.meleAllowed = true;
                playerMovement.Data = playerController.playerDataWithPet;
            }
        }
        else
        {
            combatScript.meleAllowed = false;
            //playerMovement.Data = playerController.playerDataHeal;
        }
    }

    private void DrawGizmo()
    {
        isDrawingGizmo = true;
        endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        endPoint.z = 0f;
        CreateCrosshair();
    }

    private void CreateCrosshair()
    {
        if (crosshair == null && crosshairPrefab != null)
        {
            crosshair = Instantiate(crosshairPrefab, endPoint, Quaternion.identity);
        }

        if (isDrawingGizmo && crosshair != null)
        {
            crosshair.transform.position = endPoint;
        }

    }

    private void CrosshairState()
    {
        if (crosshair != null)
        {
            crosshair.SetActive(isDrawingGizmo);
        }
    }

    private void ThrowPet()
    {
        isDrawingGizmo = false;

        if (!isThrown)
        {
            GameManager.Instance.pet.HidePet();
            isThrown = true;
            Vector3 throwDirection = (endPoint - spawnPoint.position).normalized;
            petInstance.transform.position = spawnPoint.position;
            petRigidbody.velocity = Vector2.zero;
            petInstance.SetActive(true);
            petRigidbody.velocity = throwDirection * PetThrowVelo;
        }
    }

    public void CallBackPet()
    {
        if (isThrown && petInstance != null)
        {
            GameManager.Instance.pet.SpawnFX();

            petInstance.transform.position = spawnPoint.position;
            petInstance.GetComponent<PetHealMode>().SetParentNull();
            petInstance.SetActive(false);
            isThrown = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (isDrawingGizmo)
        {
            Gizmos.color = Color.red;
            Vector3 drawEndPoint = spawnPoint.position + (endPoint - spawnPoint.position).normalized * Mathf.Min(Vector3.Distance(spawnPoint.position, endPoint), maxGizmoLength);
            Gizmos.DrawLine(spawnPoint.position, drawEndPoint);
        }
    }

    private void Heal()
    {
        if (petInstance.GetComponent<PetHealMode>().enemyHealth != null && petInstance.GetComponent<PetHealMode>().enemyHealth.healthPoints <= 0) return;

        if (playerController.playerHealth.playerCurrentHealth >= 1)
        {
            StopHealing();
            Invoke(nameof(CallBackPet), callBackDelay);
            isButtonUp = true;
            return;
        }

        if (Time.time >= nextChunkHealTime)
        {
            GetComponent<PlayerHealth>().HealPlayer();
            nextChunkHealTime = Time.time + healRate;
            petInstance.GetComponent<PetHealMode>().enemyHealth.Damage();
        }

        isHeal = true;
    }

    public void StopHealing()
    {
        onceInitialHeal = false;
        isHeal = false;
    }

    public float scaleSmoothing = 0.1f;  // Adjust this value based on your needs

    void LookAtCrosshair() => transform.localScale = new Vector2(((endPoint - transform.position).x >= 0) ? 0.8f : -0.8f, transform.localScale.y);


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("getPet"))
        {
            aquiredPet = true;
            Destroy(collision.gameObject);
        }
    }

    public void PetAquired(bool aquire)
    {
        aquiredPet = aquire;
    }
}
