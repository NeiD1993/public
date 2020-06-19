using System.Collections;
using GameScene.Services.Routines;

namespace GameScene.Services.Field
{
    public class FieldOpeningRoutinesExecutor : BaseMultipleRoutinesExecutor
    {
        public void AddRoutine(IEnumerator fieldOpeningRoutine)
        {
            routines.Enqueue(fieldOpeningRoutine);
        }

        public void ClearRoutines()
        {
            routines.Clear();
        }

        public override IEnumerator ExecuteRoutinesIteratively()
        {
            IEnumerator fieldOpeningRoutine;

            while (routines.Count > 0)
            {
                fieldOpeningRoutine = routines.Peek();

                yield return fieldOpeningRoutine;

                routines.Dequeue();
            }
        }
    }
}