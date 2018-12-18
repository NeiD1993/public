package MVP.Views.Classes.CellViews;

import MVP.Views.Enums.CellViewStates.EmptyCellViewStates;
import MVP.Views.Interfaces.CellViews.ICellView;
import MVP.Views.Interfaces.CellViews.ICellViewStates;

import java.awt.*;
import java.util.HashMap;
import java.util.Map;

/**
 * Created by NeiD on 08.09.2016.
 */
public class EmptyCellView<DisplayClass extends Graphics> extends BaseCellView<DisplayClass> implements ICellView<DisplayClass> {

    protected static Map<ICellViewStates, Image> emptyCellViewStatesMap;

    static {
        emptyCellViewStatesMap = new HashMap<>();
        getCellViewStatesMapByCellViewStates(emptyCellViewStatesMap, EmptyCellViewStates.values());
    }

    public EmptyCellView(int horizontalCellViewCoordinate, int verticalCellViewCoordinate, int cellViewWidth, int cellViewHeight) {
        super(horizontalCellViewCoordinate, verticalCellViewCoordinate, cellViewWidth, cellViewHeight);
    }

    @Override
    protected Image getCellViewImageByExtendedCellViewStates(ICellViewStates cellViewState) {
        if (cellViewState.getClass() == EmptyCellViewStates.class)
            return emptyCellViewStatesMap.get(cellViewState);
        else
            return null;
    }

}
