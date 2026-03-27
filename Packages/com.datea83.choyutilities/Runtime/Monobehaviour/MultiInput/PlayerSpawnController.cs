using System;
using EugeneC.Singleton;

namespace EugeneC.Utilities {

    public class PlayerSpawnController : GenericSingleton<PlayerSpawnController> {

        private event Action<bool> OnAllowNewPlayers;

        public void SubOnAllowNewPlayers(Action<bool> sub) {
            OnAllowNewPlayers += sub;
        }

        public void UnsubOnAllowNewPlayers(Action<bool> unsub) {
            OnAllowNewPlayers -= unsub;
        }

        public void OnAllowNewPlayersEvent(bool cast) {
            OnAllowNewPlayers?.Invoke(cast);
        }

        private event Action<bool> OnAbleControls;

        public void SubOnAbleControls(Action<bool> sub) {
            OnAbleControls += sub;
        }

        public void UnsubOnAbleControls(Action<bool> unsub) {
            OnAbleControls -= unsub;
        }

        public void OnAbleControlsEvent(bool able) {
            OnAbleControls?.Invoke(able);
        }

        private event EventHandler OnResetGame;

        public void SubOnResetGame(EventHandler sub) {
            OnResetGame += sub;
        }

        public void UnsubOnResetGame(EventHandler unsub) {
            OnResetGame -= unsub;
        }

        public void OnResetGameEvent() {
            OnResetGame?.Invoke(this, EventArgs.Empty);
        }

    }

}