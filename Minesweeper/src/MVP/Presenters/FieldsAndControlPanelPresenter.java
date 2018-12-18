package MVP.Presenters;

import General.Classes.Events.EndEvent;
import General.Classes.Events.Fields.FieldModels.FieldModelOpenEmptyCellModelEvent;
import General.Classes.Events.Fields.FieldModels.FieldModelOpenMineCellModelEvent;
import General.Classes.Events.Fields.FieldModels.FieldModelSuggestCellModelEvent;
import General.Classes.Events.Fields.FieldViews.*;
import General.Classes.Events.CellViews.BaseFieldViewCellViewChangeEvent;
import General.Classes.Events.CellViews.FieldViewCellViewFocusEvent;
import General.Classes.Events.CellViews.FieldViewCellViewPressEvent;
import General.Classes.Events.RestartEvent;
import General.Enums.EndStates;
import General.Interfaces.IFieldsGenerator;
import MVP.Presenters.Interfaces.IFieldsAndControlPanelPresenter;
import MVP.Presenters.Interfaces.Listeners.FieldViews.IFieldViewCellViewChangeEventListener;
import MVP.Presenters.Interfaces.Listeners.FieldViews.IFieldViewInputEventListener;
import MVP.Presenters.Interfaces.Listeners.FieldViews.IFieldViewMouseExitedEventListener;
import MVP.Presenters.Interfaces.Listeners.IEndEventListener;
import MVP.Presenters.Interfaces.Listeners.IFieldModelChangeCellModelStateEventListener;
import MVP.Presenters.Interfaces.Listeners.IRestartEventListener;
import MVP.Models.Classes.CellModels.EmptyCellModel;
import MVP.Models.Classes.CellModels.MineCellModel;
import MVP.Models.Classes.FieldModels.FieldModel;
import MVP.Models.Enums.CellModelStates;
import MVP.Models.Interfaces.CellModels.ICellModel;
import MVP.Views.Classes.FieldView;
import MVP.Views.Enums.CellViewStates.EmptyCellViewStates;
import MVP.Views.Enums.CellViewStates.GeneralCellViewStates;
import MVP.Views.Enums.CellViewStates.MineCellViewStates;
import MVP.Views.Enums.ControlButtonStates;
import MVP.Views.Interfaces.IControlPanel;

import java.awt.*;
import java.awt.event.MouseEvent;
import java.util.*;
import java.util.List;
import java.util.function.Consumer;

/**
 * Created by NeiD on 10.09.2016.
 */
