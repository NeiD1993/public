package General.Classes.Events.Fields.FieldModels;

import General.Classes.Events.Fields.BaseFieldChangeEvent;

import java.awt.*;

/**
 * Created by NeiD on 12.09.2016.
 */
public class FieldModelOpenMineCellModelEvent extends BaseFieldChangeEvent {

    public FieldModelOpenMineCellModelEvent(Object source, Point changedCellStateCellModelIndexes) {
        super(source, changedCellStateCellModelIndexes);
    }

}
