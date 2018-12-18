package MVP.Views.Classes;

import General.Classes.BaseField;
import General.Classes.Events.Fields.FieldViews.*;
import General.Classes.Events.CellViews.BaseFieldViewCellViewChangeEvent;
import General.Classes.Events.CellViews.FieldViewCellViewFocusEvent;
import General.Classes.Events.CellViews.FieldViewCellViewPressEvent;
import General.Enums.InputActions;
import MVP.Presenters.Interfaces.Listeners.FieldViews.IFieldViewCellViewChangeEventListener;
import MVP.Presenters.Interfaces.Listeners.FieldViews.IFieldViewInputEventListener;
import MVP.Presenters.Interfaces.Listeners.FieldViews.IFieldViewMouseExitedEventListener;
import MVP.Views.Interfaces.Listeners.IFieldViewStopInputEventListener;
import MVP.Views.Classes.Events.FieldViewStopInputEvent;
import MVP.Views.Interfaces.CellViews.ICellView;
import MVP.Views.Interfaces.CellViews.ICellViewStates;
import MVP.Views.Interfaces.IFieldView;

import javax.swing.*;
import java.awt.*;
import java.awt.event.*;
import java.awt.image.ImageObserver;
import java.util.ArrayList;
import java.util.List;

/**
 * Created by NeiD on 09.09.2016.
 */
public class FieldView<DisplayClass extends Graphics> extends JPanel implements IFieldView<DisplayClass> {

    protected static String exceptionMessage;

    protected boolean isFieldViewBlocked;

    protected int horizontalCellViewSize;

    protected int verticalCellViewSize;

    protected CellViews cellViews;

    protected ImageObserver imageObserver;

    protected Point previousChangedCellViewIndexes;

    protected FieldViewStopInputTimerRunner fieldViewStopInputTimerRunner;

    protected List<IFieldViewInputEventListener> fieldViewInputEventListeners;

    protected List<IFieldViewCellViewChangeEventListener> fieldViewCellViewChangeEventListeners;

    protected List<IFieldViewMouseExitedEventListener> fieldViewMouseExitedEventListeners;

    static {
        exceptionMessage = "Exception in FieldView";
    }

    public FieldView(int horizontalCellViewsCount, int verticalCellViewsCount, int horizontalCellViewSize, int verticalCellViewSize, ImageObserver imageObserver, int fieldViewStopInputTimerDelay,
                     int secondsOnFieldViewStopInputEventFire) {
        resetCellViews(horizontalCellViewsCount, verticalCellViewsCount);
        if (horizontalCellViewSize > 0)
            this.horizontalCellViewSize = horizontalCellViewSize;
        if (verticalCellViewSize > 0)
            this.verticalCellViewSize = verticalCellViewSize;
        setImageObserver(imageObserver);
        setFieldViewSize();
        addMouseListener(new FieldViewMouseListener());
        addMouseMotionListener(new FieldViewMouseMotionListener());
        fieldViewInputEventListeners = new ArrayList<>();
        fieldViewCellViewChangeEventListeners = new ArrayList<>();
        fieldViewMouseExitedEventListeners = new ArrayList<>();
        if (fieldViewStopInputTimerDelay > 0 && secondsOnFieldViewStopInputEventFire > 0)
            fieldViewStopInputTimerRunner = new FieldViewStopInputTimerRunner(fieldViewStopInputTimerDelay, secondsOnFieldViewStopInputEventFire);
    }

    private Point findCellViewIndexesByFieldViewPoint(Point pointOnFieldView) {
        if (horizontalCellViewSize > 0 && verticalCellViewSize > 0) {
            int horizontalCellViewIndex = 0;
            int verticalCellViewIndex = 0;
            double horizontalCoordinateOnFieldView = pointOnFieldView.getX();
            double verticalCoordinateOnFieldView = pointOnFieldView.getY();
            if (horizontalCoordinateOnFieldView == getWidth())
                horizontalCellViewIndex--;
            if (verticalCoordinateOnFieldView == getHeight())
                verticalCellViewIndex--;
            horizontalCellViewIndex += Math.floor(horizontalCoordinateOnFieldView / horizontalCellViewSize);
            verticalCellViewIndex += Math.floor(verticalCoordinateOnFieldView / verticalCellViewSize);
            return new Point(horizontalCellViewIndex, verticalCellViewIndex);
        }
        else
            return null;
    }

    protected void fireFieldViewInputEvent(FieldViewInputEvent fieldViewInputEvent) {
        try {
            fieldViewInputEventListeners.forEach(fieldViewInputEventListener -> fieldViewInputEventListener.fieldViewInputEventAction(fieldViewInputEvent));
        }
        catch (Exception exception) {
            System.out.println(exceptionMessage + ".fireFieldViewInputEvent");
        }
    }

