package General.Classes.Events.Fields;

import java.awt.*;
import java.util.EventObject;

/**
 * Created by NeiD on 12.09.2016.
 */
public abstract class BaseFieldChangeEvent extends EventObject {

    protected Point changedCellIndexes;

    public BaseFieldChangeEvent(Object source, Point changedCellIndexes) {
        super(source);
        if (changedCellIndexes != null)
            this.changedCellIndexes = changedCellIndexes;
    }

    public Point getChangedCellIndexes() {
        return changedCellIndexes;
    }

}
