using System;
using System.Collections;
using GameScene.Services.Content.Characteristics.Interfaces;
using GameScene.Services.Content.Characteristics.Interfaces.Data;
using GameScene.Services.Content.Data;
using GameScene.Services.Content.Interfaces;
using GameScene.Services.Content.Parameters;
using UnityEngine;
using UnityEngine.Events;

namespace GameScene.Services.Content
{
    public abstract class BaseContentWithChangeablyParameterizedNonDefaultExtractionService<T1, T2, T3> : BaseContentWithParameterizedDefaultExtractionService<T1, T2, T3>,
        IContentWithConstantlyParameterizedExtractionService<T1, T2, T3> where T1 : struct
    {
        public ContentExtractionData<T2, T3> EstablishContentExtractionData(T1 constantExtractionParameter)
        {
            return new ContentExtractionData<T2, T3>(ExtractDefaultContent(constantExtractionParameter), changeableExtractionParameter =>
            ExtractNonDefaultContent(changeableExtractionParameter));
        }
    }

    public abstract class BaseContentWithFullyParameterizedNonDefaultExtractionService<T1, T2, T3, T4> : BaseContentWithParameterizedDefaultExtractionService<T1,
        ContentNonDefaultExtractionParameters<T2, T3>, T4>, IContentWithConstantlyParameterizedExtractionService<T1, T2, T3, T4> where T2 : struct
    {
        public ContentExtractionData<T3, T4> EstablishContentExtractionData(T1 constantDefaultExtractionParameter, T2 constantNonDefaultExtractionParameter)
        {
            return new ContentExtractionData<T3, T4>(ExtractDefaultContent(constantDefaultExtractionParameter), changeableExtractionParameter =>
            ExtractNonDefaultContent(new ContentNonDefaultExtractionParameters<T2, T3>(constantNonDefaultExtractionParameter, changeableExtractionParameter)));
        }
    }

    public abstract class BaseContentWithNonDefaultExtractionService<T1, T2>
    {
        protected abstract T2 ExtractNonDefaultContent(T1 nonDefaultExtractionParameter);
    }

    public abstract class BaseContentWithNonParameterizedDefaultExtractionService<T1, T2, T3> :
        BaseContentWithNonParameterizedDefaultExtractionService<ContentNonDefaultExtractionParameters<T1, T2>, T3>,
        IContentWithConstantlyParameterizedExtractionService<T1, T2, T3> where T1 : struct
    {
        public ContentExtractionData<T2, T3> EstablishContentExtractionData(T1 constantExtractionParameter)
        {
            return new ContentExtractionData<T2, T3>(ExtractDefaultContent(), changeableExtractionParameter =>
            ExtractNonDefaultContent(new ContentNonDefaultExtractionParameters<T1, T2>(constantExtractionParameter, changeableExtractionParameter)));
        }
    }

    public abstract class BaseContentWithNonParameterizedDefaultExtractionService<T1, T2> : BaseContentWithNonDefaultExtractionService<T1, T2>
    {
        protected abstract T2 ExtractDefaultContent();
    }

    public abstract class BaseContentWithParameterizedDefaultExtractionService<T1, T2, T3> : BaseContentWithNonDefaultExtractionService<T2, T3>
    {
        protected abstract T3 ExtractDefaultContent(T1 constantDefaultExtractionParameter);
    }
}

namespace GameScene.Services.Content.Characteristics
{
    public abstract class BaseExtractableContentCharacteristics<T1, T2>
    {
        private readonly Func<T1, T2> contentRefresher;

        public BaseExtractableContentCharacteristics(ContentExtractionData<T1, T2> contentExtractionData)
        {
            contentRefresher = contentExtractionData.NonDefaultContentExtractor;
            Content = contentExtractionData.DefaultContent;
            Refreshed = new UnityEvent();
        }

        public T2 Content { get; private set; }

        public UnityEvent Refreshed { get; private set; }

        protected virtual void PerformPostRefreshing(bool refreshingResult)
        {
            if (refreshingResult)
                Refreshed.Invoke();
        }

        protected bool TryRefresh(T1 changeableExtractionParameter, bool withPostRefreshing = true)
        {
            bool result;
            T2 contentBeforeRefreshing = Content;

            Content = contentRefresher(changeableExtractionParameter);
            result = !Content.Equals(contentBeforeRefreshing);

            if (withPostRefreshing)
                PerformPostRefreshing(result);

            return result;
        }
    }

    public abstract class BaseExtractableContentWithNonParameterizedRefreshingCharacteristics<T1, T2> : BaseExtractableContentCharacteristics<T1, T2>,
        IExtractableContentWithNonParameterizedRefreshingCharacteristics
    {
        public BaseExtractableContentWithNonParameterizedRefreshingCharacteristics(ContentExtractionData<T1, T2> contentExtractionData) : base(contentExtractionData) { }

        protected abstract T1 GetChangeableExtractionParameter();

        public bool TryRefresh()
        {
            return TryRefresh(GetChangeableExtractionParameter());
        }
    }

