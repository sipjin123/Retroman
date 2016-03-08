using UnityEngine;
using System.Collections;

public class PlatformMinion : MonoBehaviour {


	public enum TypeofPlatform
	{
		NORMAL,
		SPIKED,
		HOLED,
		UPSPIKE,
		LOWHOLE,
		UPGROUND,
		LOWGROUND,
		COINBOX,
		LEFT,
		RIGHT,
		UNKNOWN
	}
	public TypeofPlatform _typeOfPlatform;
	public TypeofPlatform _previousTypeOfPlatform;
	public enum HeightOfPlatform
	{
		NORMAL,
		LOW,
		HIGH,
		VERYHIGH
	}
	public HeightOfPlatform _heightOfPlatform;



	public GameObject _meshParent;
	public GameObject _trapParent;

	public GameObject _coinObject, _spikeObject, _holeObject, _coinBoxObject, _leftObject, _rightObject;
	public GameObject _higherPlatform, _lowerPlatform;

	public GameObject _treeObject, _shadowObject;

	public bool _hasTree;

	void OnDisable()
	{

		_hasTree = false;
	}
	public void UpdateThisPlatform()
	{
		DisableAll();

		switch(_typeOfPlatform)
		{
			case TypeofPlatform.UNKNOWN:
				return;
				break;
			case TypeofPlatform.SPIKED:
				_spikeObject.SetActive(true);
				break;
			case TypeofPlatform.HOLED:
				_holeObject.SetActive(true);
				_meshParent.SetActive(false);
				GetComponent<BoxCollider>().enabled = false;
				break;
			case TypeofPlatform.UPSPIKE:
				_spikeObject.SetActive(true);
				break;
			case TypeofPlatform.LOWHOLE:
				_holeObject.SetActive(true);
				_meshParent.SetActive(false);
				GetComponent<BoxCollider>().enabled = false;
				break;
			case TypeofPlatform.COINBOX:
				_coinBoxObject.SetActive(true);
				break;
			case TypeofPlatform.LEFT:
				_leftObject.SetActive(true);
				break;
			case TypeofPlatform.RIGHT:
				_rightObject.SetActive(true);
				break;
		}

		switch(_heightOfPlatform)
		{
			case HeightOfPlatform.LOW:
				_lowerPlatform.SetActive(true);
				transform.position = new Vector3( transform.position.x, -1.5f, transform.position.z);
				break;
			case HeightOfPlatform.NORMAL:
				_higherPlatform.SetActive(true);
				transform.position = new Vector3( transform.position.x, 0, transform.position.z);
				break;
			case HeightOfPlatform.HIGH:
				_higherPlatform.SetActive(true);
				transform.position = new Vector3( transform.position.x, 1.5f, transform.position.z);
				break;
			case HeightOfPlatform.VERYHIGH:
				_lowerPlatform.SetActive(true);
				transform.position = new Vector3( transform.position.x, 3, transform.position.z);
				break;
		}
		if(_typeOfPlatform == TypeofPlatform.UPGROUND || _typeOfPlatform == TypeofPlatform.UPSPIKE)
		{
			transform.position = new Vector3( transform.position.x, transform.position.y +1.5f, transform.position.z);
		}
		else if(_typeOfPlatform == TypeofPlatform.LOWGROUND || _typeOfPlatform == TypeofPlatform.LOWHOLE)
		{
			transform.position = new Vector3( transform.position.x, transform.position.y -1.5f, transform.position.z);
		}

		_previousTypeOfPlatform = _typeOfPlatform;
		if(_hasTree)
			_treeObject.SetActive(true);
		_shadowObject.transform.position = new Vector3 ( _shadowObject.transform.position.x, -3 , _shadowObject.transform.position.z );
	}
	public void DisableAll()
	{
		switch(_previousTypeOfPlatform)
		{
			case TypeofPlatform.SPIKED:
				_spikeObject.SetActive(false);
				break;
			case TypeofPlatform.UPSPIKE:
				_spikeObject.SetActive(false);
				break;
			case TypeofPlatform.HOLED:
				_meshParent.SetActive(true);
				GetComponent<BoxCollider>().enabled = true;
				_holeObject.SetActive(false);
				break;
			case TypeofPlatform.LOWHOLE:
				_meshParent.SetActive(true);
				GetComponent<BoxCollider>().enabled = true;
				_holeObject.SetActive(false);
				break;
			case TypeofPlatform.COINBOX:
				_coinBoxObject.SetActive(false);
				break;
			case TypeofPlatform.LEFT:
				_leftObject.SetActive(false);
				break;
			case TypeofPlatform.RIGHT:
				_rightObject.SetActive(false);
				break;
		}
		_higherPlatform.SetActive(false);
		_lowerPlatform.SetActive(false);

		_treeObject.SetActive(false);

	}
}
