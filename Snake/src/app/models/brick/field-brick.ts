import { BrickType } from "src/app/enums/brick-type";
import { StandardBrick } from "./standard-brick";

export class FieldBrick extends StandardBrick {

    private _type: BrickType;

    constructor(x: number, y: number, type: BrickType) {
        super(x, y);
        this.type = type;
    }

    set type(type: BrickType) {
        this._type = type;
    }

    get type(): BrickType {
        return this._type;
    }

    get typeToString(): string {
        return BrickType[this._type];
    }
}