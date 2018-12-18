package MVP.Views.Classes;

import MVP.Views.Classes.Events.FieldViewStopInputEvent;
import MVP.Views.Enums.ControlButtonStates;
import MVP.Views.Interfaces.IControlPanel;
import MVP.Views.Interfaces.IFieldView;
import MVP.Views.Interfaces.IWindow;
import MVP.Views.Interfaces.Listeners.IFieldViewStopInputEventListener;

import javax.imageio.ImageIO;
import javax.swing.*;
import java.awt.*;
import java.io.IOException;
import java.util.ArrayList;

/**
 * Created by NeiD on 10.09.2016.
 */
public class Window<DisplayClass extends Graphics> extends JFrame implements IWindow<DisplayClass>, IFieldViewStopInputEventListener {

    protected static String exceptionMessage;

    protected static String iconImageFolder;

    protected static String iconImageName;

    protected static String taskBarIconImageName;

    protected static String iconImageExtension;

    protected static String nullPointerExceptionMessage;

    protected IControlPanel controlPanel;

    protected IFieldView<DisplayClass> fieldView;

    static {
        exceptionMessage = "Exception in Window";
        iconImageFolder = "Resources";
        iconImageName = "Icon";
        taskBarIconImageName = "TaskBarIcon";
        iconImageExtension = "png";
        nullPointerExceptionMessage = "NullPointerException in Window";
    }

    public Window(String windowTitle, boolean isWindowVisible,
                  boolean isWindowResizable, LayoutManager layoutManager, IFieldView<DisplayClass> fieldView, Object fieldViewConstraints, IControlPanel controlPanel, Object controlPanelConstraints) {
        super(windowTitle);
        try {
            ArrayList<Image> imageArrayList = new ArrayList<>();
            imageArrayList.add(ImageIO.read(Window.class.getClassLoader().getResource(iconImageFolder + "/" + iconImageName + "." + iconImageExtension)));
            imageArrayList.add(ImageIO.read(Window.class.getClassLoader().getResource(iconImageFolder + "/" + taskBarIconImageName + "." + iconImageExtension)));
            setIconImages(imageArrayList);
        }
        catch (IOException exception) {
            System.out.println(nullPointerExceptionMessage);
        }
        setWindowResizable(isWindowResizable);
        setWindowLayout(layoutManager);
        addControlPanelAndControlPanelConstraints(controlPanel, controlPanelConstraints);
        addFieldViewAndFieldViewConstraints(fieldView, fieldViewConstraints);
        pack();
        setLocationRelativeTo(null);
        setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        setWindowVisible(isWindowVisible);
    }

    @Override
    public void setWindowVisible(boolean isWindowVisible) {
        setVisible(isWindowVisible);
    }

    @Override
    public void setWindowResizable(boolean isWindowResizable) {
        setResizable(isWindowResizable);
    }

    @Override
    public void setWindowLayout(LayoutManager layoutManager) {
        if (layoutManager != null)
            setLayout(layoutManager);
    }

    @Override
    public void addFieldViewAndFieldViewConstraints(IFieldView<DisplayClass> fieldView, Object fieldViewConstraints) {
        if (fieldView != null && fieldViewConstraints != null) {
            if (this.fieldView != null) {
                remove((Component)this.fieldView);
                this.fieldView.removeFieldViewStopInputEventListenerForFieldViewStopInputTimerRunner(this);
            }
            try {
                add((Component)fieldView, fieldViewConstraints);
            }
            catch (Exception exception) {
                System.out.println(exceptionMessage + ".addFieldViewAndFieldViewConstraints");
            }
            this.fieldView = fieldView;
            this.fieldView.addFieldViewStopInputEventListenerForFieldViewStopInputTimerRunner(this);
        }
    }

    @Override
    public void addControlPanelAndControlPanelConstraints(IControlPanel controlPanel, Object controlPanelConstraints) {
        if (controlPanel != null && controlPanelConstraints != null) {
            if (this.controlPanel != null)
                remove((Component)this.controlPanel);
            try {
                add((Component)controlPanel, controlPanelConstraints);
            }
            catch (Exception exception) {
                System.out.println(exceptionMessage + ".addControlPanelAndControlPanelConstraints");
            }
            this.controlPanel = controlPanel;
        }
    }

    @Override
    public void fieldViewStopInputEventAction(FieldViewStopInputEvent fieldViewStopInputEvent) {
        if (controlPanel != null) {
            controlPanel.setControlButtonStateAndControlButtonIcon(ControlButtonStates.Wait);
            fieldView.stopFieldViewStopInputTimerForFieldViewStopInputTimerRunner();
        }
    }

}
