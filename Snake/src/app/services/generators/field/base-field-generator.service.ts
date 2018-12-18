import { BaseFoodGeneratorService } from "../entity/food/base-food-generator.service";
import { BaseSnakeGeneratorService } from "../entity/snake/base-snake-generator.service";
import { Bricks } from "src/app/models/bricks";
import { BrickType } from "src/app/enums/brick-type";
import { Difficulty } from "src/app/enums/difficulty";
import { Field } from "src/app/models/field";
import { FieldBrick } from "src/app/models/brick/field-brick";
import { StandardBrick } from "src/app/models/brick/standard-brick";

export abstract class BaseFieldGeneratorService {

    private static changeFreeElementIndex(bricks: Bricks, type: BrickType, index: number): void {
        if (type == BrickType.Empty)
            bricks.freeElementsIndexes.add(index);
        else
            bricks.freeElementsIndexes.remove(index);
    }

    private static generateBricks(sideBricksCount: number, bricks: Bricks): void {
        for (let x = 0; x < sideBricksCount; x++) {
            for (let y = 0; y < sideBricksCount; y++) {
                if (!bricks.elements[x][y])
                    BaseFieldGeneratorService.addBrick(bricks, new FieldBrick(x, y, BrickType.Empty));
            }
        }
    }

    static addBrick(bricks: Bricks, brick: FieldBrick): void {
        bricks.elements[brick.x][brick.y] = brick;
        BaseFieldGeneratorService.changeFreeElementIndex(bricks, brick.type, bricks.getBrickIndex(brick));
    }

    static changeBrick(bricks: Bricks, brick: StandardBrick, type: BrickType, index?: number): void {
        if (!index)
            index = bricks.getBrickIndex(brick);

        bricks.elements[brick.x][brick.y].type = type;
        BaseFieldGeneratorService.changeFreeElementIndex(bricks, type, index);
    }

    protected abstract generateSideBricksCount(difficulty: Difficulty): number;

    initializeField(difficulty: Difficulty, foodGeneratorService: BaseFoodGeneratorService, snakeGeneratorService: BaseSnakeGeneratorService,
        field: Field): boolean {
        let initializationResult;
        let sideBricksCount: number = this.generateSideBricksCount(difficulty);

        field.bricks.sideBricksCount = sideBricksCount;
        initializationResult = snakeGeneratorService.initializeSnake(field.bricks, field.snake);

        if (initializationResult) {
            BaseFieldGeneratorService.generateBricks(sideBricksCount, field.bricks);
            foodGeneratorService.generateFood(field.bricks);
        }

        return initializationResult;
    }
}