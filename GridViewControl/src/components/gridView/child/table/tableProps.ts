import { IGridViewState } from "../../gridViewState";

interface ITableProps
{
    highlightText: string;

    data: IGridViewState;

    onSort: (ordering: (items: Array<any>) => Array<any>) => void;
}

export default ITableProps;