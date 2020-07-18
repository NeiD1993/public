import * as React from "react";
import "array-linq";
import ConstantsStorage from "../../services/constantsStorage";
import IGridViewProps from "./gridViewProps";
import Pager from "./child/pager/pager";
import Table from "./child/table/table";
import { Filter } from "./child/filter/filter";
import { CSSTransition, TransitionGroup } from "react-transition-group";
import { IGridViewState, IGridViewEnumerationState } from "./gridViewState";
import "./gridView.css";

class GridView extends React.Component<IGridViewProps, IGridViewState>
{
    private readonly _children: GridViewChildren = { filter: React.createRef(), table: React.createRef(), pager: React.createRef() };

    private _ordering: (items: Array<any>) => Array<any> = null;

    constructor(props: IGridViewProps)
    {
        super(props);
        this.state = { items: this.props.items, enumeration: Pager.calculateEnumeration(this.props.navigation.pageSize, this.props.items.length) };
        this.onFilterPicking = this.onFilterPicking.bind(this);
        this.onPagerNumbering = this.onPagerNumbering.bind(this);
        this.onTableSort = this.onTableSort.bind(this);
    }

    public render()
    {
        return (
            <TransitionGroup appear={true} component={null}>
                <CSSTransition classNames="gridView" timeout={ConstantsStorage.animationsTimeouts.appearance}>
                    <div id="gridView">
                        <Filter ref={this._children.filter} onFilter={this.onFilterPicking}/>
                        <Table ref={this._children.table} highlightText={((this._children.filter.current) == null) ? Filter.defaultExpression : this._children.filter.current.SearchPattern} data={this.state} onSort={this.onTableSort}/>
                        <Pager ref={this._children.pager} itemsAmount={this.state.items.length} display={this.props.navigation} onPaging={this.onPagerNumbering}/>
                    </div>
                </CSSTransition>
            </TransitionGroup>
        );
    }

    private onFilterPicking(searching: (items: Array<any>) => Array<any>) : void 
    {
        let filteredItems: Array<any> = (searching != null) ? searching(this.props.items) : this.props.items;

        this._children.table.current.undecorate();
        this._children.pager.current.restore();
        this.setState({ 
            items: ((this._ordering == null) ? filteredItems : this._ordering(filteredItems)), 
            enumeration: Pager.calculateEnumeration(this.props.navigation.pageSize, filteredItems.length) 
        });
    }

    private onPagerNumbering(enumeration: IGridViewEnumerationState) : void
    {
        this.setState({ items: this.state.items, enumeration: enumeration });
    }

    private onTableSort(ordering: (items: Array<any>) => Array<any>) : void
    {
        this._ordering = ordering;
        this.setState({ items: this._ordering(this.state.items), enumeration: this.state.enumeration });
    }
}

interface GridViewChildren
{
    filter: React.RefObject<Filter>;

    pager: React.RefObject<Pager>;

    table: React.RefObject<Table>;
}

export default GridView;