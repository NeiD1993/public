package General.Classes.FieldsGenerators;

import MVP.Models.Classes.CellModels.EmptyCellModel;
import MVP.Models.Classes.CellModels.MineCellModel;
import MVP.Views.Classes.CellViews.EmptyCellView;
import MVP.Views.Classes.CellViews.MineCellView;

import java.awt.*;
import java.util.function.Function;

/**
 * Created by NeiD on 09.09.2016.
 */
public class PseudoRandomFieldsGenerator<DisplayClass extends Graphics> extends BaseFieldsGenerator<EmptyCellModel, MineCellModel, DisplayClass, EmptyCellView<DisplayClass>,
        MineCellView<DisplayClass>> {

    public PseudoRandomFieldsGenerator(Function<Integer, Integer> horizontalCellsCountParameterFunction, Function<Integer, Integer> verticalCellsCountParameterFunction) {
        super(horizontalCellsCountParameterFunction.andThen(horizontalCellsCountParameter -> (int) (Math.random() * horizontalCellsCountParameter)),
                verticalCellsCountParameterFunction.andThen(verticalCellsCountParameter -> (int) (Math.random() * verticalCellsCountParameter)));
    }

    @Override
    protected int generateHorizontalCellIndex(int horizontalCellsCount) {
        return horizontalGenerateFunction.apply(horizontalCellsCount);
    }

    @Override
    protected int generateVerticalCellIndex(int verticalCellsCount) {
        return verticalGenerateFunction.apply(verticalCellsCount);
    }

    @Override
    protected EmptyCellModel createInstanceOfEmptyCellModelClass(int mineCellModelCountNear) {
        return new EmptyCellModel(mineCellModelCountNear);
    }

    @Override
    protected MineCellModel createInstanceOfMineCellModelClass() {
        return new MineCellModel();
    }

    @Override
    protected EmptyCellView<DisplayClass> createInstanceOfEmptyCellViewClass(int horizontalCellViewCoordinate, int verticalCellViewCoordinate, int horizontalCellViewSize, int verticalCellViewSize) {
        return new EmptyCellView<>(horizontalCellViewCoordinate, verticalCellViewCoordinate, horizontalCellViewSize, verticalCellViewSize);
    }

    @Override
    protected MineCellView<DisplayClass> createInstanceOfMineCellViewClass(int horizontalCellViewCoordinate, int verticalCellViewCoordinate, int horizontalCellViewSize, int verticalCellViewSize) {
        return new MineCellView<>(horizontalCellViewCoordinate, verticalCellViewCoordinate, horizontalCellViewSize, verticalCellViewSize);
    }

}
