using UnityEngine;
using System.Collections;

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

public enum PlatformRunnability
{
    Null,
    CanRun,
    MustJump
}
public class PlatformMinion : MonoBehaviour {

    public PlatformMinion PrePlatform;
    public void SetBeforePlatform(PlatformMinion platminion)
    {
        PrePlatform = platminion;
    }
    public void SetTypeOfPlatform(TypeofPlatform platformType)
    {
        if ( platformType == TypeofPlatform.HOLED ||
            platformType == TypeofPlatform.LOWHOLE ||
            platformType == TypeofPlatform.SPIKED ||
            platformType == TypeofPlatform.UNKNOWN ||
            platformType == TypeofPlatform.UPSPIKE)
        {
            _PlatformRunnability = PlatformRunnability.MustJump;
        }
        else if (
            platformType == TypeofPlatform.LEFT ||
            platformType == TypeofPlatform.LOWGROUND ||
            platformType == TypeofPlatform.NORMAL ||
            platformType == TypeofPlatform.COINBOX ||
            platformType == TypeofPlatform.RIGHT ||
            platformType == TypeofPlatform.UPGROUND )
        {

            _PlatformRunnability = PlatformRunnability.CanRun;
        }

        _TypeOfPlatform = platformType;
    }
    public TypeofPlatform TypeOfPlatform { get { return _TypeOfPlatform; } }
    private	TypeofPlatform _TypeOfPlatform;


    public TypeofPlatform _previousTypeOfPlatform;
	public enum HeightOfPlatform
	{
		LOW,
		NORMAL,
		HIGH,
		VERYHIGH
	}
	public HeightOfPlatform _heightOfPlatform;

    [SerializeField]
    PlatformRunnability _PlatformRunnability;
    public PlatformRunnability PlatformRunnability { get { return _PlatformRunnability; } }

    public GameObject _meshParent;
	public GameObject _trapParent;

	public GameObject  _spikeObject, _holeObject, _coinBoxObject, _coinBoxMesh, _coinBoxEffects, _leftObject, _rightObject;
	public GameObject _higherPlatform, _lowerPlatform;

	public GameObject _leftTreeObject,_rightTreeObject, _shadowObject;

    [SerializeField]
	BoxCollider _boxCollider;

    
	void Awake()
	{
        int randomizedVAlue = Random.Range(0, 4);
        if (randomizedVAlue == 1)
        {
            _leftTreeObject.SetActive(true);
        }
        else if (randomizedVAlue == 2)
        {
            _rightTreeObject.SetActive(true);
        }
        else if (randomizedVAlue == 3)
        {
            _rightTreeObject.SetActive(true);
            _leftTreeObject.SetActive(true);
        }

	}

	public void UpdateThisPlatform()
    {
        _boxCollider.enabled = false;
        _boxCollider.enabled = true;
		DisableAll();
		switch(_TypeOfPlatform)
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
				_boxCollider.enabled = false;
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
				_coinBoxMesh.SetActive(true);
				_coinBoxEffects.SetActive(false);
				break;
			case TypeofPlatform.LEFT:
				_leftObject.SetActive(true);
                _leftObject.transform.GetChild(0).gameObject.SetActive(true);

                _leftObject.GetComponent<BoxCollider>().enabled = true;
                _leftObject.transform.GetChild(0).GetComponent<BoxCollider>().enabled = true;

                _leftObject.GetComponent<MeshRenderer>().material.color = Color.red;
                _leftObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.red;
                break;
			case TypeofPlatform.RIGHT:
				_rightObject.SetActive(true);
                _rightObject.transform.GetChild(0).gameObject.SetActive(true);

                _rightObject.GetComponent<BoxCollider>().enabled = true;
                _rightObject.transform.GetChild(0).GetComponent<BoxCollider>().enabled = true;

                _rightObject.GetComponent<MeshRenderer>().material.color = Color.blue;
                _rightObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.blue;
                break;
		}

		switch(_heightOfPlatform)
		{
			case HeightOfPlatform.LOW:
				_lowerPlatform.SetActive(true);
                transform.position = new Vector3( transform.position.x, -1.5f, transform.position.z);
                //transform.localEulerAngles = new Vector3(15, transform.position.y, transform.position.z);
                break;
			case HeightOfPlatform.NORMAL:
				_higherPlatform.SetActive(true);
                transform.position = new Vector3( transform.position.x, 0, transform.position.z);
                //transform.localEulerAngles = new Vector3(15, transform.position.y, transform.position.z);
                break;
			case HeightOfPlatform.HIGH:
				_higherPlatform.SetActive(true);
                transform.position = new Vector3( transform.position.x, 1.5f, transform.position.z);
                //transform.localEulerAngles = new Vector3(15, transform.position.y, transform.position.z);
                break;
			case HeightOfPlatform.VERYHIGH:
				_lowerPlatform.SetActive(true);
                transform.position = new Vector3( transform.position.x, 3, transform.position.z);
                //transform.localEulerAngles = new Vector3(15, transform.position.y, transform.position.z);
                break;
		}

		if(_TypeOfPlatform == TypeofPlatform.UPGROUND || _TypeOfPlatform == TypeofPlatform.UPSPIKE)
		{
            transform.position = new Vector3( transform.position.x, transform.position.y +1.5f, transform.position.z);
            //transform.localEulerAngles = new Vector3(15, transform.position.y, transform.position.z);
        }
		else if(_TypeOfPlatform == TypeofPlatform.LOWGROUND || _TypeOfPlatform == TypeofPlatform.LOWHOLE)
		{
            transform.position = new Vector3( transform.position.x, transform.position.y -1.5f, transform.position.z);
            //transform.localEulerAngles = new Vector3(15, transform.position.y, transform.position.z);
        }

		_previousTypeOfPlatform = _TypeOfPlatform;

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
				_boxCollider.enabled = true;
				_holeObject.SetActive(false);
				break;
			case TypeofPlatform.LOWHOLE:
				_meshParent.SetActive(true);
				_boxCollider.enabled = true;
				_holeObject.SetActive(false);
				break;
			case TypeofPlatform.COINBOX:
				_coinBoxMesh.SetActive(false);
				_coinBoxEffects.SetActive(false);
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



	}
}
