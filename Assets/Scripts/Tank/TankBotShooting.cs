using UnityEngine;
using UnityEngine.UI;

public class TankBotShooting : MonoBehaviour
{
    public Rigidbody m_Shell;
    public Transform m_FireTransform;
    public Slider m_AimSlider;
    public AudioSource m_ShootingAudio;
    public AudioClip m_ChargingClip;
    public AudioClip m_FireClip;
    public float m_MinLaunchForce = 15f;
    public float m_MaxLaunchForce = 30f;
    public float m_MaxChargeTime = 0.75f;


    private string m_FireButton;
    private float m_CurrentLaunchForce;
    private float m_ChargeSpeed;
    private bool m_Fired;
    private string m_OpponentMovementAxisName;
    private string m_OpponentTurnAxisName;
    private Transform opponent;
    private float m_Speed = 12f;
    private float fireTime = 0f;
    private TankBotMovement tankBotMovement;
    public float delayTime = 3f;
    public float loadTime = 0.6f;

    public float getFireTime() {
        return fireTime;
    }

    private void OnEnable()
    {
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }


    private void Start()
    {
        fireTime = delayTime;
        opponent = GameObject.FindGameObjectsWithTag("Player")[0].transform;
        m_OpponentMovementAxisName = "Vertical" + 1;
        m_OpponentTurnAxisName = "Horizontal" + 1;
        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
        tankBotMovement = transform.gameObject.GetComponent<TankBotMovement>();
    }


    private void Update()
    {
        float opponentMovementInputValue = Input.GetAxis(m_OpponentMovementAxisName);
        float opponentTurnInputValue = Input.GetAxis(m_OpponentTurnAxisName);
        Vector3 nextPosition = tankBotMovement.getPosition(tankBotMovement.predictionTime - loadTime, opponentTurnInputValue, opponentMovementInputValue);
        float distance = Vector3.Distance(transform.position, nextPosition);
        distance = Mathf.Max(Mathf.Min(distance, m_MaxLaunchForce), m_MinLaunchForce);
        // Debug.Log(distance + " : " + m_CurrentLaunchForce);


        m_AimSlider.value = m_MinLaunchForce;

        if (fireTime > 0) {
            fireTime -= Time.deltaTime;
        }
        else if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
        {
            // At max charge, not yet fired.
            m_CurrentLaunchForce = m_MaxLaunchForce;
            Fire();
        }
        else if(m_Fired)
        {
            // Have we pressed the Fire button for the first time?
            m_Fired = false;
            m_CurrentLaunchForce = m_MinLaunchForce;

            m_ShootingAudio.clip = m_ChargingClip;
            m_ShootingAudio.Play();

        }
        else if (m_CurrentLaunchForce < distance)
        {
            // Holding the fire button, not yet fired.
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;
            m_AimSlider.value = m_CurrentLaunchForce;
        }
        else
        {
            // We releasted the fire button, having not fired yet.
            Fire();
        }
    }


    private void Fire()
    {
        // Instantiate and launch the shell.
        fireTime = delayTime;
        m_Fired = true;

        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;

        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        m_CurrentLaunchForce = m_MinLaunchForce;
    }
}