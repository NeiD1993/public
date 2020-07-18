import { IGridViewEnumerationState } from "../../gridViewState";

interface IPagerProps
{
    itemsAmount: number;

    display: IPagerDisplayProps;

    onPaging: (enumeration: IGridViewEnumerationState) => void;
}

interface IPagerDisplayProps
{
    blockLength: number;

    pageSize: number;
}

export { IPagerProps, IPagerDisplayProps };