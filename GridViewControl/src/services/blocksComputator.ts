class BlocksComputator
{
    private static defineShifts(middleLength: number) : IShifts
    {
        let halfLength: number = Math.floor(middleLength / 2);
        let restLength: number = middleLength - halfLength - 1;

        if (halfLength == restLength)
            return { minor: halfLength, major: halfLength };
        else
            return { minor: restLength, major: halfLength };
    }

    private static defineDistantBlocksInfo(sections: ISections, lengths: IBlockLengths) : Array<IBlockInfo>
    {
        if (sections.actual <= lengths.min)
            return this.evaluatePairBlocksInfo(sections.total, lengths, PairBlocksType.Left);
        else if ((sections.total - sections.actual + 1) <= lengths.min)
            return this.evaluatePairBlocksInfo(sections.total, lengths, PairBlocksType.Right)
        else
        {
            let shifts: IShifts = BlocksComputator.defineShifts(lengths.min + 1);

            if ((sections.total - sections.actual) <= (lengths.min + shifts.major))
                return this.evaluatePairBlocksInfo(sections.total, lengths, PairBlocksType.Right);
            else if ((sections.actual - lengths.min) <= (1 + shifts.minor))
                return this.evaluatePairBlocksInfo(sections.total, lengths, PairBlocksType.Left);
            else
                return [
                    { start: 1, end: lengths.min },
                    { start: (sections.actual - shifts.minor), end: (sections.actual + shifts.major) },
                    { start: (sections.total - lengths.min + 1), end: sections.total }
                ];
        }
    }
    
    private static evaluatePairBlocksInfo(sectionsTotal: number, lengths: IBlockLengths, type: PairBlocksType) : Array<IBlockInfo>
    {
        let doubleMinLength: number = 2 * lengths.min;

        return [
            { start: 1, end: ((type == PairBlocksType.Left) ? (doubleMinLength + 1) : lengths.min) },
            { start: ((type == PairBlocksType.Right) ? (sectionsTotal - doubleMinLength) : (sectionsTotal - lengths.min + 1)), end: sectionsTotal }
        ];
    }

    public static defineBlocksInfo(sections: ISections, lengths: IBlockLengths) : Array<IBlockInfo>
    {
        if (sections.total <= 1)
            return null;
        else
            return (sections.total <= lengths.max) ? [{ start: 1, end: sections.total }] : BlocksComputator.defineDistantBlocksInfo(sections, lengths);
    }
}

enum PairBlocksType
{
    Left,

    Right
}

interface IBlockInfo
{
    end: number;

    start: number;
}

interface IBlockLengths
{
    max: number, 
    
    min: number
}

interface ISections
{
    actual: number;

    total: number;
}

interface IShifts
{
    major: number,

    minor: number
}

export { BlocksComputator, IBlockInfo, ISections };