    protected void fireFieldViewCellViewChangeEvent(BaseFieldViewCellViewChangeEvent fieldViewCellViewChangeEvent) {
        try {
            fieldViewCellViewChangeEventListeners.forEach(fieldViewCellViewChangeEventListener ->
                    fieldViewCellViewChangeEventListener.fieldViewCellViewChangeEventAction(fieldViewCellViewChangeEvent));
        }
        catch (Exception exception) {
            System.out.println(exceptionMessage + ".fireFieldViewCellViewChangeEvent");
        }
    }

    protected void fireFieldViewMouseExitedEvent(FieldViewMouseExitedEvent fieldViewMouseExitedEvent) {
        try {
            fieldViewMouseExitedEventListeners.forEach(fieldViewMouseExitedEventListener -> fieldViewMouseExitedEventListener.fieldViewMouseExitedEventAction(fieldViewMouseExitedEvent));
        }
        catch (Exception exception) {
            System.out.println(exceptionMessage + ".fireFieldViewMouseExitedEvent");
        }
    }

    protected void PressFieldView(MouseEvent mouseEvent) {
        if (!isFieldViewBlocked) {
            Point point = mouseEvent.getPoint();
            if (contains(point)) {
                Point pressedCellViewIndexes = findCellViewIndexesByFieldViewPoint(point);
                if (pressedCellViewIndexes != null) {
                    FieldViewCellViewPressEvent fieldViewCellViewPressEvent = new FieldViewCellViewPressEvent(this, previousChangedCellViewIndexes, pressedCellViewIndexes, mouseEvent);
                    fireFieldViewCellViewChangeEvent(fieldViewCellViewPressEvent);
                }
            }
        }
    }

    @Override
    protected void paintComponent(Graphics display) {
        super.paintComponent(display);
        if (cellViews != null) {
            try {
                DisplayClass displayDisplayClass = (DisplayClass) display;
                for (ICellView<DisplayClass>[] verticalCellViews : cellViews.getCells()) {
                    for (ICellView<DisplayClass> cellView : verticalCellViews)
                        cellView.displayCellView(displayDisplayClass, imageObserver);
                }
            }
            catch (Exception exception) {
                System.out.println(exceptionMessage);
            }
        }
    }

    @Override
    public void addFieldViewInputEventListener(IFieldViewInputEventListener fieldViewInputEventListener) {
        if (fieldViewInputEventListener != null)
            fieldViewInputEventListeners.add(fieldViewInputEventListener);
    }

    @Override
    public void addFieldViewCellViewChangeEventListener(IFieldViewCellViewChangeEventListener fieldViewCellViewChangeEventListener) {
        if (fieldViewCellViewChangeEventListener != null)
            fieldViewCellViewChangeEventListeners.add(fieldViewCellViewChangeEventListener);
    }

    @Override
    public void addFieldViewMouseExitedEventListener(IFieldViewMouseExitedEventListener fieldViewMouseExitedEventListener) {
        if (fieldViewMouseExitedEventListener != null)
            fieldViewMouseExitedEventListeners.add(fieldViewMouseExitedEventListener);
    }

    @Override
    public void addFieldViewStopInputEventListenerForFieldViewStopInputTimerRunner(IFieldViewStopInputEventListener fieldViewStopInputEventListener) {
        if (fieldViewStopInputTimerRunner != null)
            fieldViewStopInputTimerRunner.addFieldViewStopInputEventListener(fieldViewStopInputEventListener);
    }

    @Override
    public void removeFieldViewInputEventListener(IFieldViewInputEventListener fieldViewInputEventListener) {
        fieldViewInputEventListeners.remove(fieldViewInputEventListener);
    }

    @Override
    public void removeFieldViewCellViewChangeEventListener(IFieldViewCellViewChangeEventListener fieldViewCellViewChangeEventListener) {
        fieldViewCellViewChangeEventListeners.remove(fieldViewCellViewChangeEventListener);
    }

    @Override
    public void removeFieldViewMouseExitedEventListener(IFieldViewMouseExitedEventListener fieldViewMouseExitedEventListener) {
        fieldViewMouseExitedEventListeners.remove(fieldViewMouseExitedEventListener);
    }

    @Override
    public void removeFieldViewStopInputEventListenerForFieldViewStopInputTimerRunner(IFieldViewStopInputEventListener fieldViewStopInputEventListener) {
        if (fieldViewStopInputTimerRunner != null)
            fieldViewStopInputTimerRunner.removeFieldViewStopInputEventListener(fieldViewStopInputEventListener);
    }

    @Override
    public void setFieldViewBlocked(boolean fieldViewBlocked) {
        this.isFieldViewBlocked = fieldViewBlocked;
    }

    @Override
    public void resetCellViews(int horizontalCellViewsCount, int verticalCellViewsCount) {
        if (horizontalCellViewsCount > 0 && verticalCellViewsCount > 0) {
            cellViews = new CellViews(horizontalCellViewsCount, verticalCellViewsCount);
        }
    }

