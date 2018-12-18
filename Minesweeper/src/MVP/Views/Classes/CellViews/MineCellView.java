package MVP.Views.Classes.CellViews;

import MVP.Views.Enums.CellViewStates.MineCellViewStates;
import MVP.Views.Interfaces.CellViews.ICellView;
import MVP.Views.Interfaces.CellViews.ICellViewStates;

import java.awt.*;
import java.util.HashMap;
import java.util.Map;

/**
 * Created by NeiD on 08.09.2016.
 */
public class MineCellView<DisplayClass extends Graphics> extends BaseCellView<DisplayClass> implements ICellView<DisplayClass> {

    protected static Map<ICellViewStates, Image> mineCellViewStatesMap;

    static {
        mineCellViewStatesMap = new HashMap<>();
        getCellViewStatesMapByCellViewStates(mineCellViewStatesMap, MineCellViewStates.values());
    }

    public MineCellView(int horizontalCellViewCoordinate, int verticalCellViewCoordinate, int cellViewWidth, int cellViewHeight) {
        super(horizontalCellViewCoordinate, verticalCellViewCoordinate, cellViewWidth, cellViewHeight);
    }

    @Override
    protected Image getCellViewImageByExtendedCellViewStates(ICellViewStates cellViewState) {
        if (cellViewState.getClass() == MineCellViewStates.class)
            return mineCellViewStatesMap.get(cellViewState);
        else
            return null;
    }

}
