package MVP.Views.Interfaces.CellViews;

import General.Interfaces.ICell;

import java.awt.*;
import java.awt.image.ImageObserver;

/**
 * Created by NeiD on 03.09.2016.
 */
public interface ICellView<DisplayClass extends Graphics> extends ICell {

    void displayCellView(DisplayClass display, ImageObserver imageObserver);

    void setCellViewState(ICellViewStates cellViewState);

    void setHorizontalCellViewCoordinate(int horizontalCoordinate);

    void setVerticalCellViewCoordinate(int verticalCoordinate);

    void setCellViewWidth(int width);

    void setCellViewHeight(int height);

    ICellViewStates getCellViewState();

}
