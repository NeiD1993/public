package MVP.Views.Classes;

import General.Classes.Events.RestartEvent;
import MVP.Presenters.Interfaces.Listeners.IRestartEventListener;
import MVP.Views.Enums.ControlButtonStates;
import MVP.Views.Interfaces.IControlPanel;

import javax.imageio.ImageIO;
import javax.swing.*;
import javax.swing.Timer;
import java.awt.*;
import java.awt.event.*;
import java.awt.image.ImageObserver;
import java.io.IOException;
import java.text.DateFormat;
import java.util.*;
import java.util.List;

/**
 * Created by NeiD on 14.09.2016.
 */
public class ControlPanel extends JPanel implements IControlPanel {

    protected int horizontalControlPanelSize;

    protected int verticalControlPanelSize;

    protected int horizontalControlButtonSize;

    protected int horizontalFlagImageSize;

    protected int verticalFlagImageSize;

    protected int verticalSpace;

    protected static String imagesExtension;

    protected static String imagesFolder;

    protected static String exceptionMessage;

    protected static String flagImageName;

    protected static Image flagImage;

    protected ImageObserver imageObserver;

    protected static Map<ControlButtonStates, Image> controlButtonStatesMap;

    protected List<IRestartEventListener> restartEventListeners;

    static {
        imagesExtension = "png";
        imagesFolder = "Resources";
        exceptionMessage = "Exception in ControlPanel";
        flagImageName = "Flag";
        controlButtonStatesMap = new EnumMap<>(ControlButtonStates.class);
        getFlagImage();
        getControlButtonStatesMapByControlButtonStates(controlButtonStatesMap, ControlButtonStates.values());
    }

    public ControlPanel(int horizontalControlPanelSize, int verticalControlPanelSize, int horizontalControlButtonSize, int verticalControlButtonSize, int horizontalTimerLabelSize,
                        int verticalTimerLabelSize, long initialTime, int timerDelay, DateFormat dateFormat, Font timerLabelFont, int horizontalFlagImageSize, int verticalFlagImageSize,
                        int horizontalFlagsLabelSize, int verticalFlagsLabelSize, Font flagsLabelFont, int verticalSpace, ImageObserver imageObserver, int mineCellsCount) {
        setLayout(null);
        setBackground(Color.WHITE);
        this.horizontalControlPanelSize = horizontalControlPanelSize;
        this.verticalControlPanelSize = verticalControlPanelSize;
        this.horizontalControlButtonSize = horizontalControlButtonSize;
        this.verticalSpace = verticalSpace;
        setControlPanelSize();
        addControlButton(verticalControlButtonSize);
        addTimerLabel(horizontalTimerLabelSize, verticalTimerLabelSize, initialTime, timerDelay, dateFormat, timerLabelFont);
        addFlagsLabel(horizontalFlagsLabelSize, verticalFlagsLabelSize, flagsLabelFont, mineCellsCount);
        setHorizontalFlagImageSize(horizontalFlagImageSize);
        setVerticalFlagImageSize(verticalFlagImageSize);
        setImageObserver(imageObserver);
        restartEventListeners = new ArrayList<>();
    }

    protected static void getFlagImage() {
        try {

            flagImage = ImageIO.read(ControlPanel.class.getClassLoader().getResource(imagesFolder + "/" + flagImageName + "." + imagesExtension));
        }
        catch (IOException exception) {
            System.out.println(exceptionMessage + "getFlagImage");
        }
    }

    protected static void getControlButtonStatesMapByControlButtonStates(Map<ControlButtonStates, Image> controlButtonStatesMap, ControlButtonStates[] controlButtonStates) {
        try {
            for (ControlButtonStates controlButtonState : controlButtonStates)
                controlButtonStatesMap.put(controlButtonState, ImageIO.read(ControlPanel.class.getClassLoader().getResource(imagesFolder + "/" + controlButtonState.toString() + "." + imagesExtension)));
        }
        catch (IOException exception) {
            System.out.println(exceptionMessage + ".getControlButtonStatesMapByControlButtonStates");
        }
    }

