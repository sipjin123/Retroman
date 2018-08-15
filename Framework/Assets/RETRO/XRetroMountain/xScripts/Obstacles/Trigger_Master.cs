using UnityEngine;
using System.Collections;
using Common.Utils;

namespace Retroman
{

    public class Trigger_Master : MonoBehaviour
    {


        public enum TypeOfTrigger
        {
            COIN,
            SPIKE,
            LEFT,
            RIGHT,
            COUNTER,
            WATER
        }
        public TypeOfTrigger _typeOfTrigger;

        public GameObject _coinMesh, _coinEffects;

        void OnEnable()
        {
            if (_typeOfTrigger == TypeOfTrigger.COIN)
            {
                _coinMesh.SetActive(true);
                _coinEffects.SetActive(false);
            }
        }
        void OnTriggerEnter(Collider hit)
        {
          //  Debug.LogError("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< TRIGGERED OBJECT :: " + hit.gameObject.name + " :: " + hit.gameObject.tag + " :: " + hit.gameObject.layer);
            if (hit.name == "Player")
            {
                switch (_typeOfTrigger)
                {
                    case TypeOfTrigger.COIN:


                        _coinMesh.SetActive(false);
                        _coinEffects.SetActive(true);
                        if (gameObject.name == "CoinBox")
                        {
                            SoundControls.Instance._sfxBreakBlock.Play();
                        }
                        if (gameObject.name == "CoinOBJ")
                        {
                            SoundControls.Instance._sfxCoin.Play();
                            Factory.Get<DataManagerService>().MessageBroker.Publish(new AddScore { ScoreToAdd = 1 });
                            Factory.Get<DataManagerService>().MessageBroker.Publish(new UpdateScore());
                        }
                        break;
                    case TypeOfTrigger.SPIKE:
                        Factory.Get<DataManagerService>().MessageBroker.Publish(new GameOver());
                        SoundControls.Instance._sfxSpikes.Play();
                        break;
                    case TypeOfTrigger.LEFT:
                        Factory.Get<DataManagerService>().MessageBroker.Publish(new UpdatePlayerAction { PlayerAction = PlayerControls.PlayerAction.TURNLEFT });
                        //Factory.Get<DataManagerService>().PlayerControls._playerAction = PlayerControls.PlayerAction.TURNLEFT;
                        break;
                    case TypeOfTrigger.RIGHT:
                        Factory.Get<DataManagerService>().MessageBroker.Publish(new UpdatePlayerAction { PlayerAction = PlayerControls.PlayerAction.TURNLEFT });
                        //Factory.Get<DataManagerService>().PlayerControls._playerAction = PlayerControls.PlayerAction.TURNLEFT;
                        break;
                    case TypeOfTrigger.WATER:
                        SoundControls.Instance._sfxSplash.Play();

                        Factory.Get<DataManagerService>().MessageBroker.Publish(new SetupPlayerSplash { IfActive = true});
                        //Factory.Get<DataManagerService>().PlayerControls._splash.SetActive(true);
                        //Factory.Get<DataManagerService>().PlayerControls._splash.transform.position = Factory.Get<DataManagerService>().PlayerControls._deathAnim.transform.GetChild(0).transform.position;
                        //Factory.Get<DataManagerService>().GameControls.GameOverIT();
                        Factory.Get<DataManagerService>().MessageBroker.Publish(new GameOver());
                        break;
                    case TypeOfTrigger.COUNTER:
                        Factory.Get<DataManagerService>().MessageBroker.Publish(new SpawnAPlatform());
                        break;
                }
            }

        }
    }
}