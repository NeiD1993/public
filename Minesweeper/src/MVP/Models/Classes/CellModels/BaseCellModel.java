package MVP.Models.Classes.CellModels;

import MVP.Models.Enums.CellModelStates;
import MVP.Models.Interfaces.CellModels.ICellModel;

/**
 * Created by NeiD on 01.09.2016.
 */
public abstract class BaseCellModel implements ICellModel {

    protected CellModelStates cellModelState;

    BaseCellModel() {
        cellModelState = CellModelStates.Initial;
    }

    @Override
    public boolean openCellModel() {
        if (cellModelState == CellModelStates.Initial) {
            cellModelState = CellModelStates.Opened;
            return true;
        }
        else
            return false;
    }

    @Override
    public boolean suggestCellModel() {
        if (cellModelState != CellModelStates.Opened) {
            cellModelState = (cellModelState == CellModelStates.Suggested) ? CellModelStates.Initial : CellModelStates.Suggested;
            return true;
        }
        else return false;
    }

    @Override
    public CellModelStates getCellModelState() {
        return cellModelState;
    }

}
