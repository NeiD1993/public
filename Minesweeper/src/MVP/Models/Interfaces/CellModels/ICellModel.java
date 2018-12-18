package MVP.Models.Interfaces.CellModels;

import General.Interfaces.ICell;
import MVP.Models.Enums.CellModelStates;

/**
 * Created by NeiD on 01.09.2016.
 */
public interface ICellModel extends ICell {

    boolean suggestCellModel();

    boolean openCellModel();

    CellModelStates getCellModelState();

}
