using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class sheet_weapon_status
{
	public Int32       KIND;
	public string      MEMO;
	public Int32       PHYSICS;
	public Int32       MAGIC;
	public Int32       HIT;
	public Int32       CRITICAL;
	public Int32       WEIGHT;
	public Int32       DODGE;
	public Int32       DODGE_CRITICAL;
	public Int32       RANGE;
	public Int32       RANGE_MIN;
}

[Serializable]
public class sheet_weapon_attribute
{
	public Int32       KIND;
	public string      MEMO;
	public Int32       ATTRIBUTE;
}

[Serializable]
public class sheet_weapon_localize
{
	public Int32       KIND;
	public string      MEMO;
	public string      NAME;
	public string      DESC;
}



[ExcelAsset]
public class sheet_weapon : ScriptableObject
{
	public List<sheet_weapon_status>    status;
	public List<sheet_weapon_attribute> attribute;
	public List<sheet_weapon_localize>  localize;
}
