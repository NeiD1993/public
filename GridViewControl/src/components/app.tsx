import * as React from "react";
import GridView from "./gridView/gridView";
import ItemsExtractor from "../services/itemsExtractor";

class App extends React.Component
{
    public render()
    {
        return <GridView items={ItemsExtractor.obtain()} navigation={{ blockLength: 3, pageSize: 10 }}/>;
    }
}

export default App;