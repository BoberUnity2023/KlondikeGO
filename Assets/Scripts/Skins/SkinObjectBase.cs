using BloomLines.Assets;
using BloomLines.Managers;
using UnityEngine;

namespace BloomLines.Skins
{
    public class UpdateSkinPackEvent
    {  
    }

    public abstract class SkinObjectBase : MonoBehaviour
    {
        protected abstract void SetSkin(SkinPackData skinPack);

        private void UpdateSkin()
        {
            var gameState = SaveManager.GameState;
            var skinPack = GameAssets.GetSkinPackData(gameState.SkinPack);

            SetSkin(skinPack);
        }

        private void OnUpdateSkinPackEvent(UpdateSkinPackEvent eventData)
        {
            UpdateSkin();
        }

        private void OnEnable()
        {
            EventsManager.Subscribe<UpdateSkinPackEvent>(OnUpdateSkinPackEvent);

            UpdateSkin();
        }

        private void OnDisable()
        {
            EventsManager.Unsubscribe<UpdateSkinPackEvent>(OnUpdateSkinPackEvent);
        }
    }
}