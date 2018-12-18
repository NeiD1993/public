import { BaseGameLogicService } from "./base-game-logic.service";
import { BonusType } from "src/app/enums/bonus-type";
import { Difficulty } from "src/app/enums/difficulty";
import { Direction } from "../../enums/direction";
import { Snake } from "src/app/models/snake/snake";

export class GameLogicService extends BaseGameLogicService {

    protected changeSnakeSpeed(bonusType: BonusType): void {
        switch (bonusType) {
            case BonusType.LevelDown:
                {
                    if ((this.field.snake.speedLevel > this._mathService.roundNumberDivision(Snake.speedLevelsCount, 2)) &&
                        (this.field.snake.setSpeedLevel(this.field.snake.speedLevel - 1))) {
                        this.snakeSpeedLevelChanged.next(BonusType.LevelDown);
                    }
                }
                break;
            default:
                {
                    if (this.field.snake.setSpeedLevel(this.field.snake.speedLevel + 1))
                        this.snakeSpeedLevelChanged.next(BonusType.LevelUp);
                }
        }
    }

    protected getFoodCountPercentage(): number {
        switch (this.difficulty) {
            case Difficulty.Easy:
                return 0.45;
            case Difficulty.Medium:
                return 0.4;
            default:
                return 0.35;
        }
    }

    protected getForbiddenDirection(): Direction {
        return this.field.snake.getInverseDirection();
    }

    protected getFoodGenerationDelay(): number {
        switch (this.difficulty) {
            case Difficulty.Easy:
                return 1550;
            case Difficulty.Medium:
                return 1350;
            default:
                return 1150;
        }
    }
}