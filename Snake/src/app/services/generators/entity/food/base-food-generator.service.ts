import { BaseFieldEntityGeneratorService } from "../base-field-entity-generator.service";
import { BaseFieldGeneratorService } from "../../field/base-field-generator.service";
import { Bricks } from "src/app/models/bricks";
import { BrickType } from "src/app/enums/brick-type";
import { StandardBrick } from "src/app/models/brick/standard-brick";

export abstract class BaseFoodGeneratorService extends BaseFieldEntityGeneratorService {

    protected abstract generateFoodBrickIndex(bricks: Bricks): number;

    generateFood(bricks: Bricks): StandardBrick {
        let generatedFreeElementIndex: number = this.generateFoodBrickIndex(bricks);
        let foodBrick: StandardBrick = bricks.getBrick(generatedFreeElementIndex);

        BaseFieldGeneratorService.changeBrick(bricks, foodBrick, BrickType.Food, generatedFreeElementIndex);

        return new StandardBrick(foodBrick.x, foodBrick.y);
    }
}