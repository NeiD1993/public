class ItemsExtractor
{
    private static readonly itemsCount: number = 100;

    public static obtain() : Array<any>
    {            
        return require("../items.json");
    }
}

export default ItemsExtractor;