package MVP.Presenters.Interfaces.Listeners;

import General.Classes.Events.Fields.FieldModels.FieldModelOpenEmptyCellModelEvent;
import General.Classes.Events.Fields.FieldModels.FieldModelOpenMineCellModelEvent;
import General.Classes.Events.Fields.FieldModels.FieldModelSuggestCellModelEvent;

/**
 * Created by NeiD on 12.09.2016.
 */
public interface IFieldModelChangeCellModelStateEventListener {

    void fieldModelOpenEmptyCellModelEventAction(FieldModelOpenEmptyCellModelEvent fieldModelOpenEmptyCellModelEvent);

    void fieldModelOpenMineCellModelEventAction(FieldModelOpenMineCellModelEvent fieldModelOpenMineCellModelEvent);

    void fieldModelSuggestCellModelEventAction(FieldModelSuggestCellModelEvent fieldModelSuggestCellModelEvent);

}
