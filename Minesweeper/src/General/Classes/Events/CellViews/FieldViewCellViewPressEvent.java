package General.Classes.Events.CellViews;

import java.awt.*;
import java.awt.event.MouseEvent;

/**
 * Created by NeiD on 13.09.2016.
 */
public class FieldViewCellViewPressEvent extends BaseFieldViewCellViewChangeEvent {

    protected MouseEvent mouseEvent;

    public FieldViewCellViewPressEvent(Object source, Point previousPressedCellViewIndexes, Point nextPressedCellViewIndexes, MouseEvent mouseEvent) {
        super(source, previousPressedCellViewIndexes, nextPressedCellViewIndexes);
        if (mouseEvent != null)
            this.mouseEvent = mouseEvent;
    }

    public MouseEvent getMouseEvent() {
        return mouseEvent;
    }

}
