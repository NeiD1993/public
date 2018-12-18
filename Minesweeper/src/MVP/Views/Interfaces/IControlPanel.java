package MVP.Views.Interfaces;

import MVP.Presenters.Interfaces.Listeners.IRestartEventListener;
import MVP.Views.Enums.ControlButtonStates;

import java.awt.*;
import java.awt.image.ImageObserver;
import java.text.DateFormat;

/**
 * Created by NeiD on 14.09.2016.
 */
public interface IControlPanel {

    void setControlPanelSize();

    void addControlButton(int verticalControlPanelSize);

    void addTimerLabel(int horizontalTimerLabelSize, int verticalTimerLabelSize, long initialTime, int timerDelay, DateFormat dateFormat, Font textFont);

    void addFlagsLabel(int horizontalFlagLabelSize, int verticalFlagLabelSize, Font font, int mineCellsCount);

    void addRestartEventListener(IRestartEventListener restartEventListener);

    void removeRestartEventListener(IRestartEventListener restartEventListener);

    void setControlButtonStateAndControlButtonIcon(ControlButtonStates controlButtonState);

    void stopTimerLabel();

    void startTimerLabel();

    void increaseFlagsLabelValue();

    void decreaseFlagsLabelValue();

    void resetFlagsLabelValue();

    void setHorizontalFlagImageSize(int horizontalFlagImageSize);

    void setVerticalFlagImageSize(int verticalFlagImageSize);

    void setImageObserver(ImageObserver imageObserver);

    ControlButtonStates getControlButtonState();

}
