using UnityEngine;
using UnityEngine.UI;

public class TankBotMovement : MonoBehaviour
{
    public int m_PlayerNumber = 1;
    public float m_Speed = 12f;
    public float m_TurnSpeed = 180f;
    public AudioSource m_MovementAudio;
    public AudioClip m_EngineIdling;
    public AudioClip m_EngineDriving;
    public float m_PitchRange = 0.2f;

    private string m_OpponentMovementAxisName;
    private string m_OpponentTurnAxisName;
    private Rigidbody m_Rigidbody;
    private float m_MovementInputValue;
    private float m_TurnInputValue;
    private float m_OriginalPitch;
    Transform opponent;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        opponent = GameObject.FindGameObjectsWithTag("Player")[0].transform;
        
    }


    private void OnEnable()
    {
        m_Rigidbody.isKinematic = false;
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }


    private void OnDisable()
    {
        m_Rigidbody.isKinematic = true;
    }


    private void Start()
    {
        m_OpponentMovementAxisName = "Vertical" + 1;
        m_OpponentTurnAxisName = "Horizontal" + 1;
        m_OriginalPitch = m_MovementAudio.pitch;
    }


    private void Update()
    {
        // Debug.Log(opponent.position);
        // Debug.Log(opponent.rotation);
        // Store the player's input and make sure the audio for the engine is playing.
        // Debug.Log("bot : " + m_MovementInputValue + " : " + m_TurnInputValue);
        float m_OpponentInputValue = Input.GetAxis(m_OpponentMovementAxisName);
        Vector3 expected_position = opponent.position + opponent.forward * m_OpponentInputValue * m_Speed * 0.5f;
        Vector3 targetDir = expected_position - transform.position;
        Vector3 tmp = Vector3.Cross(transform.forward, targetDir);
        float angle = Vector3.Angle(transform.forward, targetDir);
        if (tmp.y < 0) {
            angle *= -1;
        }
        m_TurnInputValue = angle * 5 / m_TurnSpeed;
        
        EngineAudio();
    }


    private void EngineAudio()
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


    private void FixedUpdate()
    {
        // Move and turn the tank.
        Debug.Log(m_TurnInputValue);
        Move();
        Turn();
    }


    private void Move()
    {
        // Adjust the position of the tank based on the player's input.
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }


    private void Turn()
    {
        // Adjust the rotation of the tank based on the player's input.
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }
}