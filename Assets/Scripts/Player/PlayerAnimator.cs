using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private PlayerMovement mov;
    //private Animator anim;
    private Transform GFX;

    private DemoManager demoManager;

    [Header("Movement Tilt")]
    [SerializeField] private float maxTilt;
    [SerializeField][Range(0, 1)] private float tiltSpeed;

    [Header("Particle FX")]
    [SerializeField] private GameObject jumpFX;
    [SerializeField] private GameObject landFX;
    [SerializeField] private GameObject HitFX;
    private ParticleSystem _jumpParticle;
    private ParticleSystem _landParticle;
    private Animator legsAnim;
    public bool isMoving;

    public bool startedJumping { private get; set; }
    public bool justLanded { private get; set; }

    public bool isGrounded = true;

    private void Start()
    {
        mov = GetComponent<PlayerMovement>();
        //anim = GFX.GetComponent<Animator>();
        GFX = transform.GetChild(0);
        legsAnim = GFX.GetChild(2).GetComponent<Animator>();

        demoManager = FindObjectOfType<DemoManager>();

        _jumpParticle = jumpFX.GetComponent<ParticleSystem>();
        _landParticle = landFX.GetComponent<ParticleSystem>();
    }

    private void LateUpdate()
    {
        #region Tilt
        float tiltProgress;

        int mult = -1;

        isMoving = mov._moveInput.x != 0;
        MoveAnim();

        if (startedJumping) isGrounded = false;
        if (justLanded) isGrounded = true;

        if (mov.IsSliding)
        {
            tiltProgress = 0.25f;
        }
        else
        {
            tiltProgress = Mathf.InverseLerp(-mov.Data.runMaxSpeed, mov.Data.runMaxSpeed, mov.RB.velocity.x);
            mult = (mov.IsFacingRight) ? 1 : -1;
        }

        float newRot = ((tiltProgress * maxTilt * 2) - maxTilt);
        float rot = Mathf.LerpAngle(GFX.localRotation.eulerAngles.z * mult, newRot, tiltSpeed);
        GFX.localRotation = Quaternion.Euler(0, 0, rot * mult);


        #endregion

        CheckAnimationState();

        ParticleSystem.MainModule jumpPSettings = _jumpParticle.main;
        jumpPSettings.startColor = new ParticleSystem.MinMaxGradient(demoManager.SceneData.foregroundColor);
        ParticleSystem.MainModule landPSettings = _landParticle.main;
        landPSettings.startColor = new ParticleSystem.MinMaxGradient(demoManager.SceneData.foregroundColor);
    }

    public void MoveAnim()
    {
        legsAnim.SetBool("IsRunning", isMoving);
    }

    public void MoveAnim(bool move)
    {
        legsAnim.SetBool("IsRunning", move);
    }

    public void WallHit(Transform pos)
    {
        GameObject obj = Instantiate(HitFX, pos.position, Quaternion.identity);
        Destroy(obj, 1);
    }

    private void CheckAnimationState()
    {
        if (startedJumping)
        {
            //anim.SetTrigger("Jump");
            GameObject obj = Instantiate(jumpFX, transform.position - (Vector3.up * 1.25f), Quaternion.Euler(-90, 0, 0));
            Destroy(obj, 1);
            startedJumping = false;
            return;
        }

        if (justLanded)
        {
            GameManager.Instance.CameraShake.ShakeCamera(0.7f, 0.29f, 0.15f);
            //anim.SetTrigger("Land");
            GameObject obj = Instantiate(landFX, transform.position - (Vector3.up * 1.25f), Quaternion.Euler(-90, 0, 0));
            Destroy(obj, 1);
            justLanded = false;
            return;
        }

        //anim.SetFloat("Vel Y", mov.RB.velocity.y);
    }
}
