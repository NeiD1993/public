package General.Classes.Events.Fields.FieldModels;

import General.Classes.Events.Fields.BaseFieldChangeEvent;
import MVP.Models.Enums.CellModelStates;

import java.awt.*;

/**
 * Created by NeiD on 12.09.2016.
 */
public class FieldModelSuggestCellModelEvent extends BaseFieldChangeEvent {

    protected CellModelStates cellModelState;

    public FieldModelSuggestCellModelEvent(Object source, Point changedCellStateCellModelIndexes, CellModelStates cellModelState) {
        super(source, changedCellStateCellModelIndexes);
        this.cellModelState = cellModelState;
    }

    public CellModelStates getCellModelState() {
        return cellModelState;
    }

}
