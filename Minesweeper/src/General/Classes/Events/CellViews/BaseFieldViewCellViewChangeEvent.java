package General.Classes.Events.CellViews;

import java.awt.*;
import java.util.EventObject;

/**
 * Created by NeiD on 13.09.2016.
 */
public abstract class BaseFieldViewCellViewChangeEvent extends EventObject {

    protected Point previousChangedCellViewIndexes;

    protected Point nextChangedCellViewIndexes;

    BaseFieldViewCellViewChangeEvent(Object source, Point previousChangedCellViewIndexes, Point nextChangedCellViewIndexes) {
        super(source);
        if (previousChangedCellViewIndexes != null)
            this.previousChangedCellViewIndexes = previousChangedCellViewIndexes;
        if (nextChangedCellViewIndexes != null)
            this.nextChangedCellViewIndexes = nextChangedCellViewIndexes;
    }

    public Point getPreviousChangedCellViewIndexes() {
        return previousChangedCellViewIndexes;
    }

    public Point getNextChangedCellViewIndexes() {
        return nextChangedCellViewIndexes;
    }

}
