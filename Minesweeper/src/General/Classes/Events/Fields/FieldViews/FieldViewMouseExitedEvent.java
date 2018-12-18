package General.Classes.Events.Fields.FieldViews;

import General.Classes.Events.Fields.BaseFieldChangeEvent;

import java.awt.*;

/**
 * Created by NeiD on 13.09.2016.
 */
public class FieldViewMouseExitedEvent extends BaseFieldChangeEvent {

    protected boolean isMouseButtonReleased;

    public FieldViewMouseExitedEvent(Object source, Point previousChangedCellViewIndexes, boolean isMouseButtonReleased) {
        super(source, previousChangedCellViewIndexes);
        this.isMouseButtonReleased = isMouseButtonReleased;
    }

    public boolean IsMouseButtonReleased() {
        return isMouseButtonReleased;
    }

}
