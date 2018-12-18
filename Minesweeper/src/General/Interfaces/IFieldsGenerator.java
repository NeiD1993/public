package General.Interfaces;

import MVP.Models.Classes.CellModels.EmptyCellModel;
import MVP.Models.Classes.CellModels.MineCellModel;
import MVP.Models.Classes.FieldModels.BaseFieldModel;
import MVP.Views.Interfaces.IFieldView;

import java.awt.*;
import java.util.function.Function;;

/**
 * Created by NeiD on 08.09.2016.
 */
public interface IFieldsGenerator<EmptyCellModelClass extends EmptyCellModel, MineCellModelClass extends MineCellModel, DisplayClass extends Graphics> {

    void setHorizontalGenerateFunction(Function<Integer, Integer> horizontalGenerateFunction);

    void setVerticalGenerateFunction(Function<Integer, Integer> verticalGenerateFunction);

    void generateFields(BaseFieldModel<EmptyCellModelClass, MineCellModelClass> fieldModel, IFieldView<DisplayClass> fieldView);

}