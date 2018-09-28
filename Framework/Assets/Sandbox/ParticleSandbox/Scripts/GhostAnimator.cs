using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Retroman
{

	public class GhostAnimator : MonoBehaviour 
	{
		
		public List<Sprite> charSprite;	
		public PlayerControls pc;
		public Animation ghostAnim;
		public Transform ghostParent;
		public Transform deathCube;
		// Use this for initialization
		void Start () 
		{
			GetPlayerType();
		}
		
		void GetPlayerType()
		{
			SpriteRenderer sr;
			sr = this.GetComponent<SpriteRenderer>();
			switch (pc._playerType)
			{
				case Retroman.PlayerType.NORMAL:
					sr.sprite = charSprite[0];
					break;
				case Retroman.PlayerType.CAT:
					sr.sprite = charSprite[1];
					break;
				case Retroman.PlayerType.DONKEYKONG:
					sr.sprite = charSprite[2];
					break;
				case Retroman.PlayerType.SONIC:
					sr.sprite = charSprite[3];
					break;
				case Retroman.PlayerType.UNICORN:
					sr.sprite = charSprite[4];
					break;
				case Retroman.PlayerType.YOSHI:
					sr.sprite = charSprite[5];
					break;
					
				default:
					break;
			}	
		}
		public void StartAnimation(bool isRight)
		{
			SpriteRenderer sr;
			sr = this.GetComponent<SpriteRenderer>();
			sr.flipX=isRight;
			ghostParent.transform.position = deathCube.transform.position;
			ghostAnim.Play();
		}

	
	}
}