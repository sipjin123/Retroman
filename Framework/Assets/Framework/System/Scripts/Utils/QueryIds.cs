using System.Collections;

using UnityEngine;

namespace Framework
{
    public abstract class QueryIds
    {
        // system
        public const string SystemCamera = "SystemCamera";
        public const string SystemState = "SystemState";
        public const string DevelopmentVersion = "DevelopmentVersion";
        public const string ReleaseVersion = "ReleaseVersion";
        public const string ShowVersion = "ShowVersion";
        public const string SystemEventSystem = "EventSystem";

        // scene
        public const string CurrentScene = "CurrentScene";
        public const string PreviousScene = "PreviousScene";
        public const string Preloader = "Preloader";
        
        // IAP
        public const string StoreIsReady = "StoreIsReady";
        public const string StoreItems = "StoreItems";
        public const string StoreItemsWithType = "StoreItemsWithType";
        public const string StoreItemType = "StoreItemType";
        public const string StoreItemId = "StoreItemId";
        public const string PurchaseInProgress = "PurchaseInProgress";

        // More Games
        public const string MoreGamesItems = "MoreGamesItems";
        public const string MoreGamesItemId = "MoreGamesItemId";

        //World
        public const string GameCamera = "GameCamera";
        public const string LevelToLoad = "LevelToLoad";
        public const string LevelToLoadUI = "LevelToLoadUI";
        public const string LevelToLoadDescription = "LevelToLoadDescription";
        public const string World = "World";
        public const string Level = "Level";
        public const string LevelBounds = "LevelBounds";
        public const string CurrentWorld = "CurrentWorld";
        public const string CurrentLevel = "CurrentLevel";
        public const string CurrentLevelWaveCount = "CurrentLevelWaveCount";
        public const string CurrentMapChunk = "CurrentMapChunk";
        public const string LatestLevelProgression = "LatestLevelProgression";
        public const string MaxLevelOfWorld = "MaxLevelOfWorld";
        public const string RecentlyPlayedLevel = "RecentlyPlayedLevel";
        public const string CurrentLevelWorldCompleted = "CurrentLevelWorldCompleted";
        public const string BookUIActive = "BookUIActive";
        public const string SelectedWorld = "SelectedWorld";
        public const string ToLoadWorld = "ToLoadWorld";

        //Coin Management
        public const string AvailableCoins = "AvailableCoins";

        //Object Pool
        public const string ObjectPool = "ObjectPool";
        public const string MapObjectPool = "MapObjectPool";
        public const string MapType = "MapType";
        public const string EnemyObjectPool = "EnemyObjectPool";
        public const string ItemObjectPool = "ItemObjectPool";
        public const string ProjectilePool = "ProjectilePool";
        public const string ProjectileType = "ProjectileType";
        public const string DestructableObjectPool = "DestructableObjectPool";
        public const string PrefabName = "PrefabName";


        //Player
        public const string PlayerGameObject = " PlayerGameObject";
        public const string PlayerBikerGameObject = " PlayerBikerGameObject";
        public const string SpineAnimation = "SpineAnimation";
        public const string AttackMessageBroker = "AttackMessageBroker";
        public const string DashMessageBroker = "DashBeginMessageBroker";
        public const string FlinchMessageBroker = "FlinchMessageBroker";
        public const string JumpMessageBroker = "JumpMessageBroker";
        public const string ComicIsPlaying = "ComicIsPlaying";
        public const string TutorialIsPlaying = "TutorialIsPlaying";
        public const string GameIsPaused = "GameIsPaused";
        public const string HitCount = "HitCount";

        //PlayerStats
        public const string PlayerCurrentHealth = "PlayerCurrentHealth";
        public const string PlayerMaxHealth = "PlayerMaxHealth";
        public const string PlayerRigidBody = "PlayerRigidbody";
        public const string PlayerEnemiesKilledForSkill = "PlayerEnemiesKilledForSkill";

        //PlayerStatusEffects
        public const string PlayerBuffsManager = "PlayerBuffsManager";
        public const string PlayerStatusEffectMB = "PlayerStatusEffectMB";

