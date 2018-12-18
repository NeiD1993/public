using Assets.Entities;
using Assets.Entities.FieldObjects.FieldObject.FieldObjectsTypes;
using Assets.Scripts.Behaviour.BaseBehaviour;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Behaviour.MoveableFieldObjectBehaviour
{
    class PlayerBehaviour : BaseFieldBehaviour
    {
        [SerializeField]
        [HideInInspector]
        public bool IsActive { get; set; }

        protected override void Start()
        {
            IsActive = true;
        }

        protected override void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
                Execute(GameStatus.Restart);
            else if (Input.GetKeyDown(KeyCode.Escape))
                Execute(GameStatus.Exit);
            else if (IsActive)
            {
                if (Input.GetKeyDown(KeyCode.DownArrow))
                    Execute(MoveDirection.Down);
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                    Execute(MoveDirection.Left);
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                    Execute(MoveDirection.Right);
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                    Execute(MoveDirection.Up);
                else if (Input.GetKeyDown(KeyCode.Space))
                    Execute(null);
            }
            else
            {
                if (Field.EnemiesCount > 0)
                    ExecuteOnTimer(FieldObjectType.Player);
                else
                    Execute(new object[] { FieldObjectType.Player, "Player Win" });
            }
        }
    }
}
