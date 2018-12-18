package MVP.Models.Classes;

import MVP.Models.Interfaces.ICellModelsActionPerformer;

import java.awt.*;
import java.util.function.Consumer;
import java.util.function.Function;

/**
 * Created by NeiD on 07.09.2016.
 */
public class CellModelsActionPerformer<ActionInputType> implements ICellModelsActionPerformer<ActionInputType> {

    protected static String exceptionMessage;

    protected Consumer<ActionInputType> cellModelsIndexesAction;

    static {
        exceptionMessage = "Exception in CellModelsActionPerformer";
    }

    public CellModelsActionPerformer(Consumer<ActionInputType> cellModelsIndexesAction) {
        this.cellModelsIndexesAction = cellModelsIndexesAction;
    }

    @Override
    public void setCellModelsIndexesAction(Consumer<ActionInputType> cellModelsIndexesAction) {
        if (cellModelsIndexesAction != null)
            this.cellModelsIndexesAction = cellModelsIndexesAction;
    }

    @Override
    public void performNearCellModelsIndexesAction(int horizontalCellModelIndex, int verticalCellModelIndex, int horizontalCellModelsCount, int verticalCellModelsCount,
                                                   Function<Point, ActionInputType> cellModelsIndexesFunction) {
        if(cellModelsIndexesFunction != null && cellModelsIndexesAction != null) {
            try {
                for (int i = horizontalCellModelIndex - 1; i < horizontalCellModelIndex + 2; i++) {
                    for (int j = verticalCellModelIndex - 1; j < verticalCellModelIndex + 2; j++){
                        if (!(i == horizontalCellModelIndex && j == verticalCellModelIndex))
                            if (i >= 0 && i < horizontalCellModelsCount && j >= 0 && j < verticalCellModelsCount)
                                cellModelsIndexesAction.accept(cellModelsIndexesFunction.apply(new Point(i, j)));
                    }
                }
            }
            catch (Exception exception) {
                System.out.println(exceptionMessage + ".performNearCellModelsIndexesAction");
            }
        }
    }

    @Override
    public boolean isCellModelClassAsDescendantOfAncestorCellModelClass(Class descendantCellModelClass, Class ancestorCellModelClass) {
        if (descendantCellModelClass != null && ancestorCellModelClass != null) {
            while (descendantCellModelClass != Object.class) {
                if (descendantCellModelClass == ancestorCellModelClass)
                    return true;
                else
                    descendantCellModelClass = descendantCellModelClass.getSuperclass();
            }
            return false;
        }
        else
            return false;
    }

}
