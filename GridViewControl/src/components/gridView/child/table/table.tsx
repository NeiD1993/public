import * as React from "react";
import HeaderCell from "./cell/header/headerCell";
import ITableProps from "./tableProps";
import ITableState from "./tableState";
import SortOrder from "./cell/header/sortOrder";
import { Filter, IFilterHighlightInfo } from "../filter/filter";
import "./table.css";
import "./cell/cell.css";
import "./cell/row/rowCell.css";

class Table extends React.Component<ITableProps, ITableState>
{
    private readonly _itemsKeys: Array<string>;

    private _clickedHeaderCell: HeaderCell;

    constructor(props: ITableProps) 
    {
        super(props);
        this.state = { view: new Map<any, Array<JSX.Element>>() };
        this._itemsKeys = Object.keys(this.props.data.items.first());
        this.onHeaderCellClick = this.onHeaderCellClick.bind(this);
    }

    private static formOrdering(column: string, sortOrder: SortOrder): (items: Array<any>) => Array<any> 
    {
        return (items) => 
        {
            let keySelector: (element: any) => string = (item) => item[column];

            return (sortOrder == SortOrder.Descending) ? items.orderByDescending(keySelector) : items.orderBy(keySelector);
        }
    }

    private renderDataRow(item: any)
    {            
        if (this.props.highlightText == "")
            return (
                <tr>
                    {Object.values(item).map((itemValue) => <td>{itemValue}</td>)}
                </tr>
            );
        else
            return this.renderDecoratedRow(item);
    }

    private renderDataRows()
    {                    
        if (this.props.data.enumeration == null)
            return this.props.data.items.select((item) => this.renderDataRow(item));
        else
        {
            let itemIndex: number = this.props.data.enumeration.originIndex;
            let rows: Array<JSX.Element> = new Array<JSX.Element>();
            
            while (rows.length < this.props.data.enumeration.count)
                rows.push(this.renderDataRow(this.props.data.items[itemIndex++]));

            return rows;
        }
    }

    private renderDecoratedRow(item: any)
    {        
        if (!this.state.view.has(item))
            this.state.view.set(item, Object.values(item).map((itemValue) => 
            {
                let highlightInfo: IFilterHighlightInfo = Filter.highlight(itemValue, this.props.highlightText, (matchText) => "<mark>" + matchText + "</mark>");
                
                return highlightInfo.success ? <td dangerouslySetInnerHTML={{ __html: highlightInfo.output }}/> : <td>{highlightInfo.output}</td>;
            }));

        return (
            <tr>
                {this.state.view.get(item)}
            </tr>
        );
    }

    private renderHeaders() 
    {
        return (
            <tr>
                {this._itemsKeys.map(itemKey => <HeaderCell text={itemKey} onClick={this.onHeaderCellClick} />)}
            </tr>
        );
    }

    private renderRows() 
    {
        switch (this.props.data.items.length)
        {
            case 0:
                return (
                    <tr>
                        <td colSpan={this._itemsKeys.length} className="empty">No data to display</td>
                    </tr>
                );
            default:
                return this.renderDataRows();
        }
    }

    public undecorate() : void
    {
        this.state.view.clear();
        this.setState({ view: this.state.view });
    }

    public render() 
    {
        return (
            <table id="table">
                {this.renderHeaders()}
                {this.renderRows()}
            </table>
        );
    }

    private onHeaderCellClick(clickedHeaderCell: HeaderCell): void 
    {
        this._clickedHeaderCell?.reset();
        this._clickedHeaderCell = clickedHeaderCell;
        this.props.onSort(Table.formOrdering(this._clickedHeaderCell.props.text, this._clickedHeaderCell.switch()));
    }
}

export default Table;