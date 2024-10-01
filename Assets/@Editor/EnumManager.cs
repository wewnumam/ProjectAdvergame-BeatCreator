namespace ProjectAdvergame.Utility
{
    [System.Serializable]
    public class EnumManager
    {
        public enum BeatAccuracy
        {
            Early,
            Perfect,
            Late
        }

        public enum Direction
        {
            FromEast,
            FromWest,
            FromNorth,
            CloseUp
        }

        public enum GameState
        {
            PreGame,
            Playing,
            Pause,
            GameOver,
            GameWin
        }

        public enum StoneType
        {
            Normal,
            AddHealth,
            LongBeat
        }

        public enum TweenerMovementType
        {
            LOOP_ON_AWAKE,
            ONE_WAY,
        }
    }
}