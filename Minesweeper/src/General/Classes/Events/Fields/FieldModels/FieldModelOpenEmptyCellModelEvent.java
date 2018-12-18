package General.Classes.Events.Fields.FieldModels;

import General.Classes.Events.Fields.BaseFieldChangeEvent;
import MVP.Views.Enums.CellViewStates.EmptyCellViewStates;

import java.awt.*;

/**
 * Created by NeiD on 12.09.2016.
 */
public class FieldModelOpenEmptyCellModelEvent extends BaseFieldChangeEvent {

    protected int mineCellModelsNear;

    public FieldModelOpenEmptyCellModelEvent(Object source, Point changedCellStateCellModelIndexes, int mineCellModelsNear) {
        super(source, changedCellStateCellModelIndexes);
        if (mineCellModelsNear > 0 && mineCellModelsNear <= EmptyCellViewStates.values().length - 1)
            this.mineCellModelsNear = mineCellModelsNear;
    }

    public int getMineCellModelsNear() {
        return mineCellModelsNear;
    }

}
