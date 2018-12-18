package MVP.Views.Interfaces;

import MVP.Presenters.Interfaces.Listeners.FieldViews.IFieldViewCellViewChangeEventListener;
import MVP.Presenters.Interfaces.Listeners.FieldViews.IFieldViewInputEventListener;
import MVP.Presenters.Interfaces.Listeners.FieldViews.IFieldViewMouseExitedEventListener;
import MVP.Views.Interfaces.Listeners.IFieldViewStopInputEventListener;
import MVP.Views.Interfaces.CellViews.ICellView;
import MVP.Views.Interfaces.CellViews.ICellViewStates;

import java.awt.*;
import java.awt.image.ImageObserver;

/**
 * Created by NeiD on 08.09.2016.
 */
public interface IFieldView<DisplayClass extends Graphics> {

    void addFieldViewInputEventListener(IFieldViewInputEventListener fieldViewInputEventListener);

    void addFieldViewCellViewChangeEventListener(IFieldViewCellViewChangeEventListener fieldViewCellViewChangeEventListener);

    void addFieldViewMouseExitedEventListener(IFieldViewMouseExitedEventListener fieldViewMouseExitedEventListener);

    void addFieldViewStopInputEventListenerForFieldViewStopInputTimerRunner(IFieldViewStopInputEventListener fieldViewStopInputEventListener);

    void removeFieldViewInputEventListener(IFieldViewInputEventListener fieldViewInputEventListener);

    void removeFieldViewCellViewChangeEventListener(IFieldViewCellViewChangeEventListener fieldViewCellViewChangeEventListener);

    void removeFieldViewMouseExitedEventListener(IFieldViewMouseExitedEventListener fieldViewMouseExitedEventListener);

    void removeFieldViewStopInputEventListenerForFieldViewStopInputTimerRunner(IFieldViewStopInputEventListener fieldViewStopInputEventListener);

    void setFieldViewBlocked(boolean isFieldViewBlocked);

    void resetCellViews(int horizontalCellViewCount, int verticalCellViewCount);

    void setHorizontalCellViewSize(int horizontalCellViewSize);

    void setVerticalCellViewSize(int verticalCellViewSize);

    void setImageObserver(ImageObserver imageObserver);

    void setPreviousChangedCellViewIndexes(Point previousChangedCellViewIndexes);

    void setFieldViewSize();

    void repaintCellView(Point cellViewIndexes, ICellViewStates cellViewState);

    void stopFieldViewStopInputTimerForFieldViewStopInputTimerRunner();

    void restartFieldViewStopInputTimerForFieldViewStopInputTimerRunner();

    int getHorizontalCellViewSize();

    int getVerticalCellViewSize();

    ICellView<DisplayClass> getCellViewByIndexes(int horizontalCellViewIndex, int verticalCellViewIndex);

    ICellView<DisplayClass>[][] getCellViews();

}