        //Enemy
        public const string EnemyLayerMask = "EnemyLayerMask";

        //EnemyStats
        public const string EnemyStat = "EnemyStat";
        public const string EnemyType = "EnemyType";
        public const string EnemyTypeStat = "EnemyTypeStat";
        public const string EnemyCharacterStat = "EnemyCharacterStat";

        //Custom Services
        public const string CoinManagerServices = "CoinManagerService";
        public const string PlayerStats = "PlayerStats";
        public const string PlayerUpgradeShopData = "PlayerUpgradeShopData";
        public const string IsUnityAdsPlaying = "IsUnityAdsPlaying";
        public const string FromRetry = "FromRetry";
        public const string AchievementData = "AchievementData";
        public const string AchievementId = "AchievementId";
        public const string AchievementsUncollected = "AchievementsUncollected";
        public const string UpgradesAvailable = "UpgradesAvailable";

        //TouchInput
        public const string TouchInputMB = "TouchInputMB";
        public const string DeathScreen = "DeathScreen";

        //NewQueryIDS 9/26/17
        public const string SettingsService = "SettingsService";
        public const string SettingsServicePauseBool = "SettingsServicePauseBool";
        public const string CharSkills = "CharSkills";
        public const string CharSkillsMessageBroker = "CharSkillsMessageBroker";
        public const string CharHealth = "CharHealth";
        public const string PreloaderLoadingProgress = "PreloaderLoadingProgress";
        public const string FreeCoinAmount = "FreeCoinAmount";
        public const string CoinAmount = "CoinAmount";

        public const string DebugBuild = "DebugBuild";
        public const string IsConnected = "IsConnected";
        public const string Connecting = "Connecting";
        public const string NetworkConnectionType = "NetworkConnectionType";
        public const string ExternalLinkID = "ExternalLinkID";
        public const string ShopCatalog = "ShopCatalog";
        public const string ChosenProductId = "ChosenProductId";
        public const string CountryCode = "CountryCode";

        //LevelProgression
        public const string TotalEnemiesKilled = "TotalEnemiesKilled";
        public const string TotalWavesCleared = "TotalWavesCleared";
        public const string TotalLevelsCleared = "TotalLevelsCleared";
        public const string PandaySwordUnlocked = "PandaySwordUnlocked";

        //InGame
        public const string IsInGame = "IsInGame";

        //Collections
        public const string CollectionRecordsArray = "CollectionRecordsArray";
        public const string CollectionRecord = "CollectionRecord";
        public const string CollectionRecordID = "CollectionRecordID";

        //Collections
        public const string LevelRewardsArray = "LevelRewardsArray";
        public const string LevelUniqueReward = "LevelReward";
        public const string LevelRewardWorld = "LevelRewardWorld";
        public const string LevelRewardLevel = "LevelRewardLevel";
        public const string LevelContainsUniqueReward = "LevelContainsReward";
        public const string LevelCoinRewards = "LevelCoinRewards";
        public const string UnlockedRewards = "UnlockedRewards";
        public const string RewardType = "RewardType";

        //External Links
        public const string ExternalLinksArray = "ExternalLinksArray";

        //WorldChange
        public const string IncrementedWorld = "IncrementedWorld";
        public const string EnableChestCollectionReward = "EnableChestCollectionReward";

        //Input
        public const string KeyboardBroker = "KeyboardBroker";

        //Debug
        public const string DebugLevelToLoad = "DebugLevelToLoad";
        public const string DebugWorldToLoad = "DebugWorldToLoad";

        //LoadingScreens
        public const string OnePagerPreloader = "OnePagerPreloader";
        public const string OnePagerPreloaderImage = "OnePagerPreloaderImage";
        public const string WorldPreloader = "WorldPreloader";
        public const string WorldPreloaderImage = "WorldPreloaderImage";
        
        //OS Related
        public const string AndroidSDKVersion = "AndroidSDKVersion";
        public const string IOsDeviceGeneration = "IOsDeviceGeneration";
    }
}