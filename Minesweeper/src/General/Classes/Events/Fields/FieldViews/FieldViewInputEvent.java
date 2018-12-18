package General.Classes.Events.Fields.FieldViews;

import General.Classes.Events.Fields.BaseFieldChangeEvent;
import General.Enums.InputActions;

import java.awt.*;

/**
 * Created by NeiD on 08.09.2016.
 */
public class FieldViewInputEvent extends BaseFieldChangeEvent {

    protected InputActions inputAction;

    public FieldViewInputEvent(Object source, Point inputedCellIndexes, InputActions inputAction) {
        super(source, inputedCellIndexes);
        if (inputAction != null)
            this.inputAction = inputAction;
    }

    public InputActions getInputAction() {
        return inputAction;
    }

}
