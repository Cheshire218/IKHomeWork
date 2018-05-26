using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class IKControl : MonoBehaviour {

    private Animator animator;
    private Transform thisTransform;
    private SphereCollider triggerHead;
    [SerializeField]
    private Transform lookObj;

    public bool ikActive = false;
    public Transform rightHandObj = null;
    public List<Transform> lookObjects = null;    public float weight=1;

    public float maxHeadDistance = 2f;

    void Start()
    {
        animator = GetComponent<Animator>();
        thisTransform = transform;
        triggerHead = GetComponent<SphereCollider>();
        triggerHead.radius = maxHeadDistance;
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Lookable")
        {
            lookObjects.Add(col.transform);
        }
    }
    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Lookable")
        {
            lookObjects.Remove(col.transform);
        }
    }

    void OnAnimatorIK()
    {
        if (animator)
        {
            //Если, мы включили IK, устанавливаем позицию и вращение
            if (ikActive)
            {
                if (lookObjects.Count > 0)
                {
                    float distance = maxHeadDistance + 1;
                    foreach (var item in lookObjects)
                    {
                        if (distance > Vector3.Distance(thisTransform.position, item.position))
                        {
                            distance = Vector3.Distance(thisTransform.position, item.position);
                            lookObj = item;
                        }
                    }
                }
                else
                {
                    lookObj = null;
                }
                // Устанавливаем цель взгляда для головы
                if (lookObj != null)
                {
                    animator.SetLookAtWeight(weight);
                    animator.SetLookAtPosition(lookObj.position);
                }
                // Устанавливаем цель для правой руки и выставляем её в позицию
                if (rightHandObj != null)
                {
                    Vector3 target = rightHandObj.position - thisTransform.position;
                    Vector3 vector = Quaternion.Euler(0, 40, 0) * thisTransform.forward;

                    if (Vector3.Dot(vector, target) > 0)
                    {
                        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, weight);
                        //animator.SetIKRotationWeight(AvatarIKGoal.RightHand, xx);
                        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                        //animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
                    }
                    else
                    {
                        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                    }

                }
            }
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                animator.SetLookAtWeight(0);
            }
        }
    }
}