using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class LimitCatch : MonoBehaviour
{
    private PlayerMove player;
    [SerializeField] private CameraLimit cl_end;
    [SerializeField] private CameraLimit cl_end2;
    public float cam_x;
    public Vector3 _camera;
    private CinemachineVirtualCamera vir_cam;
    [SerializeField]private bool set; /* true:セット済み　false:未セット */

    float now_offset;
    float old_offset;

    [SerializeField] private Vector3 start_lim;
    [SerializeField] private Vector3 end_lim;

    // Start is called before the first frame update
    void Start()
    {
        vir_cam = GetComponent<CinemachineVirtualCamera>();

        _camera = transform.position;
        player = GameObject.Find("Haruko").GetComponent<PlayerMove>();
    }

    // Update is called once per frame
    void Update()
    {
        now_offset = Vector3.Distance(player.transform.position, transform.position);
        if (cl_end.end || cl_end2.end)
        {
            if(set == false)
            {
                set = true;
                cam_x = transform.position.x;
                _camera = transform.position;
                if(cl_end.end) start_lim = _camera;
                if (cl_end2.end) end_lim = _camera;
                old_offset = Vector3.Distance(player.transform.position, transform.position);

            }
            else
            {
                Debug.Log("カメラ制御発動");
                if (cl_end.end)
                {
                    if (cl_end.end_pos.x < player.transform.position.x)
                    {
                        if (player.left != 0)
                        {
                            vir_cam.Follow = null;
                            if (_camera.x < start_lim.x)
                            {
                                this.transform.position = start_lim;
                            }
                            else
                            {
                                this.transform.position = _camera;
                            }
                        }
                        else
                        {
                            _camera = transform.position;
                            if (now_offset <= old_offset)
                            {
                                vir_cam.Follow = GameObject.Find("Haruko").GetComponent<Transform>();
                                
                            }

                        }
                    }
                }
                else if (cl_end2.end)
                {
                    if (cl_end2.end_pos.x > player.transform.position.x)
                    {
                        if (player.right != 0)
                        {
                            vir_cam.Follow = null;
                            if (_camera.x > end_lim.x)
                            {
                                this.transform.position = end_lim;
                            }
                            else
                            {
                                this.transform.position = _camera;
                            }
                        }
                        else
                        {
                            _camera = transform.position;
                            if (now_offset == old_offset)
                            {
                                vir_cam.Follow = GameObject.Find("Haruko").GetComponent<Transform>();
                            }

                        }
                    }
                }
                
                //_camera = new Vector3(cam_x, 10, transform.position.z);
            }
            
            
        }
        else if(!cl_end.end && !cl_end2.end)
        {
            Debug.Log("Set初期化");
            set = false;
        }
    }
}
