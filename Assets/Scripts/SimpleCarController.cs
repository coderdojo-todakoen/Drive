using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;

    public Transform leftVisualWheel;
    public Transform rightVisualWheel;
}

// Unity Manualにある、「Wheel Collider Tutorial」を
// 使用した、WheelColliderのデモ
public class SimpleCarController : MonoBehaviour
{
    public List<AxleInfo> axleInfos;
    public float maxMotorTorque;
    public float maxSteeringAngle;

    private Rigidbody rb;

    public Text time;
    public Text speed;

    // Main Cameraの位置(車のローカル座標)
    private Vector3 cameraPosition = new Vector3(0, 1, -3.5F);

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        // 車が転ばないように、重心を下げてみる
        rb.centerOfMass = new Vector3(0, -0.5F, 0);

        // MainCameraを車の後ろへ移動し、車の方へ向けます
        Camera.main.transform.position = transform.TransformPoint(cameraPosition);
        Camera.main.transform.LookAt(transform);
    }

    // 対応する視覚的なホイールを見つけます
    // Transform を正しく適用します
    public void ApplyLocalPositionToVisuals(WheelCollider collider, Transform visualWheel)
    {
        if (visualWheel == null)
        {
            return;
        }

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

    public void FixedUpdate()
    {
        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");

        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
            ApplyLocalPositionToVisuals(axleInfo.leftWheel, axleInfo.leftVisualWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel, axleInfo.rightVisualWheel);
        }

        // 経過時間を表示します
        float t = Time.fixedTime;
        int m = (int)(t / 60.0f);
        float s = t - m * 60.0f;
        time.text = String.Format("{0:00}.{1:00.00}", m, s);

        // 速度を秒速(m/s)から時速(km/h)へ変換して表示します
        Vector3 v = rb.transform.InverseTransformDirection(rb.velocity);
        float z = v.z * 3.6f;
        speed.text = String.Format("{0:###0.00} km/h", z);

        // MainCameraを車の後ろへ移動し、車の方へ向けます
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position,
                                                        transform.TransformPoint(cameraPosition),
                                                        0.2F);
        Camera.main.transform.LookAt(transform);
    }

    public void OnReset()
    {
        // [Reset]ボタンが押されたら、
        // シーンを再表示します
        SceneManager.LoadScene("Main");
    }
}