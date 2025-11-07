using System;
using System.Collections;
using BloomLines.Assets;
using BloomLines.Boards;
using BloomLines.Controllers;
using BloomLines.Cursor;
using BloomLines.Managers;
using BloomLines.Tools;
using BloomLines.UI;
using UnityEngine;

namespace BloomLines.Tutorial
{
    public class FirstGameTutorial : TutorialBase
    {
        [SerializeField] private UIConnectionTypeTab _connectionTypeTab;
        [SerializeField] private GameObject _currentConnectionTypeObj;
        [SerializeField] private GameObject[] _connectionTypeTabs;

        private GameModeType _startGameModeType;
        private Board _board;
        private GameModeController _gameModeController;

        protected override void Awake()
        {
            base.Awake();

            _board = FindAnyObjectByType<Board>();
            _gameModeController = FindAnyObjectByType<GameModeController>();
        }

        public override void StartTutorial(Action<TutorialBase> onComplete)
        {
            base.StartTutorial(onComplete);
            AnalyticsController.SendEvent("tutorial_start");
            StartCoroutine(Step_1());
        }

        private void ResetTools()
        {
            var gameModeState = SaveManager.GameModeState;

            var pickaxeData = GameAssets.GetToolData(ToolType.Pickaxe) as EvolveRequiredToolData;
            var secateursData = GameAssets.GetToolData(ToolType.Secateurs) as CrystalRequiredToolData;

            var pickaxeState = gameModeState.GetToolState(ToolType.Pickaxe);
            var secateursState = gameModeState.GetToolState(ToolType.Secateurs);

            pickaxeState.Data = pickaxeData.EvolveCountRequired.ToString();
            secateursState.Data = (secateursData.CrystalCountRequired - 1).ToString();
        }

        private IEnumerator WaitClick(Transform target, Transform handTarget)
        {
            ShowHand(handTarget.position);

            yield return StartCoroutine(WaitClick(target.gameObject));

            HideHand();

            yield return new WaitForSeconds(0.2f);
        }
        
        private IEnumerator Step_1()
        {
            _startGameModeType = SaveManager.GameModeState.Type;

            _gameModeController.StartGame(GameModeType.Adventure, true);

            ResetTools();

            EventsManager.Publish(new StartGameModeEvent(SaveManager.GameModeState, true));
            EventsManager.Publish(new SetConnectionTypeEvent(ConnectionType.Line4));

            var spawnInTiles = new int[] { 11, 26, 28, 29 };

            foreach (var tileIndex in spawnInTiles)
            {
                var tile = _board.Tiles[tileIndex];
                var boardObject = _board.SpawnNewObjectInTile("flower_pink", tile);
                boardObject.OnSpawn();
            }
            
            _board.SpawnNewObjectInTile("flower_orange", _board.Tiles[27]).OnSpawn();     

            yield return new WaitForSeconds(0.75f);
            Debug.Log("Evolve");
            _board.Evolve(false, false);

            spawnInTiles = new int[] { 13, 21, 24, 37 };

            for (int i = 0; i < spawnInTiles.Length; i++)
            {
                var tile = _board.Tiles[spawnInTiles[i]];
                var boardObject = _board.SpawnNewObjectInTile("flower_purple", tile);
                boardObject.OnSpawn();
            }            

            yield return new WaitForSeconds(0.5f);

            var firstClickTile = _board.Tiles[11];
            var secondClickTile = _board.Tiles[27];

            yield return StartCoroutine(WaitClick(firstClickTile.RectTransform, firstClickTile.RectTransform));
            AnalyticsController.SendEvent("tutorial_step1_click1");
            yield return StartCoroutine(WaitClick(secondClickTile.RectTransform, secondClickTile.RectTransform));
            AnalyticsController.SendEvent("tutorial_step1_click2");
            yield return new WaitForSeconds(2.3f);

            StartCoroutine(Step_2());
        }

        private IEnumerator Step_2()
        { 
            _board.Evolve(false, false);

            yield return new WaitForSeconds(0.5f);

            _board.SpawnNewObjectInTile("flower_white", _board.Tiles[9]).OnSpawn();
            _board.SpawnNewObjectInTile("flower_white", _board.Tiles[18]).OnSpawn();
            _board.SpawnNewObjectInTile("flower_white", _board.Tiles[14]).OnSpawn();
            _board.SpawnNewObjectInTile("flower_white", _board.Tiles[36]).OnSpawn();            

            yield return new WaitForSeconds(0.5f);

            var firstClickTile = _board.Tiles[24];
            var secondClickTile = _board.Tiles[29];

            yield return StartCoroutine(WaitClick(firstClickTile.RectTransform, firstClickTile.RectTransform));
            AnalyticsController.SendEvent("tutorial_step2_click1");
            yield return StartCoroutine(WaitClick(secondClickTile.RectTransform, secondClickTile.RectTransform));
            AnalyticsController.SendEvent("tutorial_step2_click2");

            yield return new WaitForSeconds(2.5f);

            StartCoroutine(Step_3());
        }

