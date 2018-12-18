import { Direction } from "../enums/direction";
import { StandardBrick } from "../models/brick/standard-brick";

export class BrickOperationsExecutorService {

    static getDirectionBetweenBricks(startBrick: StandardBrick, endBrick: StandardBrick): Direction | undefined {
        if ((startBrick.x == endBrick.x) && (startBrick.y != endBrick.y))
            return (startBrick.y > endBrick.y) ? Direction.Up : Direction.Down;
        else if ((startBrick.y == endBrick.y) && (startBrick.x != endBrick.x))
            return (startBrick.x < endBrick.x) ? Direction.Left : Direction.Right;
        else
            return undefined;
    }

    static move(brick: StandardBrick, direction: Direction, step: number): StandardBrick {
        if (step == 0)
            return brick;
        else if (step > 0) {
            switch (direction) {
                case Direction.Down:
                    return new StandardBrick(brick.x, brick.y + step);
                case Direction.Left:
                    return new StandardBrick(brick.x - step, brick.y);
                case Direction.Right:
                    return new StandardBrick(brick.x + step, brick.y);
                default:
                    return new StandardBrick(brick.x, brick.y - step);
            }
        }
        else
            return undefined;
    }

    move(brick: StandardBrick, direction: Direction, step: number): StandardBrick {
        return BrickOperationsExecutorService.move(brick, direction, step);
    }
}