import { BaseGameLogicService } from "./base-game-logic.service";
import { Difficulty } from "src/app/enums/difficulty";
import { Direction } from "../../enums/direction";
import { SettingsOptionType } from "src/app/enums/settings-option-type";

export class GameLogicService extends BaseGameLogicService {

    protected decreaseSnakeSpeedLevelCondition(decreasedSnakeSpeed: number): boolean {
        return (decreasedSnakeSpeed >
            this._mathService.roundNumberDivision(this._settingsService.getSettingsOption(SettingsOptionType.SnakeSpeedLevelsCount), 2))
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

    protected increaseSnakeSpeedLevelCondition(increaseSnakeSpeed: number): boolean {
        return true;
    }
}