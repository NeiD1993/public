package General.Classes.FieldsGenerators;

import General.Interfaces.IFieldsGenerator;
import MVP.Models.Classes.CellModels.EmptyCellModel;
import MVP.Models.Classes.CellModels.MineCellModel;
import MVP.Models.Classes.CellModelsActionPerformer;
import MVP.Models.Classes.FieldModels.BaseFieldModel;
import MVP.Models.Interfaces.CellModels.ICellModel;
import MVP.Views.Classes.CellViews.EmptyCellView;
import MVP.Views.Classes.CellViews.MineCellView;
import MVP.Views.Interfaces.CellViews.ICellView;
import MVP.Views.Interfaces.IFieldView;

import java.awt.*;
import java.util.Map;
import java.util.function.Function;
import java.util.List;

/**
 * Created by NeiD on 09.09.2016.
 */
public abstract class BaseFieldsGenerator<EmptyCellModelClass extends EmptyCellModel, MineCellModelClass extends MineCellModel,
        DisplayClass extends Graphics, EmptyCellViewClass extends EmptyCellView<DisplayClass>, MineCellViewClass extends MineCellView<DisplayClass>>
        implements IFieldsGenerator<EmptyCellModelClass, MineCellModelClass, DisplayClass> {

    protected static String exceptionMessage;

    protected int mineCellsCountNear;

    protected Function<Integer, Integer> horizontalGenerateFunction;

    protected Function<Integer, Integer> verticalGenerateFunction;

    protected CellModelsActionPerformer<ICellModel> cellModelsIndexesActionPerformer;

    static {
        exceptionMessage = "Exception in BaseFieldGenerator";
    }

    BaseFieldsGenerator(Function<Integer, Integer> horizontalGenerateFunction, Function<Integer, Integer> verticalGenerateFunction) {
        setHorizontalGenerateFunction(horizontalGenerateFunction);
        setVerticalGenerateFunction(verticalGenerateFunction);
        cellModelsIndexesActionPerformer = new CellModelsActionPerformer<>(cellModel -> possibleMineCellCountNearIncreasing(cellModel));
    }

    protected abstract int generateHorizontalCellIndex(int horizontalCellsCount);

    protected abstract int generateVerticalCellIndex(int verticalCellsCount);

    protected abstract EmptyCellModelClass createInstanceOfEmptyCellModelClass(int mineCellModelCountNear);

    protected abstract MineCellModelClass createInstanceOfMineCellModelClass();

    protected abstract EmptyCellViewClass createInstanceOfEmptyCellViewClass(int horizontalCellViewCoordinate, int verticalCellViewCoordinate, int horizontalCellViewSize,
                                                                                      int verticalCellViewSize);

    protected abstract MineCellViewClass createInstanceOfMineCellViewClass(int horizontalCellViewCoordinate, int verticalCellViewCoordinate, int horizontalCellViewSize,
                                                                                    int verticalCellViewSize);

    private void generateCells(int horizontalCellsCount, int verticalCellsCount, int mineCellsCount, ICellModel[][] cellModels, List<Map<Integer, EmptyCellModelClass>> unopenedEmptyCellModels,
                               List<Map<Integer, MineCellModelClass>> unopenedMineCellModels, int horizontalCellViewSize, int verticalCellViewSize,
                               ICellView<DisplayClass>[][] cellViews) {
        generateMineCells(horizontalCellsCount, verticalCellsCount, mineCellsCount, cellModels, unopenedMineCellModels, horizontalCellViewSize, verticalCellViewSize,
                cellViews);
        for (int i = 0; i < horizontalCellsCount; i++) {
            for (int j = 0; j < verticalCellsCount; j++) {
                if (cellModels[i][j] == null) {
                    if (mineCellsCountNear != 0)
                        mineCellsCountNear = 0;
                    cellModelsIndexesActionPerformer.performNearCellModelsIndexesAction(i, j, horizontalCellsCount, verticalCellsCount,
                            cellModelIndexes -> cellModels[cellModelIndexes.x][cellModelIndexes.y]);
                    cellModels[i][j] = createInstanceOfEmptyCellModelClass(mineCellsCountNear);
                    unopenedEmptyCellModels.get(i).put(j, (EmptyCellModelClass) cellModels[i][j]);
                    cellViews[i][j] = createInstanceOfEmptyCellViewClass(i * horizontalCellViewSize, j * verticalCellViewSize, horizontalCellViewSize, verticalCellViewSize);
                }
            }
        }
    }

    private void generateMineCells(int horizontalCellsCount, int verticalCellsCount, int mineCellsCount, ICellModel[][] cellModels, List<Map<Integer, MineCellModelClass>> unopenedMineCellModels,
                                   int horizontalCellViewSize, int verticalCellViewSize, ICellView<DisplayClass>[][] cellViews) {
        int mineCellsCounter = 0;
        int horizontalCellIndex, verticalCellIndex;
        while (mineCellsCounter < mineCellsCount) {
            horizontalCellIndex = generateHorizontalCellIndex(horizontalCellsCount);
            verticalCellIndex = generateVerticalCellIndex(verticalCellsCount);
            if (cellModels[horizontalCellIndex][verticalCellIndex] == null) {
                cellModels[horizontalCellIndex][verticalCellIndex] = createInstanceOfMineCellModelClass();
                mineCellsCounter++;
                unopenedMineCellModels.get(horizontalCellIndex).put(verticalCellIndex, (MineCellModelClass) cellModels[horizontalCellIndex][verticalCellIndex]);
                cellViews[horizontalCellIndex][verticalCellIndex] = createInstanceOfMineCellViewClass(horizontalCellIndex * horizontalCellViewSize, verticalCellIndex * verticalCellViewSize,
                        horizontalCellViewSize, verticalCellViewSize);
            }
        }
    }

    private void possibleMineCellCountNearIncreasing(ICellModel cellModel) {
        if (cellModel != null && cellModelsIndexesActionPerformer.isCellModelClassAsDescendantOfAncestorCellModelClass(cellModel.getClass(), MineCellModel.class))
            mineCellsCountNear++;
    }

    @Override
    public void setHorizontalGenerateFunction(Function<Integer, Integer> horizontalGenerateFunction) {
        if (horizontalGenerateFunction != null)
            this.horizontalGenerateFunction = horizontalGenerateFunction;
    }

    @Override
    public void setVerticalGenerateFunction(Function<Integer, Integer> verticalGenerateFunction) {
        if (verticalGenerateFunction != null)
            this.verticalGenerateFunction = verticalGenerateFunction;
    }

    @Override
    public void generateFields(BaseFieldModel<EmptyCellModelClass, MineCellModelClass> fieldModel, IFieldView<DisplayClass> fieldView) {
        try {
            generateCells(fieldModel.getHorizontalCellsCount(), fieldModel.getVerticalCellsCount(), fieldModel.getMineCellModelsCount(), fieldModel.getCells(),
                    fieldModel.getUnopenedEmptyCellModels(), fieldModel.getUnopenedMineCellModels(), fieldView.getHorizontalCellViewSize(), fieldView.getVerticalCellViewSize(),
                    fieldView.getCellViews());
        }
        catch (Exception exception) {
            System.out.println(exceptionMessage);
        }
    }

}