public class FieldsAndControlPanelPresenter<EmptyCellModelClass extends EmptyCellModel, MineCellModelClass extends MineCellModel, FieldModelClass extends FieldModel<EmptyCellModelClass, MineCellModelClass>,
        DisplayClass extends Graphics, FieldViewClass extends FieldView<DisplayClass>> implements
        IFieldsAndControlPanelPresenter<EmptyCellModelClass, MineCellModelClass, FieldModelClass, DisplayClass, FieldViewClass>, IFieldModelChangeCellModelStateEventListener,
        IFieldViewInputEventListener, IFieldViewCellViewChangeEventListener, IFieldViewMouseExitedEventListener, IEndEventListener, IRestartEventListener {

    protected FieldModelClass fieldModel;

    protected FieldViewClass fieldView;

    protected IControlPanel controlPanel;

    protected IFieldsGenerator<EmptyCellModelClass, MineCellModelClass, DisplayClass> fieldsGenerator;

    public FieldsAndControlPanelPresenter(FieldModelClass fieldModel, FieldViewClass fieldView, IControlPanel controlPanel,
                                          IFieldsGenerator<EmptyCellModelClass, MineCellModelClass, DisplayClass> fieldsGenerator) {
        setFieldModel(fieldModel);
        setFieldView(fieldView);
        setControlPanel(controlPanel);
        setFieldsGenerator(fieldsGenerator);
        if (fieldsGenerator != null)
            fieldsGenerator.generateFields(fieldModel, fieldView);
    }

    protected void changePreviousCellViewStateByFieldViewCellViewChangeEvent(Point previousChangedCellViewIndexes, GeneralCellViewStates cellViewState) {
        if (previousChangedCellViewIndexes != null) {
            if (fieldModel.getCellByIndexes(previousChangedCellViewIndexes.x, previousChangedCellViewIndexes.y).getCellModelState() == CellModelStates.Initial)
                fieldView.repaintCellView(previousChangedCellViewIndexes, cellViewState);
        }
    }

    protected void changeCellViewStateOnFocusedOrPressedByFieldViewCellViewChangeEvent(Point changedCellViewIndexes, BaseFieldViewCellViewChangeEvent fieldViewCellViewChangeEvent) {
        if (fieldModel.getCellByIndexes(changedCellViewIndexes.x, changedCellViewIndexes.y).getCellModelState() == CellModelStates.Initial) {
            if (fieldViewCellViewChangeEvent.getClass() == FieldViewCellViewFocusEvent.class)
                fieldView.repaintCellView(changedCellViewIndexes, GeneralCellViewStates.Focused);
            else {
                if (((FieldViewCellViewPressEvent)fieldViewCellViewChangeEvent).getMouseEvent().getModifiers() == MouseEvent.BUTTON1_MASK)
                    fieldView.repaintCellView(changedCellViewIndexes, GeneralCellViewStates.Pressed);
                else
                    fieldView.repaintCellView(changedCellViewIndexes, GeneralCellViewStates.Focused);
            }
        }
    }

    protected void endOrRestartEventChangingCellViewStateOfCellModels(List<Consumer<Integer>> endOrRestartEventConsumers) {
        for (int i = 0; i < fieldModel.getHorizontalCellsCount(); i++) {
            for (Consumer<Integer> endOrRestartEventConsumer : endOrRestartEventConsumers)
                endOrRestartEventConsumer.accept(i);
        }
    }

    protected void endEventChangingCellViewStateOfUnopenedEmptyCellModelsByCellModelState(int horizontalEmptyCellModelsIndex) {
        Map<Integer, EmptyCellModelClass> unopenedEmptyCellModelsMap = fieldModel.getUnopenedEmptyCellModelsMapByHorizontalIndex(horizontalEmptyCellModelsIndex);
        for (Map.Entry<Integer, EmptyCellModelClass> unopenedEmptyCellModelEntry :unopenedEmptyCellModelsMap.entrySet()) {
            EmptyCellModelClass unopenedEmptyCellModel = unopenedEmptyCellModelEntry.getValue();
            if (unopenedEmptyCellModel.getCellModelState() == CellModelStates.Suggested)
                fieldView.repaintCellView(new Point(horizontalEmptyCellModelsIndex, unopenedEmptyCellModelEntry.getKey()), EmptyCellViewStates.WrongSuggested);
            else
                fieldView.repaintCellView(new Point(horizontalEmptyCellModelsIndex, unopenedEmptyCellModelEntry.getKey()),
                        EmptyCellViewStates.getEmptyCellViewStateByMinesNearCount(unopenedEmptyCellModel.getMineCellModelsNear()));
        }
    }

    protected void endEventChangingCellViewStateOfUnopenedMineCellModelsByCellModelState(int horizontalMineCellModelsIndex) {
        Map<Integer, MineCellModelClass> unopenedMineCellModelsMap = fieldModel.getUnopenedMineCellModelsByHorizontalIndex(horizontalMineCellModelsIndex);
        for (Map.Entry<Integer, MineCellModelClass> unopenedMineCellModelEntry : unopenedMineCellModelsMap.entrySet()) {
            if (unopenedMineCellModelEntry.getValue().getCellModelState() == CellModelStates.Initial)
                fieldView.repaintCellView(new Point(horizontalMineCellModelsIndex, unopenedMineCellModelEntry.getKey()), MineCellViewStates.Demined);
            else
                fieldView.repaintCellView(new Point(horizontalMineCellModelsIndex, unopenedMineCellModelEntry.getKey()), MineCellViewStates.RightSuggested);
        }
    }

    protected void restartEventChangingCellViewStateOfOpenedOrSuggetsedCellModelsOnStandartControlButtonState(int horizontalMineCellModelsIndex) {
        Map<Integer, ICellModel> openedOrSuggestedCellModelsMap = fieldModel.getOpenedOrSuggestedCellModelsMapByHorizontalIndex(horizontalMineCellModelsIndex);
        for (Map.Entry<Integer, ICellModel> openedOrSuggestedCellModelEntry : openedOrSuggestedCellModelsMap.entrySet())
                fieldView.repaintCellView(new Point(horizontalMineCellModelsIndex, openedOrSuggestedCellModelEntry.getKey()), GeneralCellViewStates.Initial);
    }

    @Override
    public void setFieldModel(FieldModelClass fieldModel) {
        if (fieldModel != null) {
            if (this.fieldModel != null) {
                this.fieldModel.removeFieldModelChangeCellModelStateEventListeners(this);
                this.fieldModel.removeEndEventListener(this);
            }
            this.fieldModel = fieldModel;
            this.fieldModel.addFieldModelChangeCellModelStateEventListeners(this);
            this.fieldModel.addEndEventListener(this);
        }
    }

    @Override
    public void setFieldView(FieldViewClass fieldView) {
        if (fieldView != null) {
            if (this.fieldView != null) {
                this.fieldView.removeFieldViewInputEventListener(this);
                this.fieldView.removeFieldViewCellViewChangeEventListener(this);
                this.fieldView.removeFieldViewMouseExitedEventListener(this);
                this.fieldView.setPreviousChangedCellViewIndexes(null);
            }
            this.fieldView = fieldView;
            this.fieldView.addFieldViewInputEventListener(this);
            this.fieldView.addFieldViewCellViewChangeEventListener(this);
            this.fieldView.addFieldViewMouseExitedEventListener(this);
        }
    }

    @Override
    public void setControlPanel(IControlPanel controlPanel) {
        if (controlPanel != null) {
            if (this.controlPanel != null)
                this.controlPanel.removeRestartEventListener(this);
            this.controlPanel = controlPanel;
            this.controlPanel.addRestartEventListener(this);
        }
    }

    @Override
    public void setFieldsGenerator(IFieldsGenerator<EmptyCellModelClass, MineCellModelClass, DisplayClass> fieldsGenerator) {
        if (fieldsGenerator != null)
            this.fieldsGenerator = fieldsGenerator;
    }

    @Override
    public void fieldModelOpenEmptyCellModelEventAction(FieldModelOpenEmptyCellModelEvent fieldModelOpenEmptyCellModelEvent) {
        if (fieldModelOpenEmptyCellModelEvent != null && fieldView != null) {
            Point openedEmptyCellModelIndexes = fieldModelOpenEmptyCellModelEvent.getChangedCellIndexes();
            fieldView.repaintCellView(openedEmptyCellModelIndexes, EmptyCellViewStates.getEmptyCellViewStateByMinesNearCount(fieldModelOpenEmptyCellModelEvent.getMineCellModelsNear()));
        }
    }

    @Override
    public void fieldModelOpenMineCellModelEventAction(FieldModelOpenMineCellModelEvent fieldModelOpenMineCellModelEvent) {
        if (fieldModelOpenMineCellModelEvent != null && fieldView != null) {
            Point openedMineCellModelIndexes = fieldModelOpenMineCellModelEvent.getChangedCellIndexes();
            fieldView.repaintCellView(openedMineCellModelIndexes, MineCellViewStates.Explosed);
        }
    }

    @Override
    public void fieldModelSuggestCellModelEventAction(FieldModelSuggestCellModelEvent fieldModelSuggestCellModelEvent) {
        if (fieldModelSuggestCellModelEvent != null && fieldView != null && controlPanel != null) {
            Point suggestedCellModelIndexes = fieldModelSuggestCellModelEvent.getChangedCellIndexes();
            if (fieldModelSuggestCellModelEvent.getCellModelState() == CellModelStates.Suggested) {
                fieldView.repaintCellView(suggestedCellModelIndexes, GeneralCellViewStates.Suggested);
                controlPanel.decreaseFlagsLabelValue();
            }
            else {
                fieldView.repaintCellView(suggestedCellModelIndexes, GeneralCellViewStates.Focused);
                controlPanel.increaseFlagsLabelValue();
            }
        }
    }

    @Override
    public void fieldViewInputEventAction(FieldViewInputEvent fieldViewInputEvent) {
        if (fieldViewInputEvent != null && fieldModel != null && controlPanel != null) {
            Point inputCellViewIndexes = fieldViewInputEvent.getChangedCellIndexes();
            if (inputCellViewIndexes != null) {
                if (controlPanel.getControlButtonState() == ControlButtonStates.Wait)
                    controlPanel.setControlButtonStateAndControlButtonIcon(ControlButtonStates.Standart);
                fieldView.restartFieldViewStopInputTimerForFieldViewStopInputTimerRunner();
                switch (fieldViewInputEvent.getInputAction()) {
                    case Opening:
                        fieldModel.openCellModel(inputCellViewIndexes.x, inputCellViewIndexes.y);
                        break;
                    case Suggesting:
                        fieldModel.suggestCellModel(inputCellViewIndexes.x, inputCellViewIndexes.y);
                        break;
                }
            }
        }
    }

    @Override
    public void fieldViewCellViewChangeEventAction(BaseFieldViewCellViewChangeEvent fieldViewCellViewChangeEvent) {
        if (fieldViewCellViewChangeEvent != null && fieldModel != null) {
            Point previousChangedCellViewIndexes = fieldViewCellViewChangeEvent.getPreviousChangedCellViewIndexes();
            Point nextChangedCellViewIndexes = fieldViewCellViewChangeEvent.getNextChangedCellViewIndexes();
            if (nextChangedCellViewIndexes != null) {
                if (!nextChangedCellViewIndexes.equals(previousChangedCellViewIndexes)) {
                    changePreviousCellViewStateByFieldViewCellViewChangeEvent(previousChangedCellViewIndexes, GeneralCellViewStates.Initial);
                    changeCellViewStateOnFocusedOrPressedByFieldViewCellViewChangeEvent(nextChangedCellViewIndexes, fieldViewCellViewChangeEvent);
                    fieldView.setPreviousChangedCellViewIndexes(nextChangedCellViewIndexes);
                }
                else
                    changeCellViewStateOnFocusedOrPressedByFieldViewCellViewChangeEvent(previousChangedCellViewIndexes, fieldViewCellViewChangeEvent);
            }
        }
    }

    @Override
    public void fieldViewMouseExitedEventAction(FieldViewMouseExitedEvent fieldViewMouseExitedEvent) {
        if (fieldViewMouseExitedEvent != null && fieldModel != null) {
            Point previousChangedCellViewIndexes = fieldViewMouseExitedEvent.getChangedCellIndexes();
            if (fieldViewMouseExitedEvent.IsMouseButtonReleased())
                changePreviousCellViewStateByFieldViewCellViewChangeEvent(previousChangedCellViewIndexes, GeneralCellViewStates.Focused);
            else
                changePreviousCellViewStateByFieldViewCellViewChangeEvent(previousChangedCellViewIndexes, GeneralCellViewStates.Initial);
        }
    }

    @Override
    public void endEventAction(EndEvent endEvent) {
       if (endEvent != null && fieldView != null && controlPanel != null) {
            EndStates endState = endEvent.getEndState();
            if (endState != null) {
                List<Consumer<Integer>> endEventConsumers = new ArrayList<>();
                endEventConsumers.add(horizontalCellModelsIndex -> endEventChangingCellViewStateOfUnopenedMineCellModelsByCellModelState(horizontalCellModelsIndex));
                if (endState == EndStates.Lose) {
                    endEventConsumers.add(horizontalCellModelsIndex -> endEventChangingCellViewStateOfUnopenedEmptyCellModelsByCellModelState(horizontalCellModelsIndex));
                    controlPanel.setControlButtonStateAndControlButtonIcon(ControlButtonStates.Lose);
                }
                else
                    controlPanel.setControlButtonStateAndControlButtonIcon(ControlButtonStates.Win);
                endOrRestartEventChangingCellViewStateOfCellModels(endEventConsumers);
                fieldView.setFieldViewBlocked(true);
                fieldView.stopFieldViewStopInputTimerForFieldViewStopInputTimerRunner();
                controlPanel.stopTimerLabel();
            }
        }
    }

    @Override
    public void restartEventAction(RestartEvent restartEvent) {
        if (fieldModel != null && fieldView != null && fieldsGenerator != null) {
            ControlButtonStates controlButtonState = controlPanel.getControlButtonState();
            if (controlButtonState == ControlButtonStates.Standart) {
                List<Consumer<Integer>> restartEventConsumers = new ArrayList<>();
                restartEventConsumers.add(horizontalCellModelsIndex -> restartEventChangingCellViewStateOfOpenedOrSuggetsedCellModelsOnStandartControlButtonState(horizontalCellModelsIndex));
                endOrRestartEventChangingCellViewStateOfCellModels(restartEventConsumers);
            }
            fieldModel.resetCellModels();
            fieldsGenerator.generateFields(fieldModel, fieldView);
            if (controlButtonState != ControlButtonStates.Standart)
                fieldView.repaint();
            fieldView.setFieldViewBlocked(false);
            controlPanel.setControlButtonStateAndControlButtonIcon(ControlButtonStates.Standart);
            controlPanel.resetFlagsLabelValue();
            fieldView.restartFieldViewStopInputTimerForFieldViewStopInputTimerRunner();
            controlPanel.startTimerLabel();
        }
    }

}
