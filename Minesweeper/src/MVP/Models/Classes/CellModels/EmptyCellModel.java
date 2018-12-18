package MVP.Models.Classes.CellModels;

import MVP.Models.Interfaces.CellModels.IEmptyCellModel;
import MVP.Views.Enums.CellViewStates.EmptyCellViewStates;

/**
 * Created by NeiD on 01.09.2016.
 */
public class EmptyCellModel extends BaseCellModel implements IEmptyCellModel {

    protected int mineCellModelsNear;

    public EmptyCellModel(int mineCellModelsNear) {
        super();
        setMineCellModelsNear(mineCellModelsNear);
    }

    @Override
    public void setMineCellModelsNear(int mineCellModelsNear) {
        if (mineCellModelsNear > 0 && mineCellModelsNear <= EmptyCellViewStates.values().length - 1)
            this.mineCellModelsNear = mineCellModelsNear;
    }

    @Override
    public int getMineCellModelsNear() {
        return mineCellModelsNear;
    }

}
