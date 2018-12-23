import { Difficulty } from "src/app/enums/difficulty";
import { IBrickDecorationService } from "./i-brick-decoration.service";

export class BrickDecorationService implements IBrickDecorationService {

    getBoundaryBrickSpaceLength(difficulty: Difficulty): number {
        switch (difficulty) {
            case Difficulty.Easy:
                return 8;
            case Difficulty.Medium:
                return 6;
            default:
                return 4;
        }
    }

    getBoundaryLength(difficulty: Difficulty): number {
        switch (difficulty) {
            case Difficulty.Easy:
                return 10;
            case Difficulty.Medium:
                return 17;
            default:
                return 29;
        }
    }

    getBrickMarginLength(difficulty: Difficulty): number {
        switch (difficulty) {
            case Difficulty.Easy:
                return 4;
            case Difficulty.Medium:
                return 2;
            default:
                return 1;
        }
    }

    getBrickSideLength(difficulty: Difficulty): number {
        switch (difficulty) {
            case Difficulty.Easy:
                return 61;
            case Difficulty.Medium:
                return 30;
            default:
                return 20;
        }
    }
}