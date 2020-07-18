import * as React from "react";
import * as replaceString from "replace-string";
import ConstantsStorage from "../../../../services/constantsStorage";
import IFilterProps from "./filterProps";
import { CSSTransition, TransitionGroup } from "react-transition-group";
import "./filter.css";

class Filter extends React.Component<IFilterProps>
{
    public static readonly defaultExpression = "";

    private readonly _input: React.RefObject<HTMLInputElement> = React.createRef();

    private _searchPattern: string = Filter.defaultExpression;

    constructor(props: IFilterProps) 
    {
        super(props);
        this.onButtonClick = this.onButtonClick.bind(this);
    }

    public get SearchPattern() : string
    {
        return this._searchPattern;
    }

    private formSearching() : (items: Array<any>) => Array<any>
    {
        return (items) => 
        {
            if (this._searchPattern == Filter.defaultExpression)
                return items;
            else
            {
                let searchChecker: RegExp = new RegExp(this._searchPattern, "i");

                return items.where(item => (Object.values(item).firstOrUndefined((itemValue) => searchChecker.test(itemValue.toString())) != undefined));
            }
        }
    }

    public static highlight(value: any, needle: string, decorator: (match: string) => string) : IFilterHighlightInfo
    {
        let info: IFilterHighlightInfo = { output: "", success: false };

        info.output = replaceString(value.toString(), needle, (matchedSubstring, matchesCount) => 
        {
            if (matchesCount == 1)
                info.success = true;

            return decorator(matchedSubstring);
        }, { caseInsensitive: true });

        return info;
    }

    public render() 
    {
        React.createRef()
        return (
            <TransitionGroup appear={true} component={null}>
                <CSSTransition classNames="interactive filter" timeout={ConstantsStorage.animationsTimeouts.appearance}>
                    <div id="filter">
                        <input ref={this._input} placeholder="Filter..."/>
                        <button onClick={this.onButtonClick}>OK</button>
                    </div>
                </CSSTransition>
            </TransitionGroup>
        );
    }

    private onButtonClick() : void
    {
        if (this._input.current.value != this._searchPattern)
        {
            this._searchPattern = this._input.current.value;
            this.props.onFilter(this.formSearching());
        }
    }
}

interface IFilterHighlightInfo
{
    success: boolean;

    output: string;
}

export { Filter, IFilterHighlightInfo };