import { BaseFieldGeneratorService } from "./base-field-generator.service";
import { Difficulty } from "src/app/enums/difficulty";

export class FieldGeneratorService extends BaseFieldGeneratorService {

    protected generateSideBricksCount(difficulty: Difficulty): number {
        switch (difficulty) {
            case Difficulty.Easy:
                return 10;
            case Difficulty.Medium:
                return 20;
            default:
                return 30;
        }
    }
}