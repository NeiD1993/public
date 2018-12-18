package MVP.Views.Enums.CellViewStates;

import MVP.Views.Interfaces.CellViews.ICellViewStates;

/**
 * Created by NeiD on 03.09.2016.
 */
public enum EmptyCellViewStates implements ICellViewStates {

    NoneMinesNear,

    OneMineNear,

    TwoMinesNear,

    ThreeMinesNear,

    FourMinesNear,

    FiveMinesNear,

    SixMinesNear,

    SevenMinesNear,

    EightMinesNear,

    WrongSuggested;

    public static EmptyCellViewStates getEmptyCellViewStateByMinesNearCount(int minesNearCount) {
        switch (minesNearCount) {
            case 1:
                return OneMineNear;
            case 2:
                return TwoMinesNear;
            case 3:
                return ThreeMinesNear;
            case 4:
                return FourMinesNear;
            case 5:
                return FiveMinesNear;
            case 6:
                return SixMinesNear;
            case 7:
                return SevenMinesNear;
            case 8:
                return EightMinesNear;
            default:
                return NoneMinesNear;
        }
    }

}
