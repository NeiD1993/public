package MVP.Presenters.Interfaces;

import General.Interfaces.IFieldsGenerator;
import MVP.Models.Classes.CellModels.EmptyCellModel;
import MVP.Models.Classes.CellModels.MineCellModel;
import MVP.Models.Classes.FieldModels.FieldModel;
import MVP.Views.Classes.FieldView;
import MVP.Views.Interfaces.IControlPanel;

import java.awt.*;

/**
 * Created by NeiD on 10.09.2016.
 */
public interface IFieldsAndControlPanelPresenter<EmptyCellModelClass extends EmptyCellModel, MineCellModelClass extends MineCellModel, FieldModelClass extends FieldModel<EmptyCellModelClass,
        MineCellModelClass>, DisplayClass extends Graphics, FieldViewClass extends FieldView> {

    void setFieldModel(FieldModelClass fieldModel);

    void setFieldView(FieldViewClass fieldView);

    void setControlPanel(IControlPanel controlPanel);

    void setFieldsGenerator(IFieldsGenerator<EmptyCellModelClass, MineCellModelClass, DisplayClass> fieldsGenerator);

}
