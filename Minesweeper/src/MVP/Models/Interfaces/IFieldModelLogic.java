package MVP.Models.Interfaces;

import MVP.Presenters.Interfaces.Listeners.IEndEventListener;

/**
 * Created by NeiD on 07.09.2016.
 */
public interface IFieldModelLogic {

    void openCellModel(int horizontalCellModelIndex, int verticalCellModelIndex);

    void suggestCellModel(int horizontalCellModelIndex, int verticalCellModelIndex);

    void addEndEventListener(IEndEventListener endEventListener);

    void removeEndEventListener(IEndEventListener endEventListener);

    boolean checkingWinConditions();

    boolean checkingLoseConditions();

}
