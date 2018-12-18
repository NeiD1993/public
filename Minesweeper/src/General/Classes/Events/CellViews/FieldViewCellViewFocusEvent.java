package General.Classes.Events.CellViews;

import java.awt.*;

/**
 * Created by NeiD on 12.09.2016.
 */
public class FieldViewCellViewFocusEvent extends BaseFieldViewCellViewChangeEvent {

    public FieldViewCellViewFocusEvent(Object source, Point previousFocusedCellViewIndexes, Point nextFocusedCellViewIndexes) {
        super(source, previousFocusedCellViewIndexes, nextFocusedCellViewIndexes);
    }

}
