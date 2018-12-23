import { AnimationMetadata } from "@angular/animations";
import { BaseAnimationServiceGameComponentViewState } from "../../../base.game.component/base-animation-service-game.component.view-state";
import { BaseGameLogicService } from "src/app/services/game-logic/base-game-logic.service";
import { BonusType } from "src/app/enums/bonus-type";
import { snakeDirectionSnakeSpeedLevelTextChangedAnimations } from "./snake-direction-dashboard.component.animations"
import { SettingsOptionType } from "src/app/enums/settings-option-type";
import { SettingsService } from "src/app/services/settings.service";

export class SnakeDirectionDashboardComponentViewState extends BaseAnimationServiceGameComponentViewState {

    private _speedLevelTextChangedAnimation: AnimationMetadata;

    get speedLevelTextChangedAnimation(): AnimationMetadata {
        return this._speedLevelTextChangedAnimation;
    }

    protected createAnimations(): AnimationMetadata[] {
        return Array.from(snakeDirectionSnakeSpeedLevelTextChangedAnimations.values());
    }

    getSpeedLevelTextContainerText(gameLogicService: BaseGameLogicService, settingsService: SettingsService): string {
        let snakeSpeedLevel: number = gameLogicService.field.snake.speedLevel;

        switch (snakeSpeedLevel) {
            case 1:
                return snakeSpeedLevel + ' [Min]';
            case settingsService.getSettingsOption(SettingsOptionType.SnakeSpeedLevelsCount):
                return snakeSpeedLevel + ' [Max]';
            default:
                return snakeSpeedLevel.toString();
        }
    }

    setSpeedLevelTextChangedAnimation(bonusType: BonusType) {
        this._speedLevelTextChangedAnimation = snakeDirectionSnakeSpeedLevelTextChangedAnimations.get(bonusType);
    }
}