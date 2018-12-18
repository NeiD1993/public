package MVP.Models.Classes.FieldModels;

import General.Classes.Events.EndEvent;
import General.Classes.Events.Fields.FieldModels.FieldModelOpenEmptyCellModelEvent;
import General.Classes.Events.Fields.FieldModels.FieldModelOpenMineCellModelEvent;
import General.Classes.Events.Fields.FieldModels.FieldModelSuggestCellModelEvent;
import General.Enums.EndStates;
import MVP.Presenters.Interfaces.Listeners.IEndEventListener;
import MVP.Models.Classes.CellModels.EmptyCellModel;
import MVP.Models.Classes.CellModels.MineCellModel;
import MVP.Models.Classes.CellModelsActionPerformer;
import MVP.Models.Enums.CellModelStates;
import MVP.Models.Interfaces.IFieldModelLogic;

import java.awt.*;
import java.util.ArrayList;
import java.util.List;

/**
 * Created by NeiD on 09.09.2016.
 */
public class FieldModel<EmptyCellModelClass extends EmptyCellModel, MineCellModelClass extends MineCellModel> extends BaseFieldModel<EmptyCellModelClass, MineCellModelClass>
        implements IFieldModelLogic {

    protected static String exceptionMessage;

    protected boolean isMineCellModelOpened;

    protected int openedCellModelsCount;

    protected CellModelsActionPerformer<Point> cellModelsIndexesActionPerformer;

    protected List<IEndEventListener> endEventListeners;

    static {
        exceptionMessage = "Exception in FieldModel";
    }

    public FieldModel(int horizontalCellModelsCount, int verticalCellModelsCount, int mineCellModelsCount) {
        super(horizontalCellModelsCount, verticalCellModelsCount, mineCellModelsCount);
        cellModelsIndexesActionPerformer = new CellModelsActionPerformer<>(emptyCellModelIndexes -> openEmptyCellModel(emptyCellModelIndexes.x, emptyCellModelIndexes.y));
        endEventListeners = new ArrayList<>();
    }

    protected void fireFieldModelOpenEmptyCellModelEvent(FieldModelOpenEmptyCellModelEvent fieldModelOpenEmptyCellModelEvent) {
        try {
            fieldModelChangeCellModelStateEventListeners.forEach(fieldModelChangeCellModelStateEventListener ->
                    fieldModelChangeCellModelStateEventListener.fieldModelOpenEmptyCellModelEventAction(fieldModelOpenEmptyCellModelEvent));
        }
        catch (Exception exception) {
            System.out.println(exceptionMessage + ".fireFieldModelOpenEmptyCellModelEvent");
        }
    }

    protected void fireFieldModelOpenMineCellModelEvent(FieldModelOpenMineCellModelEvent fieldModelOpenMineCellModelEvent) {
        try {
            fieldModelChangeCellModelStateEventListeners.forEach(fieldModelChangeCellModelStateEventListener ->
                    fieldModelChangeCellModelStateEventListener.fieldModelOpenMineCellModelEventAction(fieldModelOpenMineCellModelEvent));
        }
        catch (Exception exception) {
            System.out.println(exceptionMessage + ".fireFieldModelOpenMineCellModelEvent");
        }
    }

    protected void fireFieldModelSuggestCellModelEvent(FieldModelSuggestCellModelEvent fieldModelSuggestCellModelEvent) {
        try {
            fieldModelChangeCellModelStateEventListeners.forEach(fieldModelChangeCellModelStateEventListener ->
                    fieldModelChangeCellModelStateEventListener.fieldModelSuggestCellModelEventAction(fieldModelSuggestCellModelEvent));
        }
        catch (Exception exception) {
            System.out.println(exceptionMessage + ".fireFieldModelSuggestCellModelEvent");
        }
    }

    protected void fireEndEvent(EndEvent endEvent) {
        try {
            endEventListeners.forEach(endEventListener -> endEventListener.endEventAction(endEvent));
        }
        catch (Exception exception) {
            System.out.println(exceptionMessage + ".fireEndEvent");
        }
    }

    protected void openEmptyCellModel(int horizontalEmptyCellModelIndex, int verticalEmptyCellModelIndex) {
        if (cells[horizontalEmptyCellModelIndex][verticalEmptyCellModelIndex].openCellModel()) {
            openedCellModelsCount++;
            unopenedEmptyCellModels.get(horizontalEmptyCellModelIndex).remove(verticalEmptyCellModelIndex);
            openedOrSuggestedCellModels.get(horizontalEmptyCellModelIndex).put(verticalEmptyCellModelIndex, cells[horizontalEmptyCellModelIndex][verticalEmptyCellModelIndex]);
            int mineCellModelsNear = ((EmptyCellModel)cells[horizontalEmptyCellModelIndex][verticalEmptyCellModelIndex]).getMineCellModelsNear();
            FieldModelOpenEmptyCellModelEvent fieldModelOpenEmptyCellModelEvent = new FieldModelOpenEmptyCellModelEvent(this, new Point(horizontalEmptyCellModelIndex, verticalEmptyCellModelIndex),
                    mineCellModelsNear);
            fireFieldModelOpenEmptyCellModelEvent(fieldModelOpenEmptyCellModelEvent);
            if (mineCellModelsNear == 0)
                cellModelsIndexesActionPerformer.performNearCellModelsIndexesAction(horizontalEmptyCellModelIndex, verticalEmptyCellModelIndex, horizontalCellsCount, verticalCellsCount,
                        cellModelIndexes -> cellModelIndexes);
        }
    }

    @Override
    protected void resetCellModelsParameters() {
        isMineCellModelOpened = false;
        openedCellModelsCount = 0;
    }

    @Override
    public void openCellModel(int horizontalCellModelIndex, int verticalCellModelIndex) {
        try{
            if (cellModelsIndexesActionPerformer.isCellModelClassAsDescendantOfAncestorCellModelClass(cells[horizontalCellModelIndex][verticalCellModelIndex].getClass(), EmptyCellModel.class)) {
                openEmptyCellModel(horizontalCellModelIndex, verticalCellModelIndex);
                if (checkingWinConditions())
                    fireEndEvent(new EndEvent(this, EndStates.Win));
            }
            else {
                if (cells[horizontalCellModelIndex][verticalCellModelIndex].openCellModel()) {
                    if (!isMineCellModelOpened)
                        isMineCellModelOpened = true;
                    unopenedMineCellModels.get(horizontalCellModelIndex).remove(verticalCellModelIndex);
                    openedOrSuggestedCellModels.get(horizontalCellModelIndex).put(verticalCellModelIndex, cells[horizontalCellModelIndex][verticalCellModelIndex]);
                    FieldModelOpenMineCellModelEvent fieldModelOpenMineCellModelEvent = new FieldModelOpenMineCellModelEvent(this, new Point(horizontalCellModelIndex, verticalCellModelIndex));
                    fireFieldModelOpenMineCellModelEvent(fieldModelOpenMineCellModelEvent);
                }
                if (checkingLoseConditions())
                    fireEndEvent(new EndEvent(this, EndStates.Lose));
            }
        }
        catch (Exception exception) {
            System.out.println(exceptionMessage + ".openCellModel");
        }
    }

    @Override
    public void suggestCellModel(int horizontalCellModelIndex, int verticalCellModelIndex) {
        try {
            if (cells[horizontalCellModelIndex][verticalCellModelIndex].suggestCellModel()) {
                if (cells[horizontalCellModelIndex][verticalCellModelIndex].getCellModelState() == CellModelStates.Suggested)
                    openedOrSuggestedCellModels.get(horizontalCellModelIndex).put(verticalCellModelIndex, cells[horizontalCellModelIndex][verticalCellModelIndex]);
                else
                    openedOrSuggestedCellModels.get(horizontalCellModelIndex).remove(verticalCellModelIndex);
                FieldModelSuggestCellModelEvent fieldModelSuggestCellModelEvent = new FieldModelSuggestCellModelEvent(this, new Point(horizontalCellModelIndex, verticalCellModelIndex),
                        cells[horizontalCellModelIndex][verticalCellModelIndex].getCellModelState());
                fireFieldModelSuggestCellModelEvent(fieldModelSuggestCellModelEvent);
            }
        }
        catch (Exception exception) {
            System.out.println(exceptionMessage + ".suggestCellModel");
        }
    }

    @Override
    public void addEndEventListener(IEndEventListener endEventListener) {
        if (endEventListener != null)
            endEventListeners.add(endEventListener);
    }

    @Override
    public void removeEndEventListener(IEndEventListener endEventListener) {
        endEventListeners.remove(endEventListener);
    }

    @Override
    public boolean checkingWinConditions() {
        return (openedCellModelsCount == horizontalCellsCount * verticalCellsCount - mineCellModelsCount) ? true : false;
    }

    @Override
    public boolean checkingLoseConditions() {
        return isMineCellModelOpened ? true : false;
    }

}
