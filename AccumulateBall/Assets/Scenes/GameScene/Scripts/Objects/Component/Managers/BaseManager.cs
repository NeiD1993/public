using System;
using System.Collections;
using GameScene.Objects;
using UnityEngine;

namespace GameScene.Managers
{
    public abstract class BaseManager : BaseComponentObject
    {
        public bool IsConfigurated { get; private set; }

        private IEnumerator ProcessStartIteratively()
        {
            Func<bool> canConfigureConditionFunction = GetCanConfigureConditionFunction();

            yield return new WaitUntil(canConfigureConditionFunction);

            PerpetrateStartProcessing();
        }

        protected virtual void PerpetrateStartProcessing()
        {
            IsConfigurated = true;
        }

        protected override void PreProcessStart()
        {
            IsConfigurated = false;
            base.PreProcessStart();
        }

        protected override void ProcessStart()
        {
            StartCoroutine(ProcessStartIteratively());
        }

        protected virtual Func<bool> GetCanConfigureConditionFunction()
        {
            return () => true;
        }
    }
}