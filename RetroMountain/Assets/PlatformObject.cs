using UnityEngine;
using System.Collections;

public class PlatformObject : MonoBehaviour {

	public enum TypeOfPlatform
	{
		UNKNOWN,
		NORMAL,
		SPIKED,
		HOLED,
		COINED,
		RIGHT,
		LEFT,
		RECYCLE,
		ENEMY,
		SPRING,
		FALLBRIDGE,
		CATCHER
	}
	public TypeOfPlatform _typeOfPlatform ;
	public TypeOfPlatform _previousTypeOfPlatform ;

	public GameObject _spikeObject, _coinObject, _rightObject, _leftObject, _holeObject, _enemyObject, _springObject, _fallBridgeObject, _catcherObject, _fallStopperObject;

	public void UpdateThisPlatform()
	{
		switch(_typeOfPlatform)
		{
			case TypeOfPlatform.NORMAL:
				break;

			case TypeOfPlatform.SPRING:
				_springObject.SetActive(true);
				break;

			case TypeOfPlatform.CATCHER:
				_catcherObject.SetActive(true);
				break;

			case TypeOfPlatform.FALLBRIDGE:
				_fallBridgeObject.SetActive(true);
				GetComponent<BoxCollider>().enabled = false;
				GetComponent<MeshRenderer>().enabled = false;
				_fallStopperObject.SetActive(false);
				break;

			case TypeOfPlatform.SPIKED:
				_spikeObject.SetActive(true);
				break;

			case TypeOfPlatform.HOLED:
				_holeObject.SetActive(true);
				GetComponent<MeshRenderer>().enabled = false;
				GetComponent<BoxCollider>().enabled = false;
				break;

			case TypeOfPlatform.COINED:
				_coinObject.SetActive(true);
				break;

			case TypeOfPlatform.RIGHT:
				_rightObject.SetActive(true);
				break;

			case TypeOfPlatform.LEFT:
				_leftObject.SetActive(true);
				break;

			case TypeOfPlatform.ENEMY:
				_enemyObject.SetActive(true);
				break;

			case TypeOfPlatform.RECYCLE:
				PlatformManager.Instance._platformSpawnedParent.transform.GetChild(0).gameObject.SetActive(false);
				PlatformManager.Instance._platformSpawnedParent.transform.GetChild(0).transform.parent = PlatformManager.Instance._platformPoolParent.transform;
				
				GetComponent<BoxCollider>().enabled = true;
				GetComponent<MeshRenderer>().enabled = true;
				_fallStopperObject.SetActive(true);


				switch(_previousTypeOfPlatform)
				{
					case TypeOfPlatform.ENEMY:
						_enemyObject.GetComponent<TriggerableObjects>()._path1.SetActive(false);
						_enemyObject.GetComponent<TriggerableObjects>()._path2.SetActive(false);
						_enemyObject.GetComponent<TriggerableObjects>()._enemyObject.SetActive(false);
						_enemyObject.GetComponent<TriggerableObjects>()._path1.transform.localPosition = new Vector3(0,0,0);
						_enemyObject.GetComponent<TriggerableObjects>()._path2.transform.localPosition = new Vector3(0,0,0);
						_enemyObject.GetComponent<TriggerableObjects>()._enemyObject.transform.localPosition = new Vector3(0,0,0);
						_enemyObject.SetActive(false);
						break;

					case TypeOfPlatform.COINED:	
						_coinObject.SetActive(false);
						_coinObject.GetComponent<MeshRenderer>().material.color = Color.white;
						_coinObject.GetComponent<TriggerableObjects>()._coinObject.SetActive(false);
						break;

					case TypeOfPlatform.FALLBRIDGE:
						for(int i = 0 ; i < 3 ; i++)
						{
							_fallBridgeObject.GetComponent<TriggerableObjects>()._fallBridges[i].SetActive(false);
						}
						_fallBridgeObject.SetActive(false);
						break;

					case TypeOfPlatform.RIGHT:
						_rightObject.SetActive(false);
						break;

					case TypeOfPlatform.LEFT:
						_leftObject.SetActive(false);
						break;
				
					case TypeOfPlatform.SPIKED:
						_spikeObject.SetActive(false);
						_spikeObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.white;
						break;
				
					case TypeOfPlatform.SPRING:
						_springObject.SetActive(false);
						break;

					case TypeOfPlatform.CATCHER:
						_catcherObject.GetComponent<TriggerableObjects>()._catcherTrigger.SetActive(false);
						_catcherObject.GetComponent<TriggerableObjects>()._catcherObject.SetActive(false);
						_catcherObject.SetActive(false);
						break;
				}
				_typeOfPlatform = TypeOfPlatform.UNKNOWN;
				_previousTypeOfPlatform = TypeOfPlatform.UNKNOWN;
				break;
		}
	}
}
