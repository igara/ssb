using System;
using System.Collections;
using UnityEngine;

public class UnityChanCharacterScript : MonoBehaviour
{
    public UserData userData;
    private Animator animator;
    private Rigidbody rigidBody;
    private AudioSource inputTypeAudioSource;
    private Vector3 position;
    private bool flying = true;
    public bool dead = false;
    private int damage = 0;
    public bool attack = false;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
        inputTypeAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        position = transform.position;

        if (GameSetting.selfUserData.id == userData.id)
        {
            if (Input.GetKeyDown(InputType.KeyType.a.ToString()))
            {
                DoAttack();
                GameSetting.selfUserData.inputType = (int)InputType.KeyType.a;
            }
            else if (flying == false && Input.GetKeyDown(InputType.KeyType.up.ToString()))
            {
                DoUp();
                GameSetting.selfUserData.inputType = (int)InputType.KeyType.up;
            }
            else if (Input.GetKey(InputType.KeyType.right.ToString()))
            {
                DoRight();
                GameSetting.selfUserData.inputType = (int)InputType.KeyType.right;
            }
            else if (Input.GetKey(InputType.KeyType.left.ToString()))
            {
                DoLeft();
                GameSetting.selfUserData.inputType = (int)InputType.KeyType.left;
            }
            else
            {
                animator.SetInteger("moveType", (int)InputType.KeyType.wait);
                GameSetting.selfUserData.inputType = (int)InputType.KeyType.wait;
            }
        }
        else
        {
            if (userData.inputType == (int)InputType.KeyType.a)
            {
                DoAttack();
            }
            else if (flying == false && userData.inputType == (int)InputType.KeyType.up)
            {
                DoUp();
            }
            else if (userData.inputType == (int)InputType.KeyType.right)
            {
                DoRight();
            }
            else if (userData.inputType == (int)InputType.KeyType.left)
            {
                DoLeft();
            }
            else
            {
                animator.SetInteger("moveType", (int)InputType.KeyType.wait);
            }
        }
    }
    void DoUp()
    {
        flying = true;
        animator.SetInteger("moveType", (int)InputType.KeyType.up);
        rigidBody.AddForce(Vector3.up * 150);
        AudioClip audioClip = Resources.Load("Sounds/UnityChan/Jump0001") as AudioClip;
        inputTypeAudioSource.clip = audioClip;
        inputTypeAudioSource.Play();
    }

    void DoRight()
    {
        animator.SetInteger("moveType", (int)InputType.KeyType.right);
        transform.rotation = Quaternion.Euler(0, (int)UserData.RotationStatus.RIGHT, 0);
        position = transform.position;
        position.x += 0.1f;
        transform.position = position;
        userData.rotationStatus = (int)UserData.RotationStatus.RIGHT;
    }

    void DoLeft()
    {
        animator.SetInteger("moveType", (int)InputType.KeyType.left);
        transform.rotation = Quaternion.Euler(0, (int)UserData.RotationStatus.LEFT, 0);
        position = transform.position;
        position.x -= 0.1f;
        transform.position = position;
        userData.rotationStatus = (int)UserData.RotationStatus.LEFT;
    }

    void DoAttack()
    {
        animator.SetInteger("moveType", (int)InputType.KeyType.a);
        attack = true;
        StartCoroutine(SetAttackFalse());
    }

    private IEnumerator SetAttackFalse()
    {
        yield return new WaitForSeconds(0.5f);
        AudioClip audioClip = Resources.Load("Sounds/UnityChan/Attack1102") as AudioClip;
        inputTypeAudioSource.clip = audioClip;
        inputTypeAudioSource.Play();
        attack = false;
    }

    public void DoOpenMessage()
    {
        AudioClip audioClip = Resources.Load("Sounds/UnityChan/Create0015") as AudioClip;
        inputTypeAudioSource.clip = audioClip;
        inputTypeAudioSource.Play();
    }

    public void DoDeadMessage()
    {
        AudioClip audioClip = Resources.Load("Sounds/UnityChan/Dead1095") as AudioClip;
        inputTypeAudioSource.clip = audioClip;
        inputTypeAudioSource.Play();
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "stage")
        {
            flying = false;
        }
        if (collision.gameObject.tag == "wall_top" || collision.gameObject.tag == "wall_under" || collision.gameObject.tag == "wall_left" || collision.gameObject.tag == "wall_right")
        {
            dead = true;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        GameObject targetGameObject = collider.gameObject;

        try
        {
            if (targetGameObject.tag == "robot_kyle_character_attack")
            {
                RobotKyleCharacterRightArmScript targetRobotKyleCharacterRightArmScript = targetGameObject.GetComponent<RobotKyleCharacterRightArmScript>();
                RobotKyleCharacterScript targetRobotKyleCharacter = targetRobotKyleCharacterRightArmScript.robotKyleCharacterGameObject.GetComponent<RobotKyleCharacterScript>();

                if (targetRobotKyleCharacter.attack)
                {
                    damage += 10;
                    Debug.Log(damage);
                }
            }
        }
        catch
        {

        }

    }

    public int GetCpuInputType()
    {
        System.Random random = new System.Random();
        return random.Next(System.Enum.GetValues(typeof(InputType.KeyType)).Length);
    }
}
