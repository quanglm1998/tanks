using UnityEngine;
using UnityEngine.UI;

public class TankBotShooting : TankShooting
{
    protected string m_OpponentMovementAxisName;
    protected string m_OpponentTurnAxisName;
    protected Transform opponent;
    protected float m_Speed = 12f;
    protected float fireTime = 0f;
    protected TankBotMovement tankBotMovement;
    public float delayTime = 3f;
    public float loadTime = 0.6f;

    public float getFireTime() {
        return fireTime;
    }

    protected new void OnEnable()
    {
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }

    public override void SetUp(float bot_delayTime, float bot_loadTime) {
        delayTime = bot_delayTime;
        loadTime = bot_loadTime;
    }

    protected new void Start()
    {
        fireTime = delayTime * Random.Range(0.3f, 1f);
        opponent = GameObject.FindGameObjectsWithTag("Player")[0].transform;
        m_OpponentMovementAxisName = "Vertical" + 1;
        m_OpponentTurnAxisName = "Horizontal" + 1;
        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
        tankBotMovement = transform.gameObject.GetComponent<TankBotMovement>();
    }


    protected new void Update()
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


    protected new void Fire()
    {
        // Instantiate and launch the shell.
        fireTime = delayTime * Random.Range(0.3f, 1f);
        m_Fired = true;

        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;

        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        m_CurrentLaunchForce = m_MinLaunchForce;
    }
}