using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardLogistInfo
{
	
	public Vector3 position;
	public Quaternion rot;
	


	public CardLogistInfo (Vector3 v, Quaternion q)
	{
		position = v;
		rot = q;
	}

}

