package General.Classes;

import General.Interfaces.ICell;
import General.Interfaces.IField;

/**
 * Created by NeiD on 05.09.2016.
 */
public abstract class BaseField<CellType extends ICell> implements IField<CellType> {

    protected static String exceptionMessage;

    protected int horizontalCellsCount;

    protected int verticalCellsCount;

    protected CellType[][] cells;

    static {
        exceptionMessage = "Exception in BaseField";
    }

    public BaseField(int horizontalCellsCount, int verticalCellsCount) {
        if (horizontalCellsCount > 0 && verticalCellsCount > 0) {
            this.horizontalCellsCount = horizontalCellsCount;
            this.verticalCellsCount = verticalCellsCount;
            cells = createCellTypes(horizontalCellsCount, verticalCellsCount);
        }
    }

    protected abstract CellType[][] createCellTypes(int horizontalCellsCount, int verticalCellsCount);

    @Override
    public void setHorizontalCellsCount(int horizontalCellsCount) {
        if (cells == null) {
            if (horizontalCellsCount > 0)
                this.horizontalCellsCount = horizontalCellsCount;
        }
    }

    @Override
    public void setVerticalCellsCount(int verticalCellsCount) {
        if (cells == null) {
            if (verticalCellsCount > 0)
                this.verticalCellsCount = verticalCellsCount;
        }
    }

    @Override
    public boolean isNullifyCells() {
        if (cells != null) {
            cells = null;
            return true;
        }
        return false;
    }

    @Override
    public boolean isResetCells() {
        if (horizontalCellsCount > 0 && verticalCellsCount > 0)
        {
            cells = createCellTypes(horizontalCellsCount, verticalCellsCount);
            return true;
        }
        return false;
    }

    @Override
    public int getHorizontalCellsCount() {
        return horizontalCellsCount;
    }

    @Override
    public int getVerticalCellsCount() {
        return verticalCellsCount;
    }

    @Override
    public CellType getCellByIndexes(int horizontalCellIndex, int verticalCellIndex) {
        try {
            return cells[horizontalCellIndex][verticalCellIndex];
        }
        catch (Exception exception) {
            System.out.println(exceptionMessage + ".getCellByIndexes");
            return null;
        }
    }

    @Override
    public CellType[][] getCells() {
        return cells;
    }

}
