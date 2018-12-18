import General.Classes.FieldsGenerators.PseudoRandomFieldsGenerator;
import MVP.Presenters.FieldsAndControlPanelPresenter;
import MVP.Models.Classes.CellModels.EmptyCellModel;
import MVP.Models.Classes.CellModels.MineCellModel;
import MVP.Models.Classes.FieldModels.FieldModel;
import MVP.Views.Classes.*;
import MVP.Views.Classes.Window;

import java.awt.*;
import java.awt.image.ImageObserver;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.function.Function;

/**
 * Created by NeiD on 09.09.2016.
 */
public class Main {

    private final static boolean isWindowVisible = true;

    private final static boolean isWindowResizable = false;

    private final static int horizontalCellsCount = 25;

    private final static int verticalCellsCount = 12;

    private final static int mineCellsCount = 50;

    private final static int horizontalCellViewSize = 64;

    private final static int verticalCellViewSize = 64;

    private final static int horizontalControlButtonSize = 128;

    private final static int verticalControlButtonSize = 128;

    private final static int horizontalTimerLabelSize = (horizontalCellsCount * horizontalCellViewSize - horizontalControlButtonSize) / 2;

    private final static int verticalTimerLabelSize = verticalControlButtonSize;

    private final static int horizontalFlagImageSize = horizontalCellViewSize;

    private final static int verticalFlagImageSize = verticalCellViewSize;

    private final static int horizontalFlagLabelSize = horizontalTimerLabelSize / 2;

    private final static int verticalFlagLabelSize = verticalCellViewSize;

    private final static int verticalSpace = 1;

    private final static int timerLabelTimerDelay = 50;

    private final static int fieldViewStopInputTimerDelay = 20;

    private final static long initialTime = 75600000;

    private final static int secondsOnFieldViewStopInputEventFire = 5000;

    private final static DateFormat dateFormat = new SimpleDateFormat("HH:mm:ss");

    private final static Font timerLabelFont = new Font("TimesNewRoman", Font.BOLD, 40);

    private final static Font flagsLabelFont = timerLabelFont;

    private final static String windowTitle = "Minesweeper";

    private final static ImageObserver imageObserver = null;

    private final static ImageObserver flagsPanelImageObserver = null;

    private final static Function<Integer, Integer> horizontalGenerateFunction = horizontalCellsCountParameter -> horizontalCellsCountParameter - 1;

    private final static Function<Integer, Integer> verticalGenerateFunction = verticalCellsCountParameter -> verticalCellsCountParameter - 1;

    private final static LayoutManager windowlayoutManager = new BorderLayout();

    private final static Object fieldViewConstraints = BorderLayout.CENTER;

    private final static Object controlPanelConstraints = BorderLayout.NORTH;

    public static void main(String[] args) {
        FieldModel<EmptyCellModel, MineCellModel> fieldModel = new FieldModel(horizontalCellsCount, verticalCellsCount, mineCellsCount);
        FieldView<Graphics2D> fieldView = new FieldView<>(horizontalCellsCount, verticalCellsCount, horizontalCellViewSize, verticalCellViewSize, imageObserver, fieldViewStopInputTimerDelay,
                secondsOnFieldViewStopInputEventFire);
        PseudoRandomFieldsGenerator<Graphics2D> pseudoRandomFieldsGenerator = new PseudoRandomFieldsGenerator<>(horizontalGenerateFunction, verticalGenerateFunction);
        ControlPanel controlPanel = new ControlPanel(horizontalCellsCount * horizontalCellViewSize, verticalControlButtonSize, horizontalControlButtonSize, verticalControlButtonSize,
                horizontalTimerLabelSize, verticalTimerLabelSize, initialTime, timerLabelTimerDelay, dateFormat, timerLabelFont, horizontalFlagImageSize,
                verticalFlagImageSize, horizontalFlagLabelSize, verticalFlagLabelSize, flagsLabelFont, verticalSpace, flagsPanelImageObserver, mineCellsCount);
        FieldsAndControlPanelPresenter<EmptyCellModel, MineCellModel, FieldModel<EmptyCellModel, MineCellModel>, Graphics2D, FieldView<Graphics2D>> fieldPresenter =
                new FieldsAndControlPanelPresenter<>(fieldModel, fieldView, controlPanel, pseudoRandomFieldsGenerator);
        Window<Graphics2D> window = new Window<>(windowTitle, isWindowVisible, isWindowResizable, windowlayoutManager, fieldView, fieldViewConstraints, controlPanel, controlPanelConstraints);
    }

}
