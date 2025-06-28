using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    /// <summary>  
    /// プレイヤー  
    /// </summary>  
    [SerializeField] private Player player_ = null;

	bool isViewing = false;

    /// <summary>  
    /// ワールド行列   
    /// </summary>  
    private Matrix4x4 worldMatrix_ = Matrix4x4.identity;

    /// <summary>  
    /// ターゲットとして設定する  
    /// </summary>  
    /// <param name="enable">true:設定する / false:解除する</param>  
    public void SetTarget(bool enable)
    {
        // マテリアルの色を変更する  
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.materials[0].color = enable ? Color.red : Color.white;
    }

	/// <summary>
	/// 開始処理
	/// </summary>
	public void Start()
    {
    }

    /// <summary>  
    /// 更新処理  
    /// </summary>  
    public void Update()
    {
		Viewing();
		Focus();
		Tracking();
    }

	//-----視野角-----
	void Viewing()
	{
		var normalZ = new Vector3(0, 0, -1);
		var playerForward = player_.worldMatrix * normalZ;

		var playerViewCos = Mathf.Cos(player_.viewRadian);

		var playerToEnemy = (this.transform.position - player_.transform.position).normalized;

		var dot = Vector3.Dot(playerForward, playerToEnemy);

		Player target_ = null;

		if (playerViewCos <= dot)
		{
			target_ = player_;
			isViewing = true;
		}
	}

	//-----フォーカス-----
	void Focus()
	{
		if (isViewing)
		{
			var toTarget = (player_.transform.position - this.transform.position).normalized;
			var foward = transform.forward;

			var dot = Vector3.Dot(foward, toTarget);
			if (0.999f < dot) { return; }

			var radian = Mathf.Acos(dot);

			var cross = Vector3.Cross(foward, toTarget);
			radian *= (cross.y / Mathf.Abs(cross.y));

			transform.rotation *= Quaternion.Euler(0, Mathf.Rad2Deg * radian, 0);
		}
	}
	//--------追跡--------
	void Tracking()
	{
		var dist = player_.transform.position - this.transform.position;
		var length = Mathf.Sqrt((dist.x * dist.x) + (dist.y * dist.y) + (dist.z * dist.z));

		if(isViewing)
		{
			var vector = dist / length;

			vector *= (length / 20.0f);

			transform.position += vector;
		}
	}
}
