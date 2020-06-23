using UnityEngine;
using UnityEngine.UI;

public class TankBotMovement : TankMovement
{
    protected string m_OpponentMovementAxisName;
    protected string m_OpponentTurnAxisName;
    protected Transform opponent;
    protected TankBotShooting tankBotShooting;
    // protected new TankMovement tankMovement;
    protected float lastMovementInput = 0f;
    protected float updateTime = 0f;
    public float predictionTime = 1.3f;
    public float randomMovement = 0.3f;
    public float turnRate = 1;
    public float moveSpeed = 1f;

    public new void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        opponent = GameObject.FindGameObjectsWithTag("Player")[0].transform;
        tankBotShooting = transform.gameObject.GetComponent<TankBotShooting>();
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
        // tankMovement = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<TankMovement>();
    }

    public override void SetUp(float bot_predictionTime, float bot_turnRate, float bot_MoveSpeed) {
        predictionTime = bot_predictionTime;
        turnRate = bot_turnRate;
        moveSpeed = bot_MoveSpeed;
        // Debug.Log(predictionTime + " : " + turnRate);
    }

    protected new void OnEnable()
    {
        m_Rigidbody.isKinematic = false;
    }


    protected new void OnDisable()
    {
        m_Rigidbody.isKinematic = true;
    }


    protected new void Start()
    {
        m_OpponentMovementAxisName = "Vertical" + 1;
        m_OpponentTurnAxisName = "Horizontal" + 1;
        m_OriginalPitch = m_MovementAudio.pitch;
    }


    public Vector3 getPosition(float time, float opponentTurnInputValue, float opponentMovementInputValue)
    {
        float step = time / Time.deltaTime;
        Vector3 nextPosition = opponent.position;
        Vector3 nextForward = opponent.forward;
        float turn = opponentTurnInputValue * m_TurnSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        while (step > 0)
        {
            nextForward = turnRotation * nextForward;
            nextPosition += nextForward * opponentMovementInputValue * m_Speed * Time.deltaTime;
            step--;
        }
        return nextPosition;
    }

    protected new void Update()
    {
        float opponentMovementInputValue = Input.GetAxis(m_OpponentMovementAxisName);
        float opponentTurnInputValue = Input.GetAxis(m_OpponentTurnAxisName);
        // Debug.Log("opponentMove : " + opponentMovementInputValue + " : " + tankMovement.getMovementInputValue());
        Vector3 nextPosition = getPosition(predictionTime - tankBotShooting.loadTime, opponentTurnInputValue, opponentMovementInputValue);
        // Vector3 next_position = opponent.position + (turnRotation * opponent.forward) * m_OpponentInputValue * m_Speed * predictionTime;
        float distance = Vector3.Distance(transform.position, nextPosition);
        if (tankBotShooting.getFireTime() > 0)
        {
            if (updateTime > tankBotShooting.delayTime / 3)
            {
                // m_MovementInputValue = 0.5f + Random.Range(-randomMovement * 2, randomMovement);
                // m_MovementInputValue = Mathf.Min(Mathf.Max(-1, m_MovementInputValue), 1);
                m_MovementInputValue = Random.Range(0, 2);
                if (m_MovementInputValue == 0)
                {
                    m_MovementInputValue = -0.5f;
                }
                updateTime = 0;
            }
            else
            {
                updateTime += Time.deltaTime;
            }
        }
        else
        {
            m_MovementInputValue = 0;
        }
        if (distance > tankBotShooting.m_MaxLaunchForce)
        {
            m_MovementInputValue = 1;
        }
        if (distance < tankBotShooting.m_MinLaunchForce)
        {
            m_MovementInputValue = -1;
        }
        m_MovementInputValue *= moveSpeed * Random.Range(0.5f, 2f);
        m_MovementInputValue = Mathf.Min(Mathf.Max(-1, m_MovementInputValue), 1);
        m_MovementInputValue = Mathf.Lerp(lastMovementInput, m_MovementInputValue, Time.deltaTime * 5f);
        lastMovementInput = m_MovementInputValue;
        Vector3 expected_position = getPosition(predictionTime, opponentTurnInputValue, opponentMovementInputValue);
        Vector3 targetDir = expected_position - transform.position;
        Vector3 tmp = Vector3.Cross(transform.forward, targetDir);
        float angle = Vector3.Angle(transform.forward, targetDir);

        if (tmp.y < 0)
        {
            angle *= -1;
        }
        m_TurnInputValue = Mathf.Min(Mathf.Max(-1, angle * turnRate * Random.Range(0.5f, 1.5f) / m_TurnSpeed), 1);

        EngineAudio();
    }


    protected new void EngineAudio()
    {
        // Play the correct audio clip based on whether or not the tank is moving and what audio is 
        // currently playing.
        if (Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f)
        {
            if (m_MovementAudio.clip == m_EngineDriving)
            {
                m_MovementAudio.clip = m_EngineIdling;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange,
                    m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
        else
        {
            if (m_MovementAudio.clip == m_EngineIdling)
            {
                m_MovementAudio.clip = m_EngineDriving;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange,
                    m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
    }


    protected new void FixedUpdate()
    {
        // Move and turn the tank.
        // Debug.Log(m_TurnInputValue);
        Move();
        Turn();
    }


    protected new void Move()
    {
        // Adjust the position of the tank based on the player's input.
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }


    protected new void Turn()
    {
        // Adjust the rotation of the tank based on the player's input.
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }
}