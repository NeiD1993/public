export class StandardBrick {

    private _x: number;

    private _y: number;

    constructor(x: number, y: number) {
        this.setX(x);
        this.setY(y);
    }

    private setX(x: number) {
        this._x = (x >= 0) ? x : -1;
    }

    get x(): number {
        return this._x;
    }

    private setY(y: number) {
        this._y = (y >= 0) ? y : -1;
    }

    get y(): number {
        return this._y;
    }
}