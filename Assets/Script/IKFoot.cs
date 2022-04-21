using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKFoot : MonoBehaviour
{

    JumpSystem jp = GameObject.Find("Haruko").GetComponent<JumpSystem>();

    private Animator animator = GameObject.Find("Haruko").GetComponent<Animator>();

    //　テスト用のIKのOn・Offスイッチ
    [SerializeField]
    private bool useIK = true;
    //　IKで角度を有効にするかどうか
    [SerializeField]
    private bool useIKRot = true;
    //　右足のウエイト
    private float rightFootWeight = 0f;
    //　左足のウエイト
    private float leftFootWeight = 0f;
    //　右足の位置
    private Vector3 rightFootPos;
    //　左足の位置
    private Vector3 leftFootPos;
    //　右足の角度
    private Quaternion rightFootRot;
    //　左足の角度
    private Quaternion leftFootRot;
    //　右足と左足の距離
    private float distance;
    //　足を付く位置のオフセット値
    [SerializeField]
    private float offset = 0.1f;
    //　コライダの中心位置
    private Vector3 defaultCenter;
    //　レイを飛ばす距離
    [SerializeField]
    private float rayRange = 1f;

    //　コライダの位置を調整する時のスピード
    [SerializeField]
    private float smoothing = 100f;

    //　レイを飛ばす位置の調整値
    [SerializeField]
    private Vector3 rayPositionOffset = Vector3.up * 0.3f;


    void OnAnimatorIK()
    {
        //　IKを使わない場合はこれ以降なにもしない
        if (!useIK)
        {
            return;
        }

        //　アニメーションパラメータからIKのウエイトを取得
        rightFootWeight = animator.GetFloat("RightFootWeight");
        leftFootWeight = animator.GetFloat("LeftFootWeight");

        //　右足用のレイの視覚化
        Debug.DrawRay(animator.GetIKPosition(AvatarIKGoal.RightFoot) + rayPositionOffset, -transform.up * rayRange, Color.red);
        //　右足用のレイを飛ばす処理
        var ray = new Ray(animator.GetIKPosition(AvatarIKGoal.RightFoot) + rayPositionOffset, -transform.up);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayRange, LayerMask.GetMask("Field")))
        {
            rightFootPos = hit.point;

            //　右足IKの設定
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightFootWeight);
            animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootPos + new Vector3(0f, offset, 0f));
            if (useIKRot)
            {
                rightFootRot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
                animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, rightFootWeight);
                animator.SetIKRotation(AvatarIKGoal.RightFoot, rightFootRot);
            }
        }

        //　左足用のレイを飛ばす処理
        ray = new Ray(animator.GetIKPosition(AvatarIKGoal.LeftFoot) + rayPositionOffset, -transform.up);
        //　左足用のレイの視覚化
        Debug.DrawRay(animator.GetIKPosition(AvatarIKGoal.LeftFoot) + rayPositionOffset, -transform.up * rayRange, Color.red);

        if (Physics.Raycast(ray, out hit, rayRange, LayerMask.GetMask("Field")))
        {
            leftFootPos = hit.point;

            //　左足IKの設定
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
            animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootPos + new Vector3(0f, offset, 0f));

            if (useIKRot)
            {
                leftFootRot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
                animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
                animator.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootRot);
            }
        }
    }

}
