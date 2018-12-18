package MVP.Views.Classes.CellViews;

import MVP.Views.Enums.CellViewStates.GeneralCellViewStates;
import MVP.Views.Interfaces.CellViews.ICellView;
import MVP.Views.Interfaces.CellViews.ICellViewStates;

import javax.imageio.ImageIO;
import java.awt.*;
import java.awt.image.ImageObserver;
import java.io.IOException;
import java.util.HashMap;
import java.util.Map;

/**
 * Created by NeiD on 08.09.2016.
 */
public abstract class BaseCellView<DisplayClass extends Graphics> extends Rectangle implements ICellView<DisplayClass> {

    protected static String imagesExtension;

    protected static String imagesFolder;

    protected static String exceptionMessage;

    protected static Map<ICellViewStates, Image> generalCellViewStatesMap;

    protected ICellViewStates cellViewState;

    static {
        imagesExtension = "png";
        imagesFolder = "Resources";
        exceptionMessage = "Exception in BaseCellView";
        generalCellViewStatesMap = new HashMap<>();
        getCellViewStatesMapByCellViewStates(generalCellViewStatesMap, GeneralCellViewStates.values());
    }

    BaseCellView(int horizontalCellViewCoordinate, int verticalCellViewCoordinate, int cellViewWidth, int cellViewHeight) {
        super(horizontalCellViewCoordinate, verticalCellViewCoordinate, cellViewWidth, cellViewHeight);
        cellViewState = GeneralCellViewStates.Initial;
    }

    protected abstract Image getCellViewImageByExtendedCellViewStates(ICellViewStates cellViewState);

    protected static void getCellViewStatesMapByCellViewStates(Map<ICellViewStates, Image> cellViewStatesMap, ICellViewStates[] cellViewStates) {
        try {
            for (ICellViewStates cellViewState : cellViewStates)

                cellViewStatesMap.put(cellViewState, ImageIO.read(BaseCellView.class.getClassLoader().getResource(imagesFolder + "/" + cellViewState.toString() + "." + imagesExtension)));
        }
        catch (IOException exception) {
            System.out.println(exceptionMessage + ".getCellViewStatesMapByCellViewStates");
        }
    }

    protected Image getCellViewImageByCellViewState(ICellViewStates cellViewState) {
        try {
            if (cellViewState.getClass() == GeneralCellViewStates.class)
                return generalCellViewStatesMap.get(cellViewState);
            else
                return getCellViewImageByExtendedCellViewStates(cellViewState);
        }
        catch (NullPointerException nullPointerException) {
            System.out.println(exceptionMessage + ".getCellViewImageByCellViewState");
        }
        return null;
    }

    @Override
    public void displayCellView(DisplayClass display, ImageObserver imageObserver) {
        try {
            display.drawImage(getCellViewImageByCellViewState(cellViewState), x, y, width, height, imageObserver);
        }
        catch (Exception exception) {
            System.out.println(exception.getMessage() + ".displayCellView");
        }
    }

    @Override
    public void setCellViewState(ICellViewStates cellViewState) {
        if (cellViewState != null)
            this.cellViewState = cellViewState;
    }

    @Override
    public void setHorizontalCellViewCoordinate(int horizontalCoordinate) { x = horizontalCoordinate; }

    @Override
    public void setVerticalCellViewCoordinate(int verticalCoordinate) {
        y = verticalCoordinate;
    }

    @Override
    public void setCellViewWidth(int width) {
        if (width > 0)
            this.width = width;
    }

    @Override
    public void setCellViewHeight(int height) {
        if (height > 0)
            this.height = height;
    }

    @Override
    public ICellViewStates getCellViewState() {
        return cellViewState;
    }

}
