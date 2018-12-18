import { BaseFieldEntityGeneratorService } from "../base-field-entity-generator.service";
import { BaseFoodGeneratorService } from "./base-food-generator.service";
import { Bricks } from "src/app/models/bricks";

export class FoodGeneratorService extends BaseFoodGeneratorService {

    protected generateFoodBrickIndex(bricks: Bricks): number {
        let freeElementsIndexesArray: Array<number> = bricks.freeElementsIndexes.toArray();

        return freeElementsIndexesArray[BaseFieldEntityGeneratorService.parameterGenerator(0, freeElementsIndexesArray.length)];
    }
}