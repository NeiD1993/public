package MVP.Models.Interfaces;

import MVP.Presenters.Interfaces.Listeners.IFieldModelChangeCellModelStateEventListener;
import MVP.Models.Classes.CellModels.EmptyCellModel;
import MVP.Models.Classes.CellModels.MineCellModel;
import MVP.Models.Interfaces.CellModels.ICellModel;

import java.util.List;
import java.util.Map;

/**
 * Created by NeiD on 05.09.2016.
 */
public interface IFieldModel<EmptyCellModelClass extends EmptyCellModel, MineCellModelClass extends MineCellModel> {

    void nullifyCellModels();

    void resetCellModels();

    void setMineCellModelsCount(int mineCellModelsCount);

    void addFieldModelChangeCellModelStateEventListeners(IFieldModelChangeCellModelStateEventListener fieldModelChangeCellModelStateEventListeners);

    void removeFieldModelChangeCellModelStateEventListeners(IFieldModelChangeCellModelStateEventListener fieldModelChangeCellModelStateEventListeners);

    void nullifyUnopenedCellModels();

    void resetUnopenedCellModels();

    int getMineCellModelsCount();

    Map<Integer, EmptyCellModelClass> getUnopenedEmptyCellModelsMapByHorizontalIndex(int horizontalCellModelsIndex);

    Map<Integer, MineCellModelClass> getUnopenedMineCellModelsByHorizontalIndex(int horizontalCellModelsIndex);

    Map<Integer, ICellModel> getOpenedOrSuggestedCellModelsMapByHorizontalIndex(int horizontalCellModelsIndex);

    List<Map<Integer, EmptyCellModelClass>> getUnopenedEmptyCellModels();

    List<Map<Integer, MineCellModelClass>> getUnopenedMineCellModels();

    List<Map<Integer, ICellModel>> getOpenedOrSuggestedCellModels();

}
