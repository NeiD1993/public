import IGridViewData from "./gridViewData";

interface IGridViewState extends IGridViewData
{
    enumeration: IGridViewEnumerationState;
}

interface IGridViewEnumerationState
{
    count: number;

    originIndex: number;
}

export { IGridViewState, IGridViewEnumerationState };