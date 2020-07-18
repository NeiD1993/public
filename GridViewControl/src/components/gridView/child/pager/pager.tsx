import * as React from "react";
import ConstantsStorage from "../../../../services/constantsStorage";
import IPagerState from "./pagerState";
import { IGridViewEnumerationState } from "../../gridViewState";
import { IPagerProps } from "./pagerProps";
import { BlocksComputator, IBlockInfo } from "../../../../services/blocksComputator";
import { CSSTransition, TransitionGroup } from "react-transition-group";
import "./pager.css";

class Pager extends React.Component<IPagerProps, IPagerState>
{
    private static readonly _restoredState: IPagerState = { currentPageNumber: 1 };

    private _wasInitialRender: boolean = false;

    constructor(props: IPagerProps)
    {
        super(props);
        this.restore();
        this.onPageLinkClick = this.onPageLinkClick.bind(this);
    }

    private renderPagesLinks(elements: Array<JSX.Element>, blockInfo: IBlockInfo) : void 
    {       
        let pageLink: JSX.Element;
        
        for (let pageNumber: number = blockInfo.start; pageNumber <= blockInfo.end; pageNumber++)
        {
            if (this.state.currentPageNumber == pageNumber)
                pageLink = <b className="chosen">{pageNumber}</b>
            else
                pageLink = <b className="unchosen" onClick={this.onPageLinkClick}>{pageNumber}</b>
                
            elements.push(pageLink);
        }
    }

    private renderInternally(blocksInfo: Array<IBlockInfo>)
    {
        let id: string = "pager";
        let elements: Array<JSX.Element> = this.renderElements(blocksInfo);

        if (this._wasInitialRender)
            return (
                <div id={id} className="pager-appear-done">
                    {elements}
                </div>
            );
        else
            return (
                <TransitionGroup appear={true} component={null}>
                    <CSSTransition classNames="interactive pager" timeout={ConstantsStorage.animationsTimeouts.appearance}>
                        <div id={id}>
                            {elements}
                        </div>
                    </CSSTransition>
                </TransitionGroup>
            );
    }

    private renderElements(blocksInfo: Array<IBlockInfo>)
    {
        let elements: Array<JSX.Element> = new Array<JSX.Element>();

        for (let blockIndex: number = 0; blockIndex < blocksInfo.length; blockIndex++)
        {
            this.renderPagesLinks(elements, blocksInfo[blockIndex]);

            if (blockIndex == (blocksInfo.length - 1))
                continue;
            else
                elements.push(<span>...</span>);
        }

        return elements;
    }

    public static calculateEnumeration(pageSize: number, itemsAmount: number, section: number = this._restoredState.currentPageNumber) : IGridViewEnumerationState
    {
        let originIndex: number = (section - 1) * pageSize;

        return { 
            count: ((originIndex + pageSize - 1) < itemsAmount) ? pageSize : (itemsAmount - originIndex), 
            originIndex : originIndex
        };
    }

    public restore() : void
    {
        if (this.state == null)
            this.state = Pager._restoredState;
        else
            this.setState(Pager._restoredState);
    }

    public render()
    {
        let blocksInfo: Array<IBlockInfo> = BlocksComputator.defineBlocksInfo(
            { actual: this.state.currentPageNumber, total: Math.ceil(this.props.itemsAmount / this.props.display.pageSize) }, 
            { min: this.props.display.blockLength, max: 3 * this.props.display.blockLength + 1 }
        );

        switch (blocksInfo != null)
        {
            case true:
                {
                    let result: JSX.Element = this.renderInternally(blocksInfo);

                    if (!this._wasInitialRender)
                        this._wasInitialRender = true;
                    
                    return result;
                }
            default:
                return null;
        }
    }

    private onPageLinkClick(event: any) : void
    {
        let pageNumber: number = Number.parseInt(event.target.innerText);

        this.setState({ currentPageNumber: pageNumber });
        this.props.onPaging(Pager.calculateEnumeration(this.props.display.pageSize, this.props.itemsAmount, pageNumber));
    }
}

export default Pager;