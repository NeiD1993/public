import * as React from "react";
import ConstantsStorage from "../../../../../../services/constantsStorage";
import IHeaderCellProps from "./headerCellProps";
import IHeaderCellState from "./headerCellState";
import SortOrder from "./sortOrder";
import { CSSTransition, TransitionGroup } from "react-transition-group";
import "../cell.css";
import "./headerCell.css";

class HeaderCell extends React.Component<IHeaderCellProps, IHeaderCellState>
{
    constructor(props: IHeaderCellProps) 
    {
        super(props);
        this.state = { sortOrder: SortOrder.None };
    }

    private renderArrow()
    {
        if (this.state.sortOrder == SortOrder.None)
            return null;
        else
            return <i className={(this.state.sortOrder == SortOrder.Descending) ? "fas fa-long-arrow-alt-down" : "fas fa-long-arrow-alt-up"}/>
    }

    public render() 
    {
        return (            
            <TransitionGroup appear={true} component={null}>
                <CSSTransition classNames="interactive th" timeout={ConstantsStorage.animationsTimeouts.appearance}>
                    <th onClick={() => this.props.onClick(this)}>
                        {this.props.text}
                        {this.renderArrow()}
                    </th>
                </CSSTransition>
            </TransitionGroup>
        );
    }

    public reset(): void 
    {
        this.setState({ sortOrder: SortOrder.None });
    }

    public switch(): SortOrder 
    {
        let switchedSortOrder: SortOrder = (this.state.sortOrder == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending;

        this.setState({ sortOrder: switchedSortOrder });

        return switchedSortOrder;
    }
}

export default HeaderCell;