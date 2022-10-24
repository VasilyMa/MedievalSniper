using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    public static class Tutorial
    {
        public static Stage CurrentStage { get; private set; } = (Stage)0;

        public static bool StageIsEnable { get; set; } = false;

        public enum Stage
        {
            FirstLaunching = 0,
            ArrowControle = 1,

            TutorialsOver = 2,
        }

        #region Servises
        public static void SetNextStage(EcsSharedInject<GameState> gameState, bool isSave = true)
        {
            int nextStage = (int)CurrentStage + 1;

            if (nextStage > (int)Stage.TutorialsOver)
            {
                Debug.LogWarning("You try set next tutorial stage on last stage.");
            }
            else
            {
                CurrentStage = (Stage)nextStage;
                gameState.Value.TutorialStage = nextStage;

                if (isSave)
                {
                    SaveManager.SaveTutorialStage(nextStage);
                }

                Debug.Log("Change stage on " + CurrentStage);
            }
        }

        public static bool isOver()
        {
            return ((int)CurrentStage >= (int)Stage.TutorialsOver);
        }

        public static bool StageIsTutorialOver(Stage stage)
        {
            return ((int)stage >= (int)Stage.TutorialsOver);
        }

        public static bool StageIsTutorialOver(int stage)
        {
            return (stage >= (int)Stage.TutorialsOver);
        }

        public static void LoadCurrentStage(int currentStage)
        {
            CurrentStage = (Stage)currentStage;
        }

        public static bool isStage(Stage stage)
        {
            return stage == CurrentStage;
        }
        #endregion

        #region Stages

        public static class FirstLaunching
        {
            private static bool _isHold = false;
            private static bool _skillCheckIsArrived = false;
            private static bool _isLaunched = false;

            public static void SetIsHolded()
            {
                _isHold = true;
            }

            public static bool isHolded()
            {
                return _isHold;
            }

            public static void SetSkillCheckIsArrived()
            {
                _skillCheckIsArrived = true;
            }

            public static bool SkillCheckIsArriving()
            {
                return _skillCheckIsArrived;
            }

            public static void SetIsLaunched()
            {
                _isLaunched = true;
            }

            public static bool isLaunched()
            {
                return _isLaunched;
            }

            public static void ResetStage()
            {
                _isHold = false;
                _skillCheckIsArrived = false;
                _isLaunched = false;
            }
        }

        public static class ArrowControle
        {
            private static bool _arrowIsLaunched = false;
            private static bool _isControlled = false;
            private static bool _isPrematureEnd = false;

            public static void SetArrowIsLaunched()
            {
                _arrowIsLaunched = true;
            }

            public static void ResetArrowIsLaunched()
            {
                _arrowIsLaunched = false;
            }

            public static bool ArrowIsLaunched()
            {
                return _arrowIsLaunched;
            }

            public static void SetIsControlled()
            {
                _isControlled = true;
            }

            public static bool IsControlled()
            {
                return _isControlled;
            }

            public static void SetIsPrematureEnd()
            {
                _isPrematureEnd = true;
            }

            public static bool IsPrematureEnd()
            {
                return _isPrematureEnd;
            }

            public static void ResetStage()
            {
                _isControlled = false;
                _isPrematureEnd = false;
            }
        }

        #endregion
    }
}
