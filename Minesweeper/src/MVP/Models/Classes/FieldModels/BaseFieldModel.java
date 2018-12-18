package MVP.Models.Classes.FieldModels;

import General.Classes.BaseField;
import MVP.Presenters.Interfaces.Listeners.IFieldModelChangeCellModelStateEventListener;
import MVP.Models.Classes.CellModels.EmptyCellModel;
import MVP.Models.Classes.CellModels.MineCellModel;
import MVP.Models.Interfaces.CellModels.ICellModel;
import MVP.Models.Interfaces.IFieldModel;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

/**
 * Created by NeiD on 09.09.2016.
 */
public abstract class BaseFieldModel<EmptyCellModelClass extends EmptyCellModel, MineCellModelClass extends MineCellModel> extends BaseField<ICellModel>
        implements IFieldModel<EmptyCellModelClass, MineCellModelClass> {

    protected static String exceptionMessage;

    protected int mineCellModelsCount;

    protected List<IFieldModelChangeCellModelStateEventListener> fieldModelChangeCellModelStateEventListeners;

    protected List<Map<Integer, EmptyCellModelClass>> unopenedEmptyCellModels;

    protected List<Map<Integer, MineCellModelClass>> unopenedMineCellModels;

    protected List<Map<Integer, ICellModel>> openedOrSuggestedCellModels;

    static {
        exceptionMessage = "Exception in BaseFieldModel";
    }

    public BaseFieldModel(int horizontalCellModelsCount, int verticalCellModelsCount, int mineCellModelsCount) {
        super(horizontalCellModelsCount, verticalCellModelsCount);
        if (mineCellModelsCount > 0 && mineCellModelsCount < horizontalCellsCount * verticalCellsCount)
            this.mineCellModelsCount = mineCellModelsCount;
        fieldModelChangeCellModelStateEventListeners = new ArrayList<>();
        resetUnopenedCellModels();
    }

    protected abstract void resetCellModelsParameters();

    @Override
    protected ICellModel[][] createCellTypes(int horizontalCellModelsCount, int verticalCellModelsCount) {
        return new ICellModel[horizontalCellModelsCount][verticalCellModelsCount];
    }

    @Override
    public void nullifyCellModels() {
        if (isNullifyCells())
        {
            nullifyUnopenedCellModels();
            resetCellModelsParameters();
        }
    }

    @Override
    public void resetCellModels() {
        if (isResetCells())
        {
            resetUnopenedCellModels();
            resetCellModelsParameters();
        }
    }

    @Override
    public void setMineCellModelsCount(int mineCellModelsCount) {
        if (mineCellModelsCount > 0 && mineCellModelsCount < horizontalCellsCount * verticalCellsCount) {
            if (isResetCells())
                this.mineCellModelsCount = mineCellModelsCount;
        }
    }

    @Override
    public void addFieldModelChangeCellModelStateEventListeners(IFieldModelChangeCellModelStateEventListener fieldModelChangeCellModelStateEventListener) {
        if (fieldModelChangeCellModelStateEventListener != null)
            fieldModelChangeCellModelStateEventListeners.add(fieldModelChangeCellModelStateEventListener);
    }

    @Override
    public void removeFieldModelChangeCellModelStateEventListeners(IFieldModelChangeCellModelStateEventListener fieldModelChangeCellStateEventListener) {
        fieldModelChangeCellModelStateEventListeners.remove(fieldModelChangeCellStateEventListener);
    }

    @Override
    public void nullifyUnopenedCellModels() {
        unopenedEmptyCellModels = null;
        unopenedMineCellModels = null;
    }

    @Override
    public void resetUnopenedCellModels() {
        unopenedEmptyCellModels = new ArrayList<>();
        unopenedMineCellModels = new ArrayList<>();
        openedOrSuggestedCellModels = new ArrayList<>();
        for (int i = 0; i < horizontalCellsCount; i++) {
            unopenedEmptyCellModels.add(new HashMap<>());
            unopenedMineCellModels.add(new HashMap<>());
            openedOrSuggestedCellModels.add(new HashMap<>());
        }
    }

    @Override
    public int getMineCellModelsCount() {
        return mineCellModelsCount;
    }

    @Override
    public Map<Integer, EmptyCellModelClass> getUnopenedEmptyCellModelsMapByHorizontalIndex(int horizontalCellModelsIndex) {
        return unopenedEmptyCellModels.get(horizontalCellModelsIndex);
    }

    @Override
    public Map<Integer, MineCellModelClass> getUnopenedMineCellModelsByHorizontalIndex(int horizontalCellModelsIndex) {
        return unopenedMineCellModels.get(horizontalCellModelsIndex);
    }

    @Override
    public Map<Integer, ICellModel> getOpenedOrSuggestedCellModelsMapByHorizontalIndex(int horizontalCellModelsIndex) {
        return openedOrSuggestedCellModels.get(horizontalCellModelsIndex);
    }

    @Override
    public List<Map<Integer, EmptyCellModelClass>> getUnopenedEmptyCellModels() {
        return unopenedEmptyCellModels;
    }

    @Override
    public List<Map<Integer, MineCellModelClass>> getUnopenedMineCellModels() {
        return unopenedMineCellModels;
    }

    @Override
    public List<Map<Integer, ICellModel>> getOpenedOrSuggestedCellModels() {
        return openedOrSuggestedCellModels;
    }

}
