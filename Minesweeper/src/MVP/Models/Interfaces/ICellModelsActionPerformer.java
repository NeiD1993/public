package MVP.Models.Interfaces;

import java.awt.*;
import java.util.function.Consumer;
import java.util.function.Function;

/**
 * Created by NeiD on 07.09.2016.
 */
public interface ICellModelsActionPerformer<ActionInputType> {

    void setCellModelsIndexesAction(Consumer<ActionInputType> cellModelsIndexesAction);

    void performNearCellModelsIndexesAction(int horizontalEmptyCellModelIndex, int verticalEmptyCellModelIndex, int horizontalCellModelsCount, int verticalCellModelsCount,
                                            Function<Point, ActionInputType> cellModelsIndexesFunction);

    boolean isCellModelClassAsDescendantOfAncestorCellModelClass(Class descendantCellModelClass, Class ancestorCellModelClass);

}
