package General.Interfaces;

/**
 * Created by NeiD on 05.09.2016.
 */
public interface IField<CellType extends ICell> {

    void setHorizontalCellsCount(int horizontalCellsCount);

    void setVerticalCellsCount(int verticalCellsCount);

    boolean isNullifyCells();

    boolean isResetCells();

    int getHorizontalCellsCount();

    int getVerticalCellsCount();

    CellType getCellByIndexes(int horizontalCellIndex, int verticalCellIndex);

    CellType[][] getCells();

}
