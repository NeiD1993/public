import IGridViewData from "./gridViewData";
import { IPagerDisplayProps } from "./child/pager/pagerProps";

interface IGridViewProps extends IGridViewData
{
    navigation: IPagerDisplayProps;
}

export default IGridViewProps;