    protected void fireRestartEvent(RestartEvent restartEvent) {
        try {
            restartEventListeners.forEach(restartEventListener -> restartEventListener.restartEventAction(restartEvent));
        }
        catch (Exception exception) {
            System.out.println(exceptionMessage + ".fireRestartEvent");
        }
    }

    @Override
    protected void paintComponent(Graphics display) {
        super.paintComponent(display);
        try {
            display.drawImage(flagImage, (3 * horizontalControlPanelSize - horizontalControlButtonSize) / 4 + horizontalFlagImageSize, (verticalControlPanelSize - verticalFlagImageSize) / 2 +
                            verticalSpace, horizontalFlagImageSize, verticalFlagImageSize, imageObserver);
        }
        catch (Exception exception) {
            System.out.println(exceptionMessage + ".paintComponent");
        }
    }

    @Override
    public void setControlPanelSize() {
        setPreferredSize(new Dimension(horizontalControlPanelSize, verticalControlPanelSize + verticalSpace));
    }

    @Override
    public void addControlButton(int verticalControlButtonSize) {
        ControlButton controlButton = new ControlButton(horizontalControlPanelSize, horizontalControlButtonSize, verticalControlButtonSize, verticalSpace);
        add(controlButton);
    }

    @Override
    public void addTimerLabel(int horizontalTimerLabelSize, int verticalTimerLabelSize, long initialTime, int timerDelay, DateFormat dateFormat, Font font) {
        TimerLabel timerLabel = new TimerLabel(horizontalTimerLabelSize, verticalTimerLabelSize, verticalSpace, initialTime, timerDelay, dateFormat, font);
        add(timerLabel);
    }

    @Override
    public void addFlagsLabel(int horizontalFlagsLabelSize, int verticalFlagsLabelSize, Font font, int mineCellsCount) {
        FlagsLabel flagsLabel = new FlagsLabel(horizontalFlagsLabelSize, verticalFlagsLabelSize, font, mineCellsCount);
        add(flagsLabel);
    }

    @Override
    public void addRestartEventListener(IRestartEventListener restartEventListener) {
        if (restartEventListener != null)
            restartEventListeners.add(restartEventListener);
    }

    @Override
    public void removeRestartEventListener(IRestartEventListener restartEventListener) {
        restartEventListeners.remove(restartEventListener);
    }

    @Override
    public void setControlButtonStateAndControlButtonIcon(ControlButtonStates controlButtonState) {
        try {
            ((ControlButton)getComponent(0)).setControlButtonStateAndControlButtonIcon(controlButtonState);
        }
        catch (Exception exception) {
            System.out.println(exceptionMessage + ".setControlButtonStateAndControlButtonIcon");
        }
    }

    @Override
    public void stopTimerLabel() {
        try {
            ((TimerLabel)getComponent(1)).stopTimer();
        }
        catch (Exception exception) {
            System.out.println(exceptionMessage + ".stopTimerLabel");
        }
    }

    @Override
    public void startTimerLabel() {
        try {
            ((TimerLabel)getComponent(1)).startTimer();
        }
        catch (Exception exception) {
            System.out.println(exceptionMessage + ".startTimerLabel");
        }
    }

    @Override
    public void increaseFlagsLabelValue() {
        try {
            ((FlagsLabel)getComponent(2)).increaseLabelValue();
        }
        catch (Exception exception) {
            System.out.println(exceptionMessage + ".increaseFlagsLabelValue");
        }
    }

    @Override
    public void decreaseFlagsLabelValue() {
        try {
            ((FlagsLabel)getComponent(2)).decreaseLabelValue();
        }
        catch (Exception exception) {
            System.out.println(exceptionMessage + ".decreaseFlagsLabelValue");
        }
    }