    public abstract class BaseExtractableContentWithParameterizedIterativeRefreshingCharacteristics<T1, T2, T3> : 
        BaseExtractableContentWithParameterizedRefreshingCharacteristics<T1, T2, T3>, IExtractableContentWithParameterizedIterativeRefreshingCharacteristics<T2>
    {
        public BaseExtractableContentWithParameterizedIterativeRefreshingCharacteristics(ContentExtractionData<T1, T3> contentExtractionData) :
            base(contentExtractionData)
        { }

        public IEnumerator Refresh(T2 refreshingParameter, Action<ContentRefreshingData> refreshingDataExtractor = null)
        {
            if (refreshingDataExtractor == null)
                TryRefresh(GetChangeableExtractionParameter(refreshingParameter));
            else
            {
                ContentRefreshingData contentRefreshingData = new ContentRefreshingData(TryRefresh(GetChangeableExtractionParameter(refreshingParameter), false));

                refreshingDataExtractor(contentRefreshingData);

                yield return new WaitUntil(() => contentRefreshingData.PerformFurtherProceeding != null);

                if (contentRefreshingData.PerformFurtherProceeding.Value)
                    PerformPostRefreshing(contentRefreshingData.Result);
            }
        }
    }

    public abstract class BaseExtractableContentWithParameterizedNonIterativeRefreshingCharacteristics<T1, T2, T3> :
        BaseExtractableContentWithParameterizedRefreshingCharacteristics<T1, T2, T3>, IExtractableContentWithParameterizedNonIterativeRefreshingCharacteristics<T2>
    {
        public BaseExtractableContentWithParameterizedNonIterativeRefreshingCharacteristics(ContentExtractionData<T1, T3> contentExtractionData) :
            base(contentExtractionData)
        { }

        public virtual bool TryRefresh(T2 refreshingParameter)
        {
            return TryRefresh(GetChangeableExtractionParameter(refreshingParameter));
        }
    }

    public abstract class BaseExtractableContentWithParameterizedRefreshingCharacteristics<T1, T2, T3> : BaseExtractableContentCharacteristics<T1, T3>
    {
        public BaseExtractableContentWithParameterizedRefreshingCharacteristics(ContentExtractionData<T1, T3> contentExtractionData) :
            base(contentExtractionData)
        { }

        protected abstract T1 GetChangeableExtractionParameter(T2 refreshingParameter);
    }
}

namespace GameScene.Services.Content.Characteristics.Interfaces
{
    public interface IExtractableContentWithNonParameterizedRefreshingCharacteristics
    {
        bool TryRefresh();
    }

    public interface IExtractableContentWithParameterizedIterativeRefreshingCharacteristics<T>
    {
        IEnumerator Refresh(T refreshingParameter, Action<ContentRefreshingData> refreshingDataExtractor = null);
    }

    public interface IExtractableContentWithParameterizedNonIterativeRefreshingCharacteristics<T>
    {
        bool TryRefresh(T refreshingParameter);
    }
}

namespace GameScene.Services.Content.Characteristics.Interfaces.Data
{
    public class ContentRefreshingData
    {
        public ContentRefreshingData(bool result)
        {
            Result = result;
            PerformFurtherProceeding = null;
        }

        public bool Result { get; private set; }

        public bool? PerformFurtherProceeding { get; set; }
    }
}

namespace GameScene.Services.Content.Data
{
    public struct ContentExtractionData<T1, T2>
    {
        public ContentExtractionData(T2 defaultContent, Func<T1, T2> nonDefaultContentExtractor)
        {
            DefaultContent = defaultContent;
            NonDefaultContentExtractor = nonDefaultContentExtractor;
        }

        public T2 DefaultContent { get; private set; }

        public Func<T1, T2> NonDefaultContentExtractor { get; private set; }
    }
}

namespace GameScene.Services.Content.Interfaces
{
    public interface IContentWithConstantlyParameterizedExtractionService<T1, T2, T3> where T1 : struct
    {
        ContentExtractionData<T2, T3> EstablishContentExtractionData(T1 constantExtractionParameter);
    }

    public interface IContentWithConstantlyParameterizedExtractionService<T1, T2, T3, T4> where T2 : struct
    {
        ContentExtractionData<T3, T4> EstablishContentExtractionData(T1 constantDefaultExtractionParameter, T2 constantNonDefaultExtractionParameter);
    }
}

namespace GameScene.Services.Content.Parameters
{
    public struct ContentNonDefaultExtractionParameters<T1, T2> where T1 : struct
    {
        public ContentNonDefaultExtractionParameters(T1 constant, T2 changeable)
        {
            Constant = constant;
            Changeable = changeable;
        }

        public T1 Constant { get; private set; }

        public T2 Changeable { get; private set; }
    }
}