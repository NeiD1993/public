using Assets.Entities.FieldObjects.FieldObject.FieldObjectsTypes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Assets.Entities.FieldObjects.FieldObject.FieldObjectBehaviour.MovingBehaviour
{
    abstract class FieldObjectMovingBehaviour : BaseFieldObjectBehaviour
    {
        private static readonly int maxMovingSpeed = 2;

        private static readonly int minMovingSpeed = 1;

        private MoveDirection moveDirection;

        private int movingSpeed;

        public void Init(MoveDirection moveDirection, int movingSpeed)
        {
            MoveDirection = moveDirection;
            MovingSpeed = movingSpeed;
        }

        public MoveDirection MoveDirection
        {
            get
            {
                return moveDirection;
            }

            set
            {
                if ((value != MoveDirection.None) && (moveDirection != value))
                    moveDirection = value;
            }
        }

        public int MovingSpeed
        {
            get
            {
                return movingSpeed;
            }

            set
            {
                movingSpeed = ((value >= minMovingSpeed) && (value <= maxMovingSpeed)) ? value : maxMovingSpeed;
            }
        }

        protected abstract void TryMove(Field field, ref Vector2 fieldIndexes, object parameter = null);

        protected void TryMoveOnPosition(Field field, ref Vector2 fieldIndexes, MoveDirection possibleMoveDirection, FieldObjectType fieldObjectType, Animator animator = null)
        {
            if ((field != null) && (field.FieldDynamicObjectsMover != null))
            {
                if (field.FieldDynamicObjectsMover.MoveNonEmptyFieldObject(ref fieldIndexes, ref moveDirection, possibleMoveDirection, MovingSpeed, fieldObjectType))
                {
                    if (animator != null)
                    {
                        animator.Play("Run");

                        GameObject fieldGameObject = field.FieldObjects[(int)fieldIndexes.x][(int)fieldIndexes.y].GameObject;

                        if (fieldObjectType == FieldObjectType.Enemy)
                            PlayFieldObjectSound(field, fieldGameObject, "Enemy Step");
                        else
                            PlayFieldObjectSound(field, fieldGameObject, "Player Step");
                    }
                }
            }
        }

        protected void ChangeGameStatus(GameStatus gameStatus)
        {
            switch (gameStatus)
            {
                case GameStatus.Restart:
                    SceneManager.LoadScene("Main Scene");
                    break;
                default:
                    Application.Quit();
                    break;
            }
        }

        public override void TryExecute(Field field, ref Vector2 fieldIndexes, object parameter = null)
        {
            if (parameter as object[] == null)
            {
                if (parameter is GameStatus)
                    ChangeGameStatus((GameStatus)parameter);
                else
                    TryMove(field, ref fieldIndexes, parameter);
            }
        }

        public bool CanIncreaseMovingSpeed()
        {
            return !(MovingSpeed == maxMovingSpeed);
        }
    }
}