    @Override
    public void resetFlagsLabelValue() {
        try {
            ((FlagsLabel)getComponent(2)).resetLabelValue();
        }
        catch (Exception exception) {
            System.out.println(exceptionMessage + ".resetFlagsLabelValue");
        }
    }

    @Override
    public void setHorizontalFlagImageSize(int horizontalFlagImageSize) {
        if (horizontalFlagImageSize > 0)
            this.horizontalFlagImageSize = horizontalFlagImageSize;
    }

    @Override
    public void setVerticalFlagImageSize(int verticalFlagImageSize) {
        if (verticalFlagImageSize > 0)
            this.verticalFlagImageSize = verticalFlagImageSize;
    }

    @Override
    public void setImageObserver(ImageObserver imageObserver) {
        if (imageObserver != null)
            this.imageObserver = imageObserver;
    }

    @Override
    public ControlButtonStates getControlButtonState() {
        try {
            return ((ControlButton)getComponent(0)).getState();
        }
        catch (Exception exception) {
            System.out.println(exceptionMessage + ".getControlButtonState");
            return null;
        }
    }


    protected interface IControlButton {

        void setControlButtonBounds(int horizontalControlPanelSize, int horizontalControlButtonSize, int verticalControlButtonSize, int verticalSpace);

        void setControlButtonStateAndControlButtonIcon(ControlButtonStates controlButtonState);

        ControlButtonStates getState();

    }


    protected class ControlButton extends JButton implements IControlButton {

        protected ControlButtonStates controlButtonState;

        ControlButton(int horizontalControlPanelSize, int horizontalControlButtonSize, int verticalControlButtonSize, int verticalSpace) {
            super();
            setControlButtonBounds(horizontalControlPanelSize, horizontalControlButtonSize, verticalControlButtonSize, verticalSpace);
            setControlButtonStateAndControlButtonIcon(ControlButtonStates.Standart);
            addMouseListener(new ControlButtonMouseListener());
        }

        @Override
        public void setControlButtonStateAndControlButtonIcon(ControlButtonStates controlButtonState) {
            try {
                if (controlButtonState != null) {
                    this.controlButtonState = controlButtonState;
                    setIcon(new ImageIcon(controlButtonStatesMap.get(this.controlButtonState)));
                }
            }
            catch (NullPointerException nullPointerException) {
                System.out.println(exceptionMessage + ".ControlButton.setControlButtonIconByControlButtonState");
            }
        }

        @Override
        public void setControlButtonBounds(int horizontalControlPanelSize, int horizontalControlButtonSize, int verticalControlButtonSize, int verticalSpace) {
            setBounds((horizontalControlPanelSize- horizontalControlButtonSize) / 2, verticalSpace, horizontalControlButtonSize, verticalControlButtonSize);
        }

        @Override
        public ControlButtonStates getState() {
            return controlButtonState;
        }


        protected class ControlButtonMouseListener implements MouseListener {

            @Override
            public void mouseClicked(MouseEvent e) {

            }

            @Override
            public void mousePressed(MouseEvent e) {

            }

            @Override
            public void mouseReleased(MouseEvent e) {
                if (e.getModifiers() == MouseEvent.BUTTON1_MASK && contains(e.getPoint()) && controlButtonState != null) {
                    RestartEvent restartEvent = new RestartEvent(this);
                    fireRestartEvent(restartEvent);
                }
            }

            @Override
            public void mouseEntered(MouseEvent e) {

            }

            @Override
            public void mouseExited(MouseEvent e) {

            }

        }

    }


    protected interface IControlPanelLabel {

        void setLabelFont(Font font);

    }


    protected interface ITimerLabel extends IControlPanelLabel {

        boolean setInitialTime(long initialTime);

        void startTimer();

        void stopTimer();

        void setDateFormat(DateFormat dateFormat);

        void setTimerLabelBounds(int horizontalTimerLabelSize, int verticalTimerLabelSize, int verticalSpace);

    }


    protected class TimerLabel extends JLabel implements ITimerLabel {

