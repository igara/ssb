﻿using System;
using System.Collections;
using UnityEngine;

public class RobotKyleCharacterScript : MonoBehaviour
{
    public UserData userData;
    private Animator animator;
    private Rigidbody rigidBody;
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
        attack = false;
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
                RobotKyleCharacterScript targetRobotKyleCharacterScript = targetRobotKyleCharacterRightArmScript.robotKyleCharacterGameObject.GetComponent<RobotKyleCharacterScript>();

                if (targetRobotKyleCharacterScript.attack)
                {
                    damage += 10;

                    Vector3 targetVector = new Vector3(targetGameObject.transform.position.x, 0, targetGameObject.transform.position.z);
                    Vector3 selfVector = new Vector3(position.x, 0, position.z);
                    Vector3 diffVector = Vector3.Normalize(selfVector - targetVector);
                    if (targetRobotKyleCharacterScript.userData.rotationStatus == (int)UserData.RotationStatus.LEFT)
                    {
                        diffVector = diffVector + new Vector3(-0.1f * damage, 0, 0);
                    }
                    else
                    {
                        diffVector = diffVector + new Vector3(0.1f * damage, 0, 0);
                    }

                    rigidBody.AddForce(diffVector * damage, ForceMode.Impulse);
                }
            }
            if (targetGameObject.tag == "unity_chan_character_attack")
            {
                UnityChanCharacterRightArmScript targetUnityChanCharacterRightArmScript = targetGameObject.GetComponent<UnityChanCharacterRightArmScript>();
                UnityChanCharacterScript targetUnityChanCharacterScript = targetUnityChanCharacterRightArmScript.unityChanCharacterGameObject.GetComponent<UnityChanCharacterScript>();

                if (targetUnityChanCharacterScript.attack)
                {
                    damage += 5;

                    Vector3 targetVector = new Vector3(targetGameObject.transform.position.x, 0, targetGameObject.transform.position.z);
                    Vector3 selfVector = new Vector3(position.x, 0, position.z);
                    Vector3 diffVector = Vector3.Normalize(selfVector - targetVector);
                    if (targetUnityChanCharacterScript.userData.rotationStatus == (int)UserData.RotationStatus.LEFT)
                    {
                        diffVector = diffVector + new Vector3(-0.1f * damage, 0, 0);
                    }
                    else
                    {
                        diffVector = diffVector + new Vector3(0.1f * damage, 0, 0);
                    }

                    rigidBody.AddForce(diffVector * damage, ForceMode.Impulse);
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
