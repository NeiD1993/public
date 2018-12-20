import { Direction } from "../../enums/direction";
import { SnakeBricksQueue } from "./snake-bricks-queue";

export class Snake {

    private _bricks: SnakeBricksQueue = new SnakeBricksQueue();

    private _direction: Direction;

    private _forbiddenDirection: Direction;

    private _speedLevel: number;

    get bricks(): SnakeBricksQueue {
        return this._bricks;
    }

    set direction(direction: Direction) {
        this._direction = direction;
    }

    get direction(): Direction {
        return this._direction;
    }

    set forbiddenDirection(forbiddenDirection: Direction) {
        this._forbiddenDirection = forbiddenDirection;
    }

    get forbiddenDirection(): Direction {
        return this._forbiddenDirection;
    }

    get speedLevel(): number {
        return this._speedLevel;
    }

    static getInverseDirection(direction: Direction): Direction {
        switch (direction) {
            case Direction.Down:
                return Direction.Up;
            case Direction.Left:
                return Direction.Right;
            case Direction.Right:
                return Direction.Left;
            default:
                return Direction.Down;
        }
    }

    directionToString(): string {
        return Direction[this._direction];
    }

    getInverseDirection(): Direction {
        return Snake.getInverseDirection(this.direction);
    }

    setSpeedLevel(speedLevel: number): boolean {
        if (speedLevel >= 1) {
            this._speedLevel = speedLevel;

            return true;
        }
        else
            return false;
    }
}