        protected long initialTime;

        protected long time;

        protected Timer timer;

        protected DateFormat dateFormat;

        TimerLabel(int horizontalTimerLabelSize, int verticalTimerLabelSize, int verticalSpace, long initialTime, int timerDelay, DateFormat dateFormat, Font font) {
            super(dateFormat.format(new Date(initialTime)).toString(), (int) CENTER_ALIGNMENT);
            setTimerLabelBounds(horizontalTimerLabelSize, verticalTimerLabelSize, verticalSpace);
            if (setInitialTime(initialTime))
                time = initialTime;
            setDateFormat(dateFormat);
            if (timerDelay > 0)
                timer = new Timer(timerDelay, new ClockListener());
            if (dateFormat != null)
                this.dateFormat = dateFormat;
            setLabelFont(font);
            if (timer != null)
                timer.start();
        }

        @Override
        public boolean setInitialTime(long initialTime) {
            if (initialTime > 0) {
                this.initialTime = initialTime;
                return true;
            }
            return false;
        }

        @Override
        public void setLabelFont(Font font) {
            if (font != null)
                setFont(font);
        }

        @Override
        public void startTimer() {
            if (initialTime > 0 && timer != null) {
                if (timer.isRunning())
                    timer.stop();
                time = initialTime;
                timer.start();
            }
        }

        @Override
        public void stopTimer() {
            if (timer != null && timer.isRunning())
                timer.stop();
        }

        @Override
        public void setDateFormat(DateFormat dateFormat) {
            if (dateFormat != null)
                this.dateFormat = dateFormat;
        }

        @Override
        public void setTimerLabelBounds(int horizontalTimerLabelSize, int verticalTimerLabelSize, int verticalSpace) {
            setBounds(0, verticalSpace, horizontalTimerLabelSize, verticalTimerLabelSize);
        }


        protected class ClockListener implements ActionListener {

            @Override
            public void actionPerformed(ActionEvent e) {
                try {
                    setText(dateFormat.format(new Date(time)));
                    time += timer.getDelay();
                }
                catch (Exception exception) {
                    System.out.println(exceptionMessage + ".TimerLabel.ClockListener.actionPerformed");
                }
            }

        }

    }


    protected interface IFlagsLabel extends IControlPanelLabel {

        void setFlagsLabelBounds(int horizontalFlagsLabelSize, int verticalFlagsLabelSize);

        void increaseLabelValue();

        void decreaseLabelValue();

        void resetLabelValue();

    }


    protected class FlagsLabel extends JLabel implements IFlagsLabel {

        protected int initialFlagsLabelValue;

        FlagsLabel(int horizontalFlagsLabelSize, int verticalFlagsLabelSize, Font font,int mineCellsCount) {
            super(Integer.toString(mineCellsCount), RIGHT);
            initialFlagsLabelValue = mineCellsCount;
            setFlagsLabelBounds(horizontalFlagsLabelSize, verticalFlagsLabelSize);
            setLabelFont(font);
        }

        @Override
        public void setLabelFont(Font font) {
            if (font != null)
                setFont(font);
        }

        @Override
        public void increaseLabelValue() {
            int flagsLabelValue = Integer.parseInt(getText());
            setText(Integer.toString(++flagsLabelValue));
        }

        @Override
        public void decreaseLabelValue() {
            int flagsLabelValue = Integer.parseInt(getText());
            setText(Integer.toString(--flagsLabelValue));
        }

        @Override
        public void resetLabelValue() {
            setText(Integer.toString(initialFlagsLabelValue));
        }

        @Override
        public void setFlagsLabelBounds(int horizontalFlagsLabelSize, int verticalFlagsLabelSize) {
            setBounds((horizontalControlPanelSize + horizontalControlButtonSize) / 2, (verticalControlPanelSize - verticalFlagsLabelSize) / 2 + verticalSpace,
                    horizontalFlagsLabelSize, verticalFlagsLabelSize);
        }

    }

}