    @Override
    public void setHorizontalCellViewSize(int horizontalCellViewSize) {
        if (cellViews != null) {
            if (cellViews.getCells() == null) {
                if (horizontalCellViewSize > 0)
                    this.horizontalCellViewSize = horizontalCellViewSize;
            }
        }
    }

    @Override
    public void setVerticalCellViewSize(int verticalCellViewSize) {
        if (cellViews != null) {
            if (cellViews.getCells() == null) {
                if (verticalCellViewSize > 0)
                    this.verticalCellViewSize = verticalCellViewSize;
            }
        }
    }

    @Override
    public void setImageObserver(ImageObserver imageObserver) {
        this.imageObserver = imageObserver;
    }

    @Override
    public void setPreviousChangedCellViewIndexes(Point previousChangedCellViewIndexes) {
        this.previousChangedCellViewIndexes = previousChangedCellViewIndexes;
    }

    @Override
    public void setFieldViewSize() {
        setPreferredSize(new Dimension(cellViews.getHorizontalCellsCount() * horizontalCellViewSize, cellViews.getVerticalCellsCount() * verticalCellViewSize));
    }

    @Override
    public void repaintCellView(Point cellViewIndexes, ICellViewStates cellViewState) {
        ICellView<DisplayClass> cellView = cellViews.getCellByIndexes(cellViewIndexes.x, cellViewIndexes.y);
        if (cellView != null) {
                cellView.setCellViewState(cellViewState);
                if (cellView.getCellViewState() != null)
                    repaint();
        }
    }

    @Override
    public void stopFieldViewStopInputTimerForFieldViewStopInputTimerRunner() {
        if (fieldViewStopInputTimerRunner != null)
            fieldViewStopInputTimerRunner.stopFieldViewStopInputTimer();
    }

    @Override
    public void restartFieldViewStopInputTimerForFieldViewStopInputTimerRunner() {
        if (fieldViewStopInputTimerRunner != null)
            fieldViewStopInputTimerRunner.restartFieldViewStopInputTimer();
    }

    @Override
    public int getHorizontalCellViewSize() {
        return horizontalCellViewSize;
    }

    @Override
    public int getVerticalCellViewSize() {
        return verticalCellViewSize;
    }

    @Override
    public ICellView<DisplayClass> getCellViewByIndexes(int horizontalCellViewIndex, int verticalCellViewIndex) {
        return cellViews.getCellByIndexes(horizontalCellViewIndex, verticalCellViewIndex);
    }

    @Override
    public ICellView<DisplayClass>[][] getCellViews() {
        return cellViews.getCells();
    }


    protected class CellViews extends BaseField<ICellView<DisplayClass>> {

        CellViews(int horizontalCellViewsCount, int verticalCellViewsCount) {
            super(horizontalCellViewsCount, verticalCellViewsCount);
        }

        @Override
        protected ICellView<DisplayClass>[][] createCellTypes(int horizontalCellsCount, int verticalCellsCount) {
            return new ICellView[horizontalCellsCount][verticalCellsCount];
        }

    }


    protected class FieldViewMouseListener implements MouseListener {

        @Override
        public void mouseClicked(MouseEvent e) {

        }

        @Override
        public void mousePressed(MouseEvent e) {
            PressFieldView(e);
        }

        @Override
        public void mouseReleased(MouseEvent e) {
            if (!isFieldViewBlocked) {
                Point point = e.getPoint();
                if (contains(point)) {
                    InputActions inputAction;
                    switch(e.getButton()) {
                        case 1:
                            inputAction = InputActions.Opening;
                            break;
                        case 3:
                            inputAction = InputActions.Suggesting;
                            break;
                        default:
                            inputAction = null;
                    }
                    if (inputAction != null) {
                        Point releasedCellIndexes = findCellViewIndexesByFieldViewPoint(point);
                        if (releasedCellIndexes != null) {
                            FieldViewInputEvent fieldViewInputEvent = new FieldViewInputEvent(this, releasedCellIndexes, inputAction);
                            fireFieldViewInputEvent(fieldViewInputEvent);
                        }
                    }
                }
                else {
                    FieldViewMouseExitedEvent fieldViewMouseExitedEvent = new FieldViewMouseExitedEvent(this, previousChangedCellViewIndexes, true);
                    fireFieldViewMouseExitedEvent(fieldViewMouseExitedEvent);
                }
            }

        }

        @Override
        public void mouseEntered(MouseEvent e) {

        }

        @Override
        public void mouseExited(MouseEvent e) {
            if (!isFieldViewBlocked) {
                FieldViewMouseExitedEvent fieldViewMouseExitedEvent = new FieldViewMouseExitedEvent(this, previousChangedCellViewIndexes, false);
                fireFieldViewMouseExitedEvent(fieldViewMouseExitedEvent);
            }
        }

    }


