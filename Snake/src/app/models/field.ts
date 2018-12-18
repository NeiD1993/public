import { Bricks } from "./bricks";
import { Snake } from "./snake/snake";

export class Field {

    private _bricks: Bricks;

    private _snake: Snake;

    constructor(bricks: Bricks, snake: Snake) {
        this._bricks = bricks;
        this._snake = snake;
    }

    get bricks(): Bricks {
        return this._bricks;
    }

    get snake(): Snake {
        return this._snake;
    }
}