        private IEnumerator Step_3()
        {
            _board.Evolve(false, false);
            var spawnInTiles = new int[] { 1, 2, 5, 6, 8, 15, 16, 23, 25, 30, 32, 39, 40, 47, 49, 50, 53, 54 };

            for (int i = 0; i < spawnInTiles.Length; i++)
            {
                var tile = _board.Tiles[spawnInTiles[i]];
                var boardObject = _board.SpawnNewObjectInTile("weed", tile);
                boardObject.OnSpawn();
            }

            _board.SpawnNewObjectInTile("flower_crystal", _board.Tiles[19]).OnSpawn();
            
            yield return new WaitForSeconds(1.0f);  

            var firstClickTile = _board.Tiles[14];
            var secondClickTile = _board.Tiles[27];

            yield return StartCoroutine(WaitClick(firstClickTile.RectTransform, firstClickTile.RectTransform));
            AnalyticsController.SendEvent("tutorial_step3_click1");
            yield return StartCoroutine(WaitClick(secondClickTile.RectTransform, secondClickTile.RectTransform));
            AnalyticsController.SendEvent("tutorial_step3_click2");

            yield return new WaitForSeconds(2.5f);

            StartCoroutine(Step_4());
        }        

        private IEnumerator Step_4()
        {  
            _board.Evolve(false, false);            

            yield return new WaitForSeconds(0.5f);

            var pickaxeTool = FindAnyObjectByType<PickaxeTool>(FindObjectsInactive.Exclude);
            yield return StartCoroutine(WaitClick(pickaxeTool.transform, pickaxeTool.Icon.rectTransform));

            AnalyticsController.SendEvent("tutorial_step4_click1");

            var pickaxeTile = _board.Tiles[19];
            yield return StartCoroutine(WaitClick(pickaxeTile.RectTransform, pickaxeTile.RectTransform));
            AnalyticsController.SendEvent("tutorial_step4_click2");
            RemoveHighlight(_board.Tiles[19].gameObject);
            HighlightObject(_board.Tiles[19].gameObject);

            yield return new WaitForSeconds(2f);
            
            var secateursTool = FindAnyObjectByType<SecateursTool>(FindObjectsInactive.Exclude);
            yield return StartCoroutine(WaitClick(secateursTool.transform, secateursTool.Icon.rectTransform));
            AnalyticsController.SendEvent("tutorial_step4_click3");
            RemoveHighlight(_board.Tiles[19].gameObject);

            var secateursTile = _board.Tiles[30];

            bool toolUsed = false;
            void OnUseTool(UseToolEvent eventData) => toolUsed = true;

            var spawnInTiles = new int[] { 1, 2, 5, 6, 8, 15, 16, 23, 25, 30, 32, 39, 40, 47, 49, 50, 53, 54 };
            foreach (var tileIndex in spawnInTiles)
                HighlightObject(_board.Tiles[tileIndex].gameObject);

            EventsManager.Subscribe<UseToolEvent>(OnUseTool);
            ShowHand(secateursTile.RectTransform.position);
            AnalyticsController.SendEvent("tutorial_step4_click4");
            while (!toolUsed)
                yield return null;

            foreach (var tileIndex in spawnInTiles)
                RemoveHighlight(_board.Tiles[tileIndex].gameObject);

            EventsManager.Unsubscribe<UseToolEvent>(OnUseTool);
            HideHand();            
            yield return new WaitForSeconds(2.3f);
            AnalyticsController.SendEvent("tutorial_complete");
            CompleteTutorial();

            _gameModeController.StartGame(_startGameModeType, true);            
        }

        private void CheckCanSaveEvent(CheckCanSaveEvent eventData)
        {
            if (eventData.SaveType == SaveType.GameMode)
            {
                var tutorialCompleted = TutorialController.IsCompleted(_id);
                if (!tutorialCompleted)
                    eventData.CanSave = false;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            EventsManager.Subscribe<CheckCanSaveEvent>(CheckCanSaveEvent);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            EventsManager.Unsubscribe<CheckCanSaveEvent>(CheckCanSaveEvent);
        }
    }
}