    protected class FieldViewMouseMotionListener implements MouseMotionListener {

        @Override
        public void mouseDragged(MouseEvent e) {
            PressFieldView(e);
        }

        @Override
        public void mouseMoved(MouseEvent e) {
            if (!isFieldViewBlocked) {
                Point focusedCellViewIndexes = findCellViewIndexesByFieldViewPoint(e.getPoint());
                if (focusedCellViewIndexes != null) {
                    FieldViewCellViewFocusEvent fieldViewCellViewFocusEvent = new FieldViewCellViewFocusEvent(this, previousChangedCellViewIndexes, focusedCellViewIndexes);
                    fireFieldViewCellViewChangeEvent(fieldViewCellViewFocusEvent);
                }
            }
        }

    }


    protected interface IFieldViewStopInputTimer {

        void setSecondsOnFieldViewStopInputEventFire(int secondsOnFieldViewStopInputEventFire);

        void addFieldViewStopInputEventListener(IFieldViewStopInputEventListener fieldViewStopInputEventListener);

        void removeFieldViewStopInputEventListener(IFieldViewStopInputEventListener fieldViewStopInputEventListener);

        void stopFieldViewStopInputTimer();

        void restartFieldViewStopInputTimer();

    }


    protected class FieldViewStopInputTimerRunner implements IFieldViewStopInputTimer {

        protected int secondsOnFieldViewStopInputEventFire;

        protected int pastSecondsAfterStopInput;

        protected Timer fieldViewStopInputTimer;

        protected List<IFieldViewStopInputEventListener> fieldViewStopInputEventListeners;

        FieldViewStopInputTimerRunner(int timerDelay, int secondsOnFieldViewStopInputEventFire) {
            if (timerDelay > 0)
                fieldViewStopInputTimer = new Timer(timerDelay, new SecondsOnFieldViewStopInputEventFireListener());
            setSecondsOnFieldViewStopInputEventFire(secondsOnFieldViewStopInputEventFire);
            fieldViewStopInputEventListeners = new ArrayList<>();
            fieldViewStopInputTimer.start();
        }

        protected void fireFieldViewStopInputEvent(FieldViewStopInputEvent fieldViewStopInputEvent) {
            try {
                fieldViewStopInputEventListeners.forEach(fieldViewStopInputEventListener -> fieldViewStopInputEventListener.fieldViewStopInputEventAction(fieldViewStopInputEvent));
            }
            catch (Exception exception) {
                System.out.println(exceptionMessage + ".FieldViewStopInputTimer.fireFieldViewStopInputEvent");
            }
        }

        @Override
        public void setSecondsOnFieldViewStopInputEventFire(int secondsOnFieldViewStopInputEventFire) {
            if (secondsOnFieldViewStopInputEventFire > 0)
                this.secondsOnFieldViewStopInputEventFire = secondsOnFieldViewStopInputEventFire;
        }

        @Override
        public void addFieldViewStopInputEventListener(IFieldViewStopInputEventListener fieldViewStopInputEventListener) {
            if (fieldViewStopInputEventListener != null)
                fieldViewStopInputEventListeners.add(fieldViewStopInputEventListener);
        }

        @Override
        public void removeFieldViewStopInputEventListener(IFieldViewStopInputEventListener fieldViewStopInputEventListener) {
            fieldViewStopInputEventListeners.remove(fieldViewStopInputEventListener);
        }

        @Override
        public void stopFieldViewStopInputTimer() {
            if (fieldViewStopInputTimer != null && fieldViewStopInputTimer.isRunning())
                fieldViewStopInputTimer.stop();
        }

        @Override
        public void restartFieldViewStopInputTimer() {
            if (fieldViewStopInputTimer != null) {
                if (fieldViewStopInputTimer.isRunning())
                    fieldViewStopInputTimer.stop();
                if (pastSecondsAfterStopInput > 0)
                    pastSecondsAfterStopInput = 0;
                fieldViewStopInputTimer.start();
            }
        }


        protected class SecondsOnFieldViewStopInputEventFireListener implements ActionListener {

            @Override
            public void actionPerformed(ActionEvent e) {
                try {
                    pastSecondsAfterStopInput += fieldViewStopInputTimer.getDelay();
                    if (pastSecondsAfterStopInput >= secondsOnFieldViewStopInputEventFire) {
                        FieldViewStopInputEvent fieldViewStopInputEvent = new FieldViewStopInputEvent(this);
                        fireFieldViewStopInputEvent(fieldViewStopInputEvent);
                    }
                }
                catch (Exception exception) {
                    System.out.println(exceptionMessage + ".TimerLabel.ClockListener.actionPerformed");
                }
            }

        }

    }

}
