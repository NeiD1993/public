import { BaseFieldEntityGeneratorService } from "../base-field-entity-generator.service";
import { BaseSnakeGeneratorService } from "./base-snake-generator.service";
import { Bricks } from "src/app/models/bricks";
import { Direction } from "src/app/enums/direction";
import { Snake } from "src/app/models/snake/snake";
import { StandardBrick } from "src/app/models/brick/standard-brick";

export class SnakeGeneratorService extends BaseSnakeGeneratorService {

    protected static headPositionGenerator = (bricks: Bricks) => {
        let sideBricksHalfCount: number = bricks.getSideBricksCountPartLength(2);
        let sideBricksQuarterCount: number = bricks.getSideBricksCountPartLength(4);

        return () => BaseFieldEntityGeneratorService.parameterGenerator(sideBricksHalfCount - sideBricksQuarterCount, sideBricksHalfCount +
            sideBricksQuarterCount + 1);
    }

    protected static isDirectionCorrect(bricks: Bricks, headPosition: StandardBrick, length: number, direction: Direction): boolean {
        switch (direction) {
            case Direction.Down:
                return (headPosition.y - length >= 0);
            case Direction.Left:
                return (headPosition.x + length < bricks.sideBricksCount);
            case Direction.Right:
                return (headPosition.x - length >= 0);
            default:
                return (headPosition.y + length < bricks.sideBricksCount);
        }
    }

    protected generateDirection(bricks: Bricks, headPosition: StandardBrick, length: number): Direction {
        let directionIndex: number;
        let direction: Direction;
        let isDirectionGenerated: boolean = false;
        let directions: string[] = Object.keys(Direction).filter(directionKey => !isNaN(Number(Direction[directionKey])));

        while (!isDirectionGenerated && (directions.length > 0)) {
            directionIndex = BaseFieldEntityGeneratorService.parameterGenerator(0, directions.length);
            direction = (<any>Direction)[directions[directionIndex]];

            if (SnakeGeneratorService.isDirectionCorrect(bricks, headPosition, length, direction))
                isDirectionGenerated = true;

            directions.splice(directionIndex, 1);
        }

        return isDirectionGenerated ? direction : undefined;
    }

    protected generateHeadPosition(bricks: Bricks): StandardBrick {
        let headPositionGenerator = SnakeGeneratorService.headPositionGenerator(bricks);

        return new StandardBrick(headPositionGenerator(), headPositionGenerator());
    }

    protected generateLength(bricks: Bricks): number {
        return Snake.minLength